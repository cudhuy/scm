/* eslint-disable @typescript-eslint/no-explicit-any */
import { CustomError } from '@/types/type';
import { auth, signOut } from './auth';
import { signOut as clientSignOut } from 'next-auth/react';
import { toast } from 'sonner';

// Constants
const PUBLIC_API_URL = process.env.NEXT_PUBLIC_API_URL_PUBLIC || '';
const PRIVATE_API_URL = process.env.NEXT_PUBLIC_API_URL_PRIVATE || '';
const DEFAULT_TIMEOUT = 15000;
const LOGIN_URL = '/auth/login'; // Define your login URL

// Định nghĩa các options mở rộng cho fetch
type ExtendedRequestInit = RequestInit & {
  params?: Record<string, string | number | boolean | undefined | null>;
  timeout?: number;
  next?: NextFetchRequestConfig;
  skipAuthRedirect?: boolean; // Option to skip auth redirect
  responseType?: 'json' | 'blob' | 'arrayBuffer' | 'text'; // Option to specify response type
};

type NextFetchRequestConfig = {
  revalidate?: number | false; // cho ISR
  tags?: string[]; // cho on-demand revalidation
};

// Helper để tạo URL với params
function createUrl(baseUrl: string, path: string, params?: Record<string, any>): string {
  const base = new URL(baseUrl);
  const fullPath = `${base.pathname.replace(/\/+$/, '')}/${path.replace(/^\/+/, '')}`;
  const url = new URL(fullPath, base.origin);

  if (params) {
    Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .forEach(([key, value]) => {
        url.searchParams.append(key, String(value));
      });
  }

  return url.toString();
}

// Timer để xử lý timeout
function timeoutPromise(ms: number): Promise<never> {
  return new Promise((_, reject) => {
    setTimeout(() => {
      reject(new Error(`Request timeout after ${ms}ms`));
    }, ms);
  });
}

// Logger function
function logRequest(type: 'request' | 'response' | 'response data' | 'error', data: any) {
  if (process.env.NODE_ENV === 'development') {
    if (type === 'error') {
      console.error(`[Fetch ${type}]:`, data);
    } else if (type === 'response data') {
      console.log(`[Fetch ${type}]:`, JSON.stringify(data, null, 2));
    } else {
      console.log(`[Fetch ${type}]:`, data);
    }
  }
}

// Helper để chuẩn hóa headers thành Record<string, string>
function normalizeHeaders(inputHeaders?: HeadersInit): Record<string, string> {
  const headers: Record<string, string> = {};

  if (!inputHeaders) {
    return headers;
  }

  if (inputHeaders instanceof Headers) {
    inputHeaders.forEach((value, key) => {
      headers[key] = value;
    });
  } else if (Array.isArray(inputHeaders)) {
    inputHeaders.forEach(([key, value]) => {
      headers[key] = value;
    });
  } else {
    Object.assign(headers, inputHeaders);
  }

  return headers;
}

/**
 * Base fetch implementation
 */
async function fetchWithBase<T>(
  url: string,
  baseUrl: string,
  options: ExtendedRequestInit = {},
): Promise<T> {
  const {
    params,
    timeout = DEFAULT_TIMEOUT,
    next,
    skipAuthRedirect = false,
    responseType = 'json',
    ...fetchOptions
  } = options;

  const fullUrl = createUrl(baseUrl, url, params);
  const isClient = typeof window !== 'undefined';

  logRequest('request', {
    url: fullUrl,
    method: fetchOptions.method || 'GET',
    params,
    body: fetchOptions.body,
    next,
    cache: fetchOptions.cache,
    responseType,
  });

  try {
    const headers = normalizeHeaders(fetchOptions.headers);
    if (!(fetchOptions.body instanceof FormData)) {
      headers['Content-Type'] = 'application/json';
    }

    // Only set Accept to application/json if we're expecting JSON
    if (responseType === 'json') {
      headers['Accept'] = 'application/json';
    }

    const response = await Promise.race([
      fetch(fullUrl, {
        ...fetchOptions,
        headers,
        credentials: 'include',
        next,
      }),
      timeoutPromise(timeout),
    ]);

    logRequest('response', {
      status: response.status,
      statusText: response.statusText,
      headers: Object.fromEntries([...response.headers.entries()]),
    });

    // Handle 401 Unauthorized response
    if (response.status === 401) {
      console.error('401 Unauthorized response received');

      if (skipAuthRedirect) {
        const errorData = await response.json().catch(() => ({ message: 'Unauthorized' }));
        const error: CustomError = new Error(errorData.message || 'Unauthorized');
        error.status = 401;
        error.data = errorData;
        throw error;
      }

      // Sign out và redirect
      console.warn('Unauthorized, signing out');

      if (isClient) {
        toast.error('Phiên của bạn đã hết hạn. Vui lòng đăng nhập lại.');
        await clientSignOut({ redirect: true, redirectTo: LOGIN_URL });
      } else {
        await signOut({ redirectTo: LOGIN_URL });
      }

      // Ném lỗi để dừng luồng
      const error: CustomError = new Error('Phiên của bạn đã hết hạn. Vui lòng đăng nhập lại.');
      error.status = 401;
      throw error;
    }

    if (!response.ok) {
      // For error responses, try to parse as JSON first
      const errorData = await response.json().catch(() => ({}));
      const error: CustomError = new Error(
        errorData.message || `Request failed with status ${response.status}`,
      );
      error.status = response.status;
      error.data = errorData;
      throw error;
    }

    // Process response based on the specified responseType
    let data: any;
    switch (responseType) {
      case 'blob':
        data = await response.blob();
        logRequest('response data', '[Blob data]');
        break;
      case 'arrayBuffer':
        data = await response.arrayBuffer();
        logRequest('response data', '[ArrayBuffer data]');
        break;
      case 'text':
        data = await response.text();
        logRequest('response data', data);
        break;
      case 'json':
      default:
        data = await response.json();
        logRequest('response data', data);
        break;
    }

    return data as T;
  } catch (error: any) {
    logRequest('error', {
      message: error.message,
      stack: error.stack,
      status: error.status,
    });
    throw error;
  }
}

/**
 * Public API fetch function
 */
export async function fetchPublic<T>(url: string, options: ExtendedRequestInit = {}): Promise<T> {
  console.log(`Making public request to: ${PUBLIC_API_URL}${url}`);
  return fetchWithBase<T>(url, PUBLIC_API_URL, options);
}

/**
 * Private API fetch function
 * Tự động thêm token từ session nếu chạy trên server và không có token trong headers
 */
export async function fetchPrivate<T>(url: string, options: ExtendedRequestInit = {}): Promise<T> {
  console.log(`Making private request to: ${PRIVATE_API_URL}${url}`);

  const headers = normalizeHeaders(options.headers);

  if (!headers.Authorization && typeof window === 'undefined') {
    try {
      const session = await auth();
      if (session?.user) {
        headers.Authorization = `Bearer ${session.user.accessToken}`;
        console.log('Automatically added token from session');
      } else {
        console.log('No session token available, proceeding without Authorization');
      }
    } catch (error) {
      console.error('Failed to get session for private request:', error);
    }
  }

  return fetchWithBase<T>(url, PRIVATE_API_URL, { ...options, headers });
}
