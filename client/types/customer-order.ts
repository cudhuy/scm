export interface Order {
  orderId: string;
  orderCode: string;
  totalAmount: number;
  status: number;
  orderType: string;
  createdAt: string;
  recipientName: string;
  recipientPhone: string;
  recipientEmail: string;
  isPaid?: boolean;
  paymentMethod: number;
}

export interface OrderList {
  items: Order[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
  pageSize: number;
}
export interface CreateOrderDto {
  customerId: string;
  branchId?: string;
  tableId?: string;
  totalAmount: number;
  orderDetails: {
    productId: string;
    quantity: number;
  }[];
  voucherId: string | null;
  shippingAddress: string;
  recipientPhone: string;
  recipientName: string;
  voucherAmount?: number;
  shippingFee?: number;
  paymentMethod?: number;
  preTotalAmount: number;
}

export interface CreateOrderInStoreDto {
  customerId?: string;
  tableId?: string;
  totalAmount: number;
  orderDetails: {
    productId: string;
    quantity: number;
  }[];
  voucherId?: string;
  voucherAmount: number;
  preTotalAmount: number;
}

export interface OrderDetail {
  orderId: string;
  orderCode: string;
  totalAmount: number;
  status: number;
  orderType: string;
  createdAt: string;
  customerName: string;
  customerEmail: string;
  cancellationReason?: string;
  orderDetails: {
    productName: string;
    quantity: number;
    price: number;
    img: string;
  }[];
  branchName: string;
  tableName?: string;
  recipientPhone?: string;
  recipientName?: string;
  shippingAddress?: string;
  voucherAmount?: number;
  shippingFee?: number;
  isPaid?: boolean;
  paymentMethod: number;
  
}

export interface CancelOderDto {
  cancelReasonId: string;
  reason?: string;
}

export interface CancelReason {
  id: string;
  reason: string;
}

export interface CreateOrderResponse {
  orderId: string;
}

export enum OrderStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Canceled = 3,
  Shipping = 5,
  Processed = 4,
}

export const OrderStatusDisplayConfig = {
  [OrderStatus.Pending]: {
    color: "yellow",
    label: "Chờ xử lý",
  },
  [OrderStatus.Processing]: {
    color: "blue",
    label: "Đang xử lý",
  },
  [OrderStatus.Completed]: {
    color: "green",
    label: "Hoàn thành",
  },
  [OrderStatus.Canceled]: {
    color: "red",
    label: "Đã hủy",
  },
  [OrderStatus.Shipping]: {
    color: "orange",
    label: "Đang giao",
  },
  [OrderStatus.Processed]: {
    color: "purple",
    label: "Đã xử lý",
  },
};
