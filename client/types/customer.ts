export interface Customer {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  dob?: string;
  isVerified: boolean;
  status?: string;
  rankId: string;
  rankName: string;
  avatar?: string;
  address?: string;
}

export interface UpdateCustomerDto {
  id: string;
  isVerified?: boolean;
  rankId?: string;
  status?: string;
}

export interface GetCustomersResponse {
  items: Customer[];
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

export interface CustomerProfile {
  id: string;
  fullName: string;
  email: string;
  phone?: string;
  dob?: string;
  address?: string;
  isVerified: boolean;
  rankName?: string;
  avatar?: string;
  status: string;
  totalOrders: number;
  pendingOrders: number;
  completedOrders: number;
}
