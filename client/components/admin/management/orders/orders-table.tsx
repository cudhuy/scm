'use client';

/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Card, CardContent } from '@/components/ui/card';
import { AlertTriangle, Loader2, Package } from 'lucide-react';
import type { OrderDetailDto, OrderDto } from '@/services/orderAdminService';
import { formatCurrency } from '@/lib/utils';
import { OrderDetailView } from './order-detail-view';
import { getStatusBadge } from '@/components/global/order-status-badge';
import { cn } from '@/lib/utils';

type OrderTableProps = {
  orders: any[];
  type: 'order-online' | 'order-offline';
  error?: Error | null;
  isError?: boolean;
  isLoading?: boolean;
  isSearching?: boolean;
  orderDetails: OrderDetailDto;
  isLoadingDetails?: boolean;
  detailsError?: any;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (isOpen: boolean) => void;
  onViewDetail?: (item: any) => void;
};

export function OrderTable({
  orders,
  type,
  error,
  isError = false,
  isLoading = false,
  isSearching = false,
  orderDetails,
  isLoadingDetails,
  detailsError,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  onViewDetail,
}: OrderTableProps) {
  // Local state for UI interactions
  const [selectedRow, setSelectedRow] = useState<number | null>(null);
  const [hoveredRow, setHoveredRow] = useState<number | null>(null);

  const renderTableHeader = () => {
    if (type === 'order-online') {
      return (
        <TableRow>
          <TableHead className="w-[120px]">Mã Đơn Hàng</TableHead>
          <TableHead>Tên Khách Hàng</TableHead>
          <TableHead className="hidden md:table-cell">Người Nhận</TableHead>
          <TableHead className="hidden lg:table-cell">Số Điện Thoại</TableHead>
          <TableHead className="hidden xl:table-cell">Địa Chỉ Giao</TableHead>
          <TableHead>Tổng Tiền</TableHead>
          <TableHead>Trạng Thái</TableHead>
          <TableHead className="text-right">Thao Tác</TableHead>
        </TableRow>
      );
    }

    return (
      <TableRow>
        <TableHead className="w-[120px]">Mã Đơn Hàng</TableHead>
        <TableHead>Tên Khách Hàng</TableHead>
        <TableHead className="hidden md:table-cell">Chi Nhánh</TableHead>
        <TableHead>Tổng Tiền</TableHead>
        <TableHead className="hidden lg:table-cell">Ngày Tạo</TableHead>
        <TableHead>Trạng Thái</TableHead>
        <TableHead className="text-right">Thao Tác</TableHead>
      </TableRow>
    );
  };

  const handleViewDetail = (order: OrderDto) => {
    setSelectedRow(orders.findIndex((o) => o.orderId === order.orderId));
    onViewDetail?.(order);
  };

  const renderTableRow = (order: OrderDto, index: number) => {
    const isHovered = hoveredRow === index;

    if (type === 'order-online') {
      return (
        <TableRow
          key={`online-${order.orderCode}-${index}`}
          className={cn('transition-colors duration-200', isHovered ? 'bg-slate-50' : '')}
          onMouseEnter={() => setHoveredRow(index)}
          onMouseLeave={() => setHoveredRow(null)}
        >
          <TableCell className="font-medium">{order.orderCode}</TableCell>
          <TableCell>{order.customerName}</TableCell>
          <TableCell className="hidden md:table-cell">{order.recipientName}</TableCell>
          <TableCell className="hidden lg:table-cell">{order.recipientPhone}</TableCell>
          <TableCell
            className="hidden xl:table-cell max-w-[180px] truncate"
            title={order.shippingAddress}
          >
            {order.shippingAddress}
          </TableCell>
          <TableCell className="font-medium">{formatCurrency(order.totalAmount)}</TableCell>
          <TableCell>{getStatusBadge(order.status)}</TableCell>
          <TableCell className="text-right">
            <OrderDetailView
              orderDetails={orderDetails || ({} as OrderDetailDto)}
              isDetailDialogOpen={selectedRow === index && isDetailDialogOpen}
              setIsDetailDialogOpen={setIsDetailDialogOpen}
              isLoadingDetails={isLoadingDetails}
              detailsError={detailsError}
              onViewDetail={() => handleViewDetail(order)}
              order={order}
              type={type}
            />
          </TableCell>
        </TableRow>
      );
    }

    return (
      <TableRow
        key={`offline-${order.orderCode}-${index}`}
        className={cn('transition-colors duration-200', isHovered ? 'bg-slate-50' : '')}
        onMouseEnter={() => setHoveredRow(index)}
        onMouseLeave={() => setHoveredRow(null)}
      >
        <TableCell className="font-medium">{order.orderCode}</TableCell>
        <TableCell>{order.customerName}</TableCell>
        <TableCell className="hidden md:table-cell">{order.branchName}</TableCell>
        <TableCell className="font-medium">{formatCurrency(order.totalAmount)}</TableCell>
        <TableCell className="hidden lg:table-cell">
          {new Date(order.createdAt).toLocaleDateString('vi-VN')}
        </TableCell>
        <TableCell>{getStatusBadge(order.status)}</TableCell>
        <TableCell className="text-right">
          <OrderDetailView
            orderDetails={orderDetails || ({} as OrderDetailDto)}
            isDetailDialogOpen={selectedRow === index && isDetailDialogOpen}
            setIsDetailDialogOpen={setIsDetailDialogOpen}
            isLoadingDetails={isLoadingDetails}
            detailsError={detailsError}
            onViewDetail={() => handleViewDetail(order)}
            order={order}
            type={type}
          />
        </TableCell>
      </TableRow>
    );
  };

  const renderCardView = (order: OrderDto, index: number) => {
    return (
      <Card
        key={`card-${order.orderCode}-${index}`}
        className="mb-4 overflow-hidden transition-all duration-200 hover:shadow-md"
      >
        <CardContent className="p-0">
          <div className="p-3 flex justify-between items-center border-b">
            <div className="flex items-center">
              <Package className="h-4 w-4 mr-2 text-slate-500" />
              <span className="font-medium">{order.orderCode}</span>
            </div>
            <div>{getStatusBadge(order.status)}</div>
          </div>

          <div className="p-4 space-y-2">
            <div className="grid grid-cols-2 gap-2">
              <div className="text-sm text-slate-500">Khách hàng</div>
              <div className="text-sm font-medium">{order.customerName}</div>
            </div>

            {type === 'order-online' ? (
              <>
                <div className="grid grid-cols-2 gap-2">
                  <div className="text-sm text-slate-500">Người nhận</div>
                  <div className="text-sm font-medium">{order.recipientName}</div>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div className="text-sm text-slate-500">SĐT</div>
                  <div className="text-sm font-medium">{order.recipientPhone}</div>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div className="text-sm text-slate-500">Địa chỉ</div>
                  <div className="text-sm font-medium">{order.shippingAddress}</div>
                </div>
              </>
            ) : (
              <>
                <div className="grid grid-cols-2 gap-2">
                  <div className="text-sm text-slate-500">Chi nhánh</div>
                  <div className="text-sm font-medium">{order.branchName}</div>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div className="text-sm text-slate-500">Ngày tạo</div>
                  <div className="text-sm font-medium">
                    {new Date(order.createdAt).toLocaleDateString('vi-VN')}
                  </div>
                </div>
              </>
            )}

            <div className="grid grid-cols-2 gap-2">
              <div className="text-sm text-slate-500">Tổng tiền</div>
              <div className="text-sm font-medium text-emerald-600">
                {formatCurrency(order.totalAmount)}
              </div>
            </div>
          </div>

          <div className="border-t p-3 flex justify-end">
            <OrderDetailView
              orderDetails={orderDetails || ({} as OrderDetailDto)}
              isDetailDialogOpen={selectedRow === index && isDetailDialogOpen}
              setIsDetailDialogOpen={setIsDetailDialogOpen}
              isLoadingDetails={isLoadingDetails}
              detailsError={detailsError}
              onViewDetail={() => handleViewDetail(order)}
              order={order}
              type={type}
            />
          </div>
        </CardContent>
      </Card>
    );
  };

  const renderEmptyState = () => (
    <div className="flex flex-col items-center justify-center py-10 text-center">
      <div className="rounded-full bg-slate-100 p-3 mb-4">
        <Package className="h-6 w-6 text-slate-400" />
      </div>
      <h3 className="text-lg font-medium mb-1">Không có đơn hàng</h3>
      <p className="text-sm text-slate-500 max-w-sm">
        {isSearching
          ? 'Không tìm thấy kết quả tìm kiếm. Vui lòng thử lại với từ khóa khác.'
          : 'Chưa có đơn hàng nào được tạo. Đơn hàng mới sẽ xuất hiện ở đây.'}
      </p>
    </div>
  );

  const renderLoadingState = () => (
    <div className="flex justify-center items-center py-10">
      <div className="flex flex-col items-center">
        <Loader2 className="h-8 w-8 animate-spin text-slate-400 mb-2" />
        <p className="text-sm text-slate-500">Đang tải dữ liệu...</p>
      </div>
    </div>
  );

  const renderErrorState = () => {
    if (isError) {
      if (error && (error as any).status === 404) {
        return <span>Không tìm thấy dữ liệu yêu cầu</span>;
      }

      return (
        <div className="flex justify-center items-center">
          <AlertTriangle className="h-6 w-6 mr-2" />
          <span>{error?.message || 'Đã xảy ra lỗi khi tải dữ liệu'}</span>
        </div>
      );
    }
  };

  const renderTableContent = () => {
    if (isLoading) {
      return (
        <TableRow>
          <TableCell colSpan={8} className="h-24 text-center">
            {renderLoadingState()}
          </TableCell>
        </TableRow>
      );
    }

    if (isError) {
      return (
        <TableRow>
          <TableCell colSpan={8} className="h-24 text-center">
            {renderErrorState()}
          </TableCell>
        </TableRow>
      );
    }

    if (orders.length > 0) {
      return orders.map((order, index) => renderTableRow(order, index));
    }

    return (
      <TableRow>
        <TableCell colSpan={8} className="h-24 text-center">
          {renderEmptyState()}
        </TableCell>
      </TableRow>
    );
  };

  return (
    <div className="space-y-4">
      {/* Table view for larger screens */}
      <div className="hidden sm:block rounded-md border overflow-hidden shadow-xs">
        <Table>
          <TableHeader>{renderTableHeader()}</TableHeader>
          <TableBody>{renderTableContent()}</TableBody>
        </Table>
      </div>

      {/* Card view for smaller screens */}
      <div className="sm:hidden space-y-4">
        {isLoading ? (
          renderLoadingState()
        ) : isError ? (
          renderErrorState()
        ) : orders.length > 0 ? (
          <div className="space-y-4">
            {orders.map((order, index) => renderCardView(order, index))}
          </div>
        ) : (
          renderEmptyState()
        )}
      </div>
    </div>
  );
}
