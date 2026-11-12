/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { Card, CardHeader } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Eye } from 'lucide-react';
import {
  OrderDto,
  ProductBestSellingDto,
  RecentCustomerDto,
  UserDetailDto,
} from '@/types/dashboard';
import { formatDateLocale, formatCurrency } from '@/lib/utils';
import { getStatusBadge } from '@/components/global/order-status-badge';
import { ImageDisplay } from '@/components/global/image-display';
import { OrderDetailDto } from '@/services/orderAdminService';
import { OrderDetailView } from './order-detail-view';
import { ProductDetail } from '@/types/product-admin';
import { ProductDetailView } from './product-detail-view';
import { UserDetailView } from './user-detail-view';
import { useState } from 'react';

interface TabsSectionProps {
  latestOrders: OrderDto[];
  isLoadingLatestOrders: boolean;
  isErrorLatestOrders: boolean;
  errorLatestOrders: any;
  topSellingProducts: ProductBestSellingDto[];
  isLoadingTopSellingProducts: boolean;
  isErrorTopSellingProducts: boolean;
  errorTopSellingProducts: any;
  recentCustomers: RecentCustomerDto[];
  isLoadingRecentCustomers: boolean;
  isErrorRecentCustomers: boolean;
  errorRecentCustomers: any;
  activeTab: string;
  setActiveTab: (tab: string) => void;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (open: boolean) => void;
  orderDetails: OrderDetailDto;
  isLoadingOrderDetails: boolean;
  detailsErrorOrder: any;
  productDetails: ProductDetail;
  isLoadingProductDetail: boolean;
  productDetailError: any;
  userDetails: UserDetailDto;
  isLoadingUserDetail: boolean;
  userDetailError: any;
  onViewUserDetail: (userId: string) => void;
  onViewProductDetail: (productId: string) => void;
  onViewOrderDetail: (orderId: string) => void;
}

export default function TabsSection({
  latestOrders,
  isLoadingLatestOrders,
  isErrorLatestOrders,
  errorLatestOrders,
  topSellingProducts,
  isLoadingTopSellingProducts,
  isErrorTopSellingProducts,
  errorTopSellingProducts,
  recentCustomers,
  isLoadingRecentCustomers,
  isErrorRecentCustomers,
  errorRecentCustomers,
  activeTab,
  setActiveTab,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  orderDetails,
  isLoadingOrderDetails,
  detailsErrorOrder,
  productDetails,
  isLoadingProductDetail,
  productDetailError,
  userDetails,
  isLoadingUserDetail,
  userDetailError,
  onViewUserDetail,
  onViewProductDetail,
  onViewOrderDetail,
}: TabsSectionProps) {
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const handleViewOrderDetail = (orderId: string) => {
    setSelectedOrderId(orderId);
    onViewOrderDetail(orderId);
    setIsDetailDialogOpen(true);
  };

  const handleViewProductDetail = (productId: string) => {
    setSelectedProductId(productId);
    onViewProductDetail(productId);
    setIsDetailDialogOpen(true);
  };

  const handleViewUserDetail = (userId: string) => {
    setSelectedUserId(userId);
    onViewUserDetail(userId);
    setIsDetailDialogOpen(true);
  };

  return (
    <Card>
      <CardHeader>
        <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
          <div className="flex items-center justify-between">
            <TabsList>
              <TabsTrigger value="recent-orders">Đơn hàng gần đây</TabsTrigger>
              <TabsTrigger value="best-selling">Sản phẩm bán chạy</TabsTrigger>
              <TabsTrigger value="recent-customers">Khách hàng gần đây</TabsTrigger>
            </TabsList>
          </div>
          <TabsContent
            value="recent-orders"
            className="border-none p-0 pt-3 max-w-[450px] sm:max-w-full"
          >
            {isLoadingLatestOrders ? (
              <div className="flex justify-center py-8">Đang tải đơn hàng...</div>
            ) : isErrorLatestOrders ? (
              <div className="flex justify-center py-8 text-destructive">
                Lỗi khi tải đơn hàng: {errorLatestOrders?.message || 'Lỗi không xác định'}
              </div>
            ) : (
              <div className="overflow-x-auto">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="text-nowrap">Khách hàng</TableHead>
                      <TableHead className="text-nowrap">Nguồn</TableHead>
                      <TableHead className="text-nowrap min-w-[120px]">Trạng thái</TableHead>
                      <TableHead className="text-nowrap">Ngày</TableHead>
                      <TableHead className="text-nowrap">Số tiền</TableHead>
                      <TableHead className="text-nowrap">Hành động</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {latestOrders.map((order) => (
                      <TableRow key={order.orderId}>
                        <TableCell>
                          <div className="font-medium">{order.customerName}</div>
                        </TableCell>
                        <TableCell>{order.type}</TableCell>
                        <TableCell>{getStatusBadge(order.status)}</TableCell>
                        <TableCell>{formatDateLocale(order.createdAt)}</TableCell>
                        <TableCell>{formatCurrency(order.totalAmount)}</TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="p-1 flex items-center gap-1 sm:gap-2 sm:p-2"
                            onClick={() => handleViewOrderDetail(order.orderId)}
                          >
                            <Eye className="h-3 w-3" />
                            <span>Xem chi tiết</span>
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            )}
          </TabsContent>
          <TabsContent
            value="best-selling"
            className="border-none p-0 pt-3 max-w-[450px] sm:max-w-full"
          >
            {isLoadingTopSellingProducts ? (
              <div className="flex justify-center py-8">Đang tải sản phẩm...</div>
            ) : isErrorTopSellingProducts ? (
              <div className="flex justify-center py-8 text-destructive">
                Lỗi khi tải sản phẩm: {errorTopSellingProducts?.message || 'Lỗi không xác định'}
              </div>
            ) : (
              <div className="overflow-x-auto">
                <Table className="w-full">
                  <TableHeader>
                    <TableRow>
                      <TableHead className="text-nowrap min-w-[120px]">Sản phẩm</TableHead>
                      <TableHead className="text-nowrap min-w-[100px]">Danh mục</TableHead>
                      <TableHead className="text-nowrap min-w-[80px]">Giá</TableHead>
                      <TableHead className="text-nowrap min-w-[60px]">Hành động</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {topSellingProducts.map((product) => (
                      <TableRow key={product.id}>
                        <TableCell className="max-w-[200px]">
                          <div className="flex items-center gap-3">
                            <ImageDisplay image={product.image} name={product.name} size="sm" />
                            <div className="font-medium truncate">{product.name}</div>
                          </div>
                        </TableCell>
                        <TableCell className="truncate">{product.categoryName}</TableCell>
                        <TableCell>{formatCurrency(product.price)}</TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="p-1 flex items-center gap-1 sm:gap-2 sm:p-2 whitespace-nowrap"
                            onClick={() => handleViewProductDetail(product.id)}
                          >
                            <Eye className="h-3 w-3" />
                            <span>Xem chi tiết</span>
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            )}
          </TabsContent>
          <TabsContent
            value="recent-customers"
            className="border-none p-0 pt-3  max-w-[450px] sm:max-w-full"
          >
            {isLoadingRecentCustomers ? (
              <div className="flex justify-center py-8">Đang tải khách hàng...</div>
            ) : isErrorRecentCustomers ? (
              <div className="flex justify-center py-8 text-destructive">
                Lỗi khi tải khách hàng: {errorRecentCustomers?.message || 'Lỗi không xác định'}
              </div>
            ) : (
              <div className="overflow-x-auto">
                <Table className="w-full">
                  <TableHeader>
                    <TableRow>
                      <TableHead>Khách hàng</TableHead>
                      <TableHead className="text-nowrap">Email</TableHead>
                      <TableHead className="text-nowrap">Tổng chi tiêu</TableHead>
                      <TableHead className="text-nowrap">Mua lần cuối</TableHead>
                      <TableHead className="text-nowrap">Hành động</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {recentCustomers.map((customer) => (
                      <TableRow key={customer.customerId}>
                        <TableCell>
                          <div className="font-medium">{customer.customerName}</div>
                        </TableCell>
                        <TableCell>{customer.email}</TableCell>
                        <TableCell>{formatCurrency(customer.totalSpent)}</TableCell>
                        <TableCell>{formatDateLocale(customer.lastPurchase)}</TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="p-1 flex items-center gap-1 sm:gap-2 sm:p-2"
                            onClick={() => handleViewUserDetail(customer.customerId)}
                          >
                            <Eye className="h-3 w-3" />
                            <span>Xem chi tiết</span>
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            )}
          </TabsContent>
        </Tabs>
      </CardHeader>
      {selectedOrderId && (
        <OrderDetailView
          orderId={selectedOrderId}
          orderDetails={orderDetails}
          isDetailDialogOpen={isDetailDialogOpen}
          setIsDetailDialogOpen={setIsDetailDialogOpen}
          isLoadingOrderDetails={isLoadingOrderDetails}
          detailsErrorOrder={detailsErrorOrder}
          onViewOrderDetail={onViewOrderDetail}
        />
      )}
      {selectedProductId && (
        <ProductDetailView
          productId={selectedProductId}
          productDetails={productDetails}
          isDetailDialogOpen={isDetailDialogOpen}
          setIsDetailDialogOpen={setIsDetailDialogOpen}
          isLoadingProductDetails={isLoadingProductDetail}
          detailsErrorProduct={productDetailError}
          onViewProductDetail={onViewProductDetail}
        />
      )}
      {selectedUserId && (
        <UserDetailView
          userId={selectedUserId}
          userDetails={userDetails}
          isDetailDialogOpen={isDetailDialogOpen}
          setIsDetailDialogOpen={setIsDetailDialogOpen}
          isLoadingUserDetails={isLoadingUserDetail}
          userDetailError={userDetailError}
          onViewUserDetail={onViewUserDetail}
        />
      )}
    </Card>
  );
}
