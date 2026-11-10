/* eslint-disable @typescript-eslint/no-explicit-any */
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Eye, Loader2 } from 'lucide-react';
import { Calendar, Package, Truck, User } from 'lucide-react';
import { OrderDetailDto, OrderDto } from '@/services/orderAdminService';
import { formatCurrency, formatDateLocale } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { CancelOrderDialog } from './orders-online/cancel-order-online';
import { useState } from 'react';
import { AcceptOrderDialog } from './orders-online/accept-order-online';
import { DeliveryOrderDialog } from './orders-online/delivery-order-dialog';
import { getStatusBadge } from '@/components/global/order-status-badge';

interface OrderDetailViewProps {
  orderDetails: OrderDetailDto;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (open: boolean) => void;
  isLoadingDetails?: boolean;
  detailsError?: any;
  onViewDetail?: (item: any) => void;
  order: OrderDto;
  type: string;
}

export function OrderDetailView({
  orderDetails,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  isLoadingDetails = false,
  detailsError = null,
  onViewDetail,
  order,
  type,
}: OrderDetailViewProps) {
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [showAcceptDialog, setShowAcceptDialog] = useState(false);
  const [showDeliveryDialog, setShowDeliveryDialog] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<any>(null);

  const handleCancelClick = (order: any) => {
    setSelectedOrder(order);
    setShowCancelDialog(true);
  };

  const handleAcceptClick = (order: any) => {
    setSelectedOrder(order);
    setShowAcceptDialog(true);
  };

  const handleDeliverClick = (order: any) => {
    setSelectedOrder(order);
    setShowDeliveryDialog(true);
  };

  const OrderDetailContent = () => (
    <div className="grid gap-3 sm:gap-4">
      <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-2">
        <h2 className="text-xl sm:text-2xl font-bold">Đơn hàng #{orderDetails.orderCode}</h2>
      </div>

      <Separator />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-3 sm:gap-4">
        <Card>
          <CardHeader className="pb-1">
            <CardTitle className="text-base font-medium flex items-center gap-2">
              <Package className="h-4 w-4" />
              Thông Tin Đơn Hàng
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-1.5 text-sm sm:text-base">
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Loại đơn hàng:</span>
              <span className="font-medium">{orderDetails.orderType}</span>
            </div>
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Trạng thái:</span>
              <span className="font-medium">{getStatusBadge(orderDetails.status)}</span>
            </div>
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Chi nhánh:</span>
              <span className="font-medium">{orderDetails.branchName || 'Không có'}</span>
            </div>
            {orderDetails.tableName && (
              <div className="flex justify-between flex-wrap gap-1">
                <span className="text-muted-foreground">Tên bàn:</span>
                <span className="font-medium">{orderDetails.tableName}</span>
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-1">
            <CardTitle className="text-base font-medium flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              Thông Tin Thời Gian
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-1.5 text-sm sm:text-base">
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Ngày tạo:</span>
              <span className="font-medium">{formatDateLocale(orderDetails.createdAt)}</span>
            </div>
            {orderDetails.cancellationReason && (
              <div className="flex justify-between flex-wrap gap-1">
                <span className="text-muted-foreground">Lý do hủy:</span>
                <span className="font-medium">{orderDetails.cancellationReason}</span>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader className="pb-1">
          <CardTitle className="text-base font-medium flex items-center gap-2">
            <User className="h-4 w-4" />
            Thông Tin Khách Hàng & Giao Hàng
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-1.5 text-sm sm:text-base">
          <div className="flex justify-between flex-wrap gap-1">
            <span className="text-muted-foreground">Tên khách hàng:</span>
            <span className="font-medium">
              {orderDetails.customerName ? (
                orderDetails.customerName
              ) : (
                <span className="italic text-red-500">Khách chưa có tài khoản</span>
              )}
            </span>
          </div>
          {orderDetails.customerEmail && (
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Email:</span>
              <span className="font-medium">{orderDetails.customerEmail}</span>
            </div>
          )}
          {type === 'order-online' && (
            <>
              <div className="flex justify-between flex-wrap gap-1">
                <span className="text-muted-foreground">Người nhận:</span>
                <span className="font-medium">{orderDetails.recipientName}</span>
              </div>
              <div className="flex justify-between flex-wrap gap-1">
                <span className="text-muted-foreground">Số điện thoại:</span>
                <span className="font-medium">{orderDetails.recipientPhone}</span>
              </div>
              <div className="flex justify-between flex-wrap gap-1">
                <span className="text-muted-foreground">Địa chỉ giao:</span>
                <span className="font-medium">{orderDetails.shippingAddress}</span>
              </div>
            </>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="pb-1">
          <CardTitle className="text-base font-medium flex items-center gap-2">
            <Truck className="h-4 w-4" />
            Chi Tiết Sản Phẩm & Thanh Toán
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-1.5 text-sm sm:text-base">
          {orderDetails.orderDetails.map((item, index) => (
            <div key={index} className="flex justify-between border-b py-1.5 flex-wrap gap-1">
              <span className="text-muted-foreground">
                {item.productName} (x{item.quantity})
              </span>
              <span className="font-medium">{formatCurrency(item.price)}</span>
            </div>
          ))}

          {orderDetails.voucherAmount > 0 && (
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Giảm giá:</span>
              <span className="font-medium">-{formatCurrency(orderDetails.voucherAmount)}</span>
            </div>
          )}

          {orderDetails.shippingFee > 0 && (
            <div className="flex justify-between flex-wrap gap-1">
              <span className="text-muted-foreground">Phí giao hàng:</span>
              <span className="font-medium">{formatCurrency(orderDetails.shippingFee)}</span>
            </div>
          )}

          <div className="flex justify-between flex-wrap gap-1">
            <span className="text-muted-foreground">Phương thức thanh toán:</span>
            <span className="font-medium">
              {orderDetails.paymentMethod === 1 ? 'Chuyển khoản' : 'Tiền mặt'}
            </span>
          </div>

          <div className="flex justify-between flex-wrap gap-1">
            <span className="text-muted-foreground">Trạng thái thanh toán:</span>
            <span
              className={`font-medium ${
                orderDetails.isPaid ? 'text-green-600' : 'text-yellow-600'
              }`}
            >
              {orderDetails.isPaid ? 'Đã thanh toán' : 'Chưa thanh toán'}
            </span>
          </div>

          <div className="flex justify-between pt-1.5 flex-wrap gap-1">
            <span className="text-muted-foreground font-bold">Tổng tiền:</span>
            <span className="font-bold">{formatCurrency(orderDetails.totalAmount)}</span>
          </div>
        </CardContent>
      </Card>

      {type === 'order-online' && (
        <div className="flex flex-col sm:flex-row sm:justify-end gap-2 mt-3">
          {order.status === 0 && (
            <>
              <Button
                variant="outline"
                onClick={() => handleCancelClick(order)}
                className="text-red-500 border-red-500 h-10 w-full sm:w-auto"
              >
                Hủy đơn
              </Button>
              <Button
                onClick={() => handleAcceptClick(order)}
                className="bg-green-600 hover:bg-green-700 text-white h-10 w-full sm:w-auto"
              >
                Chấp nhận đơn
              </Button>
            </>
          )}
          {order.status === 1 && (
            <Button
              onClick={() => handleDeliverClick(order)}
              className="bg-blue-600 hover:bg-blue-700 text-white h-10 w-full sm:w-auto"
            >
              Giao hàng
            </Button>
          )}
        </div>
      )}
    </div>
  );

  return (
    <>
      <Dialog open={isDetailDialogOpen} onOpenChange={setIsDetailDialogOpen}>
        <DialogTrigger asChild>
          <Button variant="ghost" size="icon" onClick={onViewDetail}>
            <Eye className="h-4 w-4" />
          </Button>
        </DialogTrigger>
        <DialogContent className="max-w-[90vw] sm:max-w-md md:max-w-lg overflow-y-auto max-h-[90vh] p-4">
          <DialogHeader>
            <DialogTitle className="text-lg sm:text-xl">Chi Tiết Đơn Hàng</DialogTitle>
          </DialogHeader>
          {isLoadingDetails ? (
            <div className="flex justify-center items-center py-4">
              <Loader2 className="h-6 w-6 animate-spin mr-2" />
              <span>Đang tải dữ liệu...</span>
            </div>
          ) : detailsError ? (
            <div className="text-red-500 text-center py-4 text-sm sm:text-base">
              Đã xảy ra lỗi: {detailsError.message}
            </div>
          ) : orderDetails ? (
            <OrderDetailContent />
          ) : (
            <div className="text-center py-4 text-sm sm:text-base">Không có dữ liệu chi tiết.</div>
          )}
        </DialogContent>
      </Dialog>

      <CancelOrderDialog
        isOpen={showCancelDialog}
        onClose={() => setShowCancelDialog(false)}
        selectedOrder={selectedOrder}
      />

      <AcceptOrderDialog
        isOpen={showAcceptDialog}
        onClose={() => setShowAcceptDialog(false)}
        selectedOrder={selectedOrder}
      />

      <DeliveryOrderDialog
        isOpen={showDeliveryDialog}
        onClose={() => setShowDeliveryDialog(false)}
        selectedOrder={selectedOrder}
      />
    </>
  );
}
