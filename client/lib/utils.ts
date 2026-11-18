/* eslint-disable @typescript-eslint/no-explicit-any */
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';
import { format, getDaysInMonth, getMonth, getYear, parseISO } from 'date-fns';
import { Voucher } from '@/types/voucher';
import { vi } from 'date-fns/locale/vi';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

/**
 * Converts an array of objects to CSV format
 */
export function convertToCSV<T extends Record<string, any>>(data: T[]): string {
  if (data.length === 0) return '';

  // Get headers from the first object's keys
  const headers = Object.keys(data[0]);

  // Create header row
  const headerRow = headers.join(',');

  // Create data rows
  const rows = data.map((item) => {
    return headers
      .map((header) => {
        // Handle special characters and ensure proper CSV escaping
        const value = item[header] === null || item[header] === undefined ? '' : item[header];
        const valueStr = String(value);
        // Escape quotes and wrap in quotes if contains comma, quote or newline
        if (valueStr.includes(',') || valueStr.includes('"') || valueStr.includes('\n')) {
          return `"${valueStr.replace(/"/g, '""')}"`;
        }
        return valueStr;
      })
      .join(',');
  });

  // Combine header row and data rows
  return [headerRow, ...rows].join('\n');
}

/**
 * Format date and time in Vietnamese locale
 * Example output: "29 Tháng 3, 2025, 8:18 AM"
 */
export const formatDateTimeVietnamese = (dateInput: Date | string): string => {
  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput;

  if (isNaN(date.getTime())) {
    console.error('Invalid date value:', dateInput);
    return 'Ngày không hợp lệ';
  }

  // Format date and replace "Thg" with "Tháng"
  const formattedDate = format(date, 'd MMM, yyyy - h:mm a', { locale: vi });
  return formattedDate.replace(/\bthg\b/g, 'Tháng');
};

/**
 * Converts an array of objects to JSON format
 */
export function convertToJSON<T>(data: T[]): string {
  return JSON.stringify(data, null, 2);
}

/**
 * Generic export function for any data type
 */
export async function exportData<T extends Record<string, any>>(
  getData: () => Promise<T[]>,
  format: 'csv' | 'json',
): Promise<string> {
  const data = await getData();

  if (format === 'csv') {
    return convertToCSV(data);
  } else {
    return convertToJSON(data);
  }
}
export const formatDiscount = (voucher: Voucher) => {
  if (voucher.discountType === 1) {
    return `${voucher.discountValue || 0}% OFF`;
  } else {
    return `${(voucher.discountValue || 0).toLocaleString()} VND OFF`;
  }
};

export const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) {
    console.error('Invalid date value:', dateString);
    return 'Invalid date';
  }
  return format(date, 'PPP');
};

export const formatTime = (time: string): string => {
  let timePart = time;

  if (time.includes('.')) {
    const parts = time.split('.');
    timePart = parts[parts.length - 1];
  }

  const segments = timePart.split(':');
  if (segments.length < 2) {
    return 'Invalid format';
  }

  const hours = segments[0];
  const minutes = segments[1];

  return `${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}`;
};

export const formatDateTimeWithSeconds = (dateString?: string) => {
  if (!dateString) return 'N/A';
  try {
    return format(
      typeof dateString === 'string' ? parseISO(dateString) : new Date(dateString),
      'dd/MM/yyyy HH:mm:ss',
    );
  } catch {
    return 'Ngày không hợp lệ';
  }
};

export const formatTimeSubmit = (time: string) => {
  const parts = time.split(':');
  const hour = parts[0];
  const minute = parts[1];
  return `${hour}:${minute}:00}`;
};

export const capitalize = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  }).format(amount);
}

export function formatVND(amount: number, showCurrencyCode = true): string {
  const formatted = new Intl.NumberFormat('vi-VN', {
    style: 'decimal',
    maximumFractionDigits: 0,
  }).format(amount);

  return showCurrencyCode ? `${formatted} VND` : formatted;
}

export const formatDateLocale = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
};

export function formatDateToDDMMYYYY(dateInput: string | Date): string {
  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput;
  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
}

export function getCurrentMonths(selectedYear?: string) {
  const now = new Date();
  const currentYear = getYear(now);
  const currentMonth = getMonth(now);

  const year = selectedYear ? parseInt(selectedYear, 10) : currentYear;
  const months = [];

  const vietnameseMonths = [
    'Tháng 1', // January
    'Tháng 2', // February
    'Tháng 3', // March
    'Tháng 4', // April
    'Tháng 5', // May
    'Tháng 6', // June
    'Tháng 7', // July
    'Tháng 8', // August
    'Tháng 9', // September
    'Tháng 10', // October
    'Tháng 11', // November
    'Tháng 12', // December
  ];

  if (year < currentYear) {
    for (let i = 0; i < 12; i++) {
      months.push({
        id: String(i + 1),
        name: vietnameseMonths[i],
      });
    }
  } else if (year === currentYear) {
    for (let i = 0; i <= currentMonth; i++) {
      months.push({
        id: String(i + 1),
        name: vietnameseMonths[i],
      });
    }
  }

  return months;
}

export function getWeeksInMonth(monthNumber: string, yearNumber?: string) {
  const now = new Date();
  const year = yearNumber ? parseInt(yearNumber, 10) : getYear(now);
  const month = parseInt(monthNumber, 10) - 1;

  const totalDays = getDaysInMonth(new Date(year, month));
  const weeks = [];

  for (let weekIndex = 0; weekIndex < 3; weekIndex++) {
    const startDay = weekIndex * 7 + 1;
    const endDay = (weekIndex + 1) * 7;

    const startDate = new Date(year, month, startDay);
    const endDate = new Date(year, month, endDay);

    weeks.push({
      id: String(weekIndex + 1),
      name: `Tuần ${weekIndex + 1} (${format(startDate, 'dd/MM')} - ${format(endDate, 'dd/MM')})`,
    });
  }

  if (totalDays > 21) {
    const startDay = 22;
    const endDay = totalDays;

    const startDate = new Date(year, month, startDay);
    const endDate = new Date(year, month, endDay);

    weeks.push({
      id: '4',
      name: `Tuần 4 (${format(startDate, 'dd/MM')} - ${format(endDate, 'dd/MM')})`,
    });
  }

  return weeks;
}

// Helper function to translate role to Vietnamese
export const translateRole = (role: string): string => {
  switch (role?.toUpperCase()) {
    case 'ADMIN':
      return 'Quản trị viên';
    case 'MANAGER':
      return 'Quản lý';
    case 'STAFF':
      return 'Nhân viên';
    default:
      return 'ADMIN';
  }
};
