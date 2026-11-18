export interface MonthlyRevenueDto {
  month: string;
  revenue: number;
  categoryName: string;
  branchName: string;
}

export interface RevenueReportDto {
  total: number;
  monthlyRevenue: MonthlyRevenueDto[];
  leadingMonth: string;
}

export interface OrderData {
  day: string | null;
  count: number;
}

export interface OrderStatisticsResponseDto {
  branchId: string;
  week: number;
  month: number;
  year: number;
  orderStatisticsData: OrderData[];
  highestOrderInWeek: number;
}

export interface ReportSummaryDto {
  totalCustomers: number;
  totalAmount: number;
  totalOrders: number;
  totalProducts: number;
}

export interface OrderDto {
  orderId: string;
  customerName: string;
  type: string;
  status: number;
  createdAt: string;
  totalAmount: number;
}

export interface ProductBestSellingDto {
  id: string;
  name: string;
  price: number;
  categoryName: string;
  image: string;
}

export interface RecentCustomerDto {
  customerId: string;
  customerName: string;
  email: string;
  totalSpent: number;
  lastPurchase: string;
}

export interface UserDetailDto {
  id: string;
  fullName: string | null;
  email: string | null;
  phone: string | null;
  dob: string | null;
  address: string | null;
  isVerified: boolean;
  rankName: string | null;
  status: string | null;
  avatar: string | null;
}
