/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { DollarSign, Package, ShoppingCart, Users } from 'lucide-react';
import { Skeleton } from '@/components/ui/skeleton';
import { ReportSummaryDto } from '@/types/dashboard';
import { formatCurrency } from '@/lib/utils';

interface StatsSectionProps {
  reportSummary?: ReportSummaryDto;
  isLoading: boolean;
  isError: boolean;
  error?: any;
}

export default function StatsSection({
  reportSummary,
  isLoading,
  isError,
  error,
}: StatsSectionProps) {
  if (isLoading) {
    return (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {[...Array(4)].map((_, index) => (
          <Card key={index}>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <Skeleton className="h-4 w-24" />
              <Skeleton className="h-4 w-4" />
            </CardHeader>
            <CardContent>
              <Skeleton className="h-8 w-20 mb-2" />
              <Skeleton className="h-4 w-16" />
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  // If error, show error message
  if (isError) {
    return (
      <div className="rounded-md bg-destructive/15 p-4">
        <p className="text-destructive">
          Lỗi khi tải dữ liệu: {error?.message || 'Không thể tải thống kê'}
        </p>
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Tổng số khách hàng</CardTitle>
          <Users className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{reportSummary?.totalCustomers || 0}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Tổng doanh thu</CardTitle>
          <DollarSign className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">
            {reportSummary ? formatCurrency(reportSummary.totalAmount) : '0 VND'}
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Tổng số đơn hàng</CardTitle>
          <ShoppingCart className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{reportSummary?.totalOrders || 0}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Tổng số sản phẩm</CardTitle>
          <Package className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{reportSummary?.totalProducts || 0}</div>
        </CardContent>
      </Card>
    </div>
  );
}
