'use client';

import type React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Package, User, MapPin, Calendar, DollarSign } from 'lucide-react';
import { formatCurrency, formatDateLocale } from '@/lib/utils';
import { getStatusBadge } from '@/components/global/order-status-badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Separator } from '@/components/ui/separator';
import { OrderDetailDto } from '@/services/orderAdminService';
import { memo, useCallback } from 'react';

interface OrderDetailViewProps {
  orderId: string;
  orderDetails: OrderDetailDto | null;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (open: boolean) => void;
  isLoadingOrderDetails: boolean;
  detailsErrorOrder: Error | null;
  onViewOrderDetail: (orderId: string) => void;
}

export const OrderDetailView = memo(function OrderDetailView({
  orderId,
  orderDetails,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  isLoadingOrderDetails,
  detailsErrorOrder,
  onViewOrderDetail,
}: OrderDetailViewProps) {
  const handleOpenDialog = useCallback(() => {
    if (!isLoadingOrderDetails) {
      onViewOrderDetail(orderId);
      setIsDetailDialogOpen(true);
    }
  }, [orderId, isLoadingOrderDetails, onViewOrderDetail, setIsDetailDialogOpen]);

  return (
    <Dialog open={isDetailDialogOpen} onOpenChange={setIsDetailDialogOpen}>
      <DialogContent className="max-w-3xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-xl">
            {isLoadingOrderDetails ? (
              <Skeleton className="h-7 w-48" />
            ) : (
              `Đơn hàng #${orderDetails?.orderCode || 'Không tìm thấy'}`
            )}
          </DialogTitle>
        </DialogHeader>

        {isLoadingOrderDetails ? (
          <OrderDetailSkeleton />
        ) : detailsErrorOrder ? (
          <ErrorDisplay error={detailsErrorOrder} onRetry={handleOpenDialog} />
        ) : orderDetails ? (
          <OrderDetailContent orderDetails={orderDetails} />
        ) : (
          <EmptyState />
        )}
      </DialogContent>
    </Dialog>
  );
});

interface OrderDetailContentProps {
  orderDetails: OrderDetailDto;
}

const OrderDetailContent = memo(function OrderDetailContent({
  orderDetails,
}: OrderDetailContentProps) {
  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-muted/30 p-4 rounded-lg">
        <div className="flex flex-col">
          <div className="text-sm text-muted-foreground">Trạng thái</div>
          <div className="mt-1">{getStatusBadge(orderDetails.status)}</div>
        </div>
        <div className="flex flex-col items-end">
          <div className="text-sm text-muted-foreground">Ngày đặt hàng</div>
          <div className="flex items-center gap-1 mt-1">
            <Calendar className="h-4 w-4 text-muted-foreground" />
            <span>{formatDateLocale(orderDetails.createdAt)}</span>
          </div>
        </div>
      </div>

      <div className="grid md:grid-cols-2 gap-6">
        <div className="space-y-4">
          <div className="flex items-center gap-2">
            <User className="h-5 w-5 text-primary" />
            <h3 className="font-semibold">Thông tin khách hàng</h3>
          </div>
          <div className="bg-muted/20 p-4 rounded-lg space-y-3">
            <InfoItem
              label="Khách hàng"
              value={orderDetails.customerName || 'N/A'}
              valueClassName="font-medium"
            />
            <InfoItem label="Email" value={orderDetails.customerEmail || 'N/A'} />
            <InfoItem label="Loại đơn" value={orderDetails.orderType} />
          </div>
        </div>

        <div className="space-y-4">
          <div className="flex items-center gap-2">
            <MapPin className="h-5 w-5 text-primary" />
            <h3 className="font-semibold">Thông tin giao hàng</h3>
          </div>
          <div className="bg-muted/20 p-4 rounded-lg space-y-3">
            <InfoItem
              label="Người nhận"
              value={orderDetails.recipientName}
              valueClassName="font-medium"
            />
            <InfoItem label="Số điện thoại" value={orderDetails.recipientPhone} />
            <InfoItem label="Địa chỉ" value={orderDetails.shippingAddress} />
          </div>
        </div>
      </div>

      {orderDetails.cancellationReason && (
        <div className="bg-destructive/10 p-4 rounded-lg border border-destructive/20">
          <h3 className="font-semibold text-destructive mb-1">Lý do hủy</h3>
          <p className="text-sm">{orderDetails.cancellationReason}</p>
        </div>
      )}

      <Separator />

      <div className="space-y-4">
        <div className="flex items-center gap-2">
          <Package className="h-5 w-5 text-primary" />
          <h3 className="font-semibold">Chi tiết đơn hàng</h3>
        </div>
        <div className="border rounded-lg overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="bg-muted/50">
                  <th className="p-3 text-left font-medium">Sản phẩm</th>
                  <th className="p-3 text-right font-medium">Số lượng</th>
                  <th className="p-3 text-right font-medium">Giá</th>
                  <th className="p-3 text-right font-medium">Tổng</th>
                </tr>
              </thead>
              <tbody>
                {orderDetails.orderDetails?.map((item, index) => (
                  <tr key={index} className="border-t hover:bg-muted/20 transition-colors">
                    <td className="p-3">
                      <div className="flex items-center gap-3">
                        <div className="h-10 w-10 rounded bg-muted/30 flex items-center justify-center">
                          <Package className="h-5 w-5 text-muted-foreground" />
                        </div>
                        <span className="font-medium">{item.productName}</span>
                      </div>
                    </td>
                    <td className="p-3 text-right">{item.quantity}</td>
                    <td className="p-3 text-right">{formatCurrency(item.price)}</td>
                    <td className="p-3 text-right">{formatCurrency(item.price * item.quantity)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div className="bg-muted/20 p-4 rounded-lg flex justify-between items-center">
        <div className="flex items-center gap-2">
          <DollarSign className="h-5 w-5 text-primary" />
          <span className="font-semibold">Tổng cộng</span>
        </div>
        <div className="text-xl font-bold text-primary">
          {formatCurrency(orderDetails.totalAmount)}
        </div>
      </div>
    </div>
  );
});

function OrderDetailSkeleton() {
  return (
    <div className="space-y-6">
      <Skeleton className="h-16 w-full rounded-lg" />
      <div className="grid md:grid-cols-2 gap-6">
        <div className="space-y-4">
          <Skeleton className="h-6 w-40" />
          <Skeleton className="h-32 w-full rounded-lg" />
        </div>
        <div className="space-y-4">
          <Skeleton className="h-6 w-40" />
          <Skeleton className="h-32 w-full rounded-lg" />
        </div>
      </div>
      <Skeleton className="h-px w-full" />
      <div className="space-y-4">
        <Skeleton className="h-6 w-32" />
        <Skeleton className="h-40 w-full rounded-lg" />
      </div>
      <Skeleton className="h-16 w-full rounded-lg" />
    </div>
  );
}

interface ErrorDisplayProps {
  error: Error;
  onRetry: () => void;
}

function ErrorDisplay({ error, onRetry }: ErrorDisplayProps) {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-red-100">
        <span className="text-red-600 text-xl">!</span>
      </div>
      <h3 className="text-lg font-medium text-destructive">Không thể tải chi tiết đơn hàng</h3>
      <p className="text-sm text-muted-foreground">
        {error?.message || 'Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.'}
      </p>
      <Button variant="outline" onClick={onRetry}>
        Thử lại
      </Button>
    </div>
  );
}

function EmptyState() {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted">
        <Package className="h-6 w-6 text-muted-foreground" />
      </div>
      <h3 className="text-lg font-medium">Không có chi tiết đơn hàng</h3>
      <p className="text-sm text-muted-foreground">
        Không tìm thấy thông tin đơn hàng được yêu cầu.
      </p>
    </div>
  );
}

interface InfoItemProps {
  label: string;
  value: React.ReactNode;
  valueClassName?: string;
}

const InfoItem = memo(function InfoItem({ label, value, valueClassName = '' }: InfoItemProps) {
  return (
    <div className="flex flex-col">
      <span className="text-sm text-muted-foreground">{label}</span>
      <span className={`mt-1 ${valueClassName}`}>{value}</span>
    </div>
  );
});
