/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Bar, BarChart, Line, LineChart, XAxis, YAxis } from 'recharts';
import { ChartContainer, ChartTooltip } from '@/components/ui/chart';
import type { Category } from '@/types/category';
import type { OrderStatisticsResponseDto, RevenueReportDto } from '@/types/dashboard';
import { cn } from '@/lib/utils';
import { TrendingUp, TrendingDown, DollarSign, ShoppingBag } from 'lucide-react';

interface ChartsSectionProps {
  categories: Category[];
  errorCategories: any;
  isErrorCategories: boolean;
  isLoadingCategories: boolean;
  categoryFilter: string;
  setCategoryFilter: (value: string) => void;
  branchFilter: string;
  selectedYear: string;
  selectedMonth: string;
  selectedWeek: string;
  setSelectedYear: (value: string) => void;
  setSelectedMonth: (value: string) => void;
  setSelectedWeek: (value: string) => void;
  months: Array<{ id: string; name: string }>;
  weeks: Array<{ id: string; name: string }>;
  revenueReport?: RevenueReportDto;
  errorRevenueReport: any;
  isErrorRevenueReport: boolean;
  isLoadingRevenueReport: boolean;
  orderStatistics?: OrderStatisticsResponseDto;
  errorOrderStatistics: any;
  isErrorOrderStatistics: boolean;
  isLoadingOrderStatistics: boolean;
}

export default function ChartsSection({
  categories,
  categoryFilter,
  setCategoryFilter,
  selectedYear,
  selectedMonth,
  selectedWeek,
  setSelectedMonth,
  setSelectedWeek,
  months,
  weeks,
  revenueReport,
  orderStatistics,
  isLoadingRevenueReport,
  isErrorRevenueReport,
  errorRevenueReport,
  isLoadingOrderStatistics,
  isErrorOrderStatistics,
  errorOrderStatistics,
}: ChartsSectionProps) {
  const formattedSalesData = orderStatistics
    ? orderStatistics.orderStatisticsData.map((item) => ({
        name: item.day ? item.day.substring(0, 3) : 'Không có',
        value: item.count,
      }))
    : [];

  const formattedRevenueData = revenueReport
    ? revenueReport.monthlyRevenue.map((item) => ({
        name: item.month.substring(0, 3),
        value: item.revenue,
      }))
    : [];

  const totalRevenue = revenueReport ? revenueReport.total : 0;

  const OrdersTooltipContent = ({ active, payload, label }: any) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      const value = data.value;
      const prevValue = formattedSalesData[payload[0].index - 1]?.value || 0;
      const isIncrease = value > prevValue;

      return (
        <div className="rounded-lg border bg-background p-3 shadow-md">
          <div className="flex items-center gap-2 mb-2">
            <ShoppingBag className="h-4 w-4 text-orange-500" />
            <span className="font-semibold">Đơn hàng - {label}</span>
          </div>
          <div className="grid gap-2">
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Số lượng:</span>
              <span className="font-bold text-sm">{(value || 0).toLocaleString()}</span>
            </div>
            {payload[0].index > 0 && (
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">So với ngày trước:</span>
                <div
                  className={cn(
                    'flex items-center gap-1 text-sm font-medium',
                    isIncrease ? 'text-green-500' : 'text-red-500',
                  )}
                >
                  {isIncrease ? (
                    <TrendingUp className="h-3 w-3" />
                  ) : (
                    <TrendingDown className="h-3 w-3" />
                  )}
                  {Math.abs((value || 0) - (prevValue || 0)).toLocaleString()}(
                  {isIncrease ? '+' : '-'}
                  {Math.abs(Math.round(((value - prevValue) / (prevValue || 1)) * 100))}%)
                </div>
              </div>
            )}
          </div>
        </div>
      );
    }
    return null;
  };

  const RevenueTooltipContent = ({ active, payload, label }: any) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      const value = data.value;
      const prevValue = formattedRevenueData[payload[0].index - 1]?.value || 0;
      const isIncrease = value > prevValue;

      return (
        <div className="rounded-lg border bg-background p-3 shadow-md">
          <div className="flex items-center gap-2 mb-2">
            <DollarSign className="h-4 w-4 text-orange-500" />
            <span className="font-semibold">Doanh thu - Tháng {label}</span>
          </div>
          <div className="grid gap-2">
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Tổng doanh thu: </span>
              <span className="font-bold text-sm">{(value || 0).toLocaleString('vi-VN')} VND</span>
            </div>
            <div className="text-xs text-muted-foreground">
              {(value || 0) >= 1000000
                ? `${((value || 0) / 1000000).toFixed(2)}M VND`
                : `${((value || 0) / 1000).toFixed(0)}K VND`}
            </div>
            {payload[0].index > 0 && (
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">So với tháng trước:</span>
                <div
                  className={cn(
                    'flex items-center gap-1 text-sm font-medium',
                    isIncrease ? 'text-green-500' : 'text-red-500',
                  )}
                >
                  {isIncrease ? (
                    <TrendingUp className="h-3 w-3" />
                  ) : (
                    <TrendingDown className="h-3 w-3" />
                  )}
                  {Math.abs(Math.round(((value - prevValue) / (prevValue || 1)) * 100))}%
                </div>
              </div>
            )}
          </div>
        </div>
      );
    }
    return null;
  };

  return (
    <div className="space-y-6">
      {/* Charts Grid */}
      <div className="grid gap-6 md:grid-cols-2">
        {/* Orders Chart */}
        <Card className="w-full">
          <CardHeader className="flex flex-col sm:flex-row items-start sm:items-center justify-between space-y-2 sm:space-y-0">
            <div className="flex-1 min-w-0">
              <CardTitle className="text-lg truncate">Biểu đồ đơn hàng</CardTitle>
              <p className="text-xs text-muted-foreground mt-1 truncate">
                Thống kê số lượng đơn hàng theo ngày
              </p>
            </div>
            <div className="flex gap-2 w-full sm:w-auto flex-wrap sm:flex-nowrap">
              <Select value={selectedMonth} onValueChange={setSelectedMonth}>
                <SelectTrigger className="w-full sm:w-[120px]">
                  <SelectValue placeholder="Tháng" />
                </SelectTrigger>
                <SelectContent>
                  {months.map((month) => (
                    <SelectItem key={month.id} value={month.id}>
                      {month.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <Select value={selectedWeek} onValueChange={setSelectedWeek}>
                <SelectTrigger className="w-full sm:w-[120px]">
                  <SelectValue placeholder="Tuần" />
                </SelectTrigger>
                <SelectContent>
                  {weeks.map((week) => (
                    <SelectItem key={week.id} value={week.id}>
                      {week.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </CardHeader>
          <CardContent>
            <div className="h-[250px] sm:h-[300px]">
              {isLoadingOrderStatistics ? (
                <div className="flex h-full items-center justify-center text-sm">
                  Đang tải dữ liệu...
                </div>
              ) : isErrorOrderStatistics ? (
                <div className="flex h-full items-center justify-center text-sm text-red-600">
                  Lỗi: {errorOrderStatistics?.message || 'Không thể tải dữ liệu'}
                </div>
              ) : formattedSalesData.length === 0 ? (
                <div className="flex h-full items-center justify-center text-sm">
                  Không có dữ liệu
                </div>
              ) : (
                <ChartContainer
                  config={{
                    value: {
                      label: 'Đơn hàng',
                      color: 'hsl(24, 92%, 53%)',
                    },
                  }}
                  className="h-full w-full"
                >
                  <LineChart
                    width={532}
                    height={300}
                    data={formattedSalesData}
                    margin={{ top: 10, right: 10, left: 0, bottom: 0 }}
                  >
                    <XAxis
                      dataKey="name"
                      stroke="#888888"
                      fontSize={12}
                      tickLine={false}
                      axisLine={false}
                    />
                    <YAxis
                      stroke="#888888"
                      fontSize={12}
                      tickLine={false}
                      axisLine={false}
                      tickFormatter={(value) => (value || 0).toLocaleString()}
                    />
                    <ChartTooltip
                      content={<OrdersTooltipContent />}
                      cursor={{ stroke: '#f97316', strokeWidth: 1, strokeDasharray: '4 4' }}
                    />
                    <Line
                      type="monotone"
                      dataKey="value"
                      stroke="var(--color-value)"
                      strokeWidth={2}
                      dot={{ r: 4, fill: '#f97316', stroke: '#f97316' }}
                      activeDot={{ r: 6, fill: '#f97316', stroke: '#fff', strokeWidth: 2 }}
                    />
                  </LineChart>
                </ChartContainer>
              )}
            </div>
            <div className="mt-3 space-y-1 text-xs text-muted-foreground">
              {orderStatistics?.highestOrderInWeek && (
                <p>
                  Đơn hàng cao nhất tuần:{' '}
                  <span className="font-medium">
                    {(orderStatistics.highestOrderInWeek || 0).toLocaleString()}
                  </span>
                </p>
              )}
              <p>
                Hiển thị dữ liệu: Tuần {selectedWeek}, Tháng {selectedMonth}, {selectedYear}
              </p>
            </div>
          </CardContent>
        </Card>

        {/* Revenue Chart */}
        <Card className="w-full">
          <CardHeader className="flex flex-col sm:flex-row items-start sm:items-center justify-between space-y-2 sm:space-y-0">
            <div className="flex-1 min-w-0">
              <CardTitle className="text-lg truncate">Báo cáo hiệu suất bán hàng</CardTitle>
              <p className="text-sm text-muted-foreground mt-1 truncate">
                Tổng: {(totalRevenue || 0).toLocaleString('vi-VN')} VND
              </p>
            </div>
            <Select value={categoryFilter} onValueChange={setCategoryFilter}>
              <SelectTrigger className="w-full sm:w-[180px]">
                <SelectValue placeholder="Danh mục" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả danh mục</SelectItem>
                {categories.map((category) => (
                  <SelectItem key={category.categoryId} value={category.categoryId}>
                    {category.categoryName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </CardHeader>
          <CardContent>
            <div className="h-[250px] sm:h-[300px]">
              {isLoadingRevenueReport ? (
                <div className="flex h-full items-center justify-center text-sm">
                  Đang tải dữ liệu...
                </div>
              ) : isErrorRevenueReport ? (
                <div className="flex h-full items-center justify-center text-sm text-red-600">
                  Lỗi: {errorRevenueReport?.message || 'Không thể tải dữ liệu'}
                </div>
              ) : formattedRevenueData.length === 0 ? (
                <div className="flex h-full items-center justify-center text-sm">
                  Không có dữ liệu
                </div>
              ) : (
                <ChartContainer
                  config={{
                    value: {
                      label: 'Doanh thu',
                      color: 'hsl(24, 92%, 53%)',
                    },
                  }}
                  className="h-full w-full"
                >
                  <BarChart
                    width={532}
                    height={300}
                    data={formattedRevenueData}
                    margin={{ top: 10, right: 10, left: 0, bottom: 0 }}
                  >
                    <XAxis
                      dataKey="name"
                      stroke="#888888"
                      fontSize={12}
                      tickLine={false}
                      axisLine={false}
                    />
                    <YAxis
                      stroke="#888888"
                      fontSize={12}
                      tickLine={false}
                      axisLine={false}
                      tickFormatter={(value) =>
                        (value || 0) >= 1000000
                          ? `${((value || 0) / 1000000).toFixed(1)}M`
                          : `${((value || 0) / 1000).toFixed(0)}K`
                      }
                    />
                    <ChartTooltip
                      content={<RevenueTooltipContent />}
                      cursor={{ fill: 'rgba(249, 115, 22, 0.1)' }}
                    />
                    <Bar
                      dataKey="value"
                      fill="var(--color-value)"
                      radius={[4, 4, 0, 0]}
                      barSize={30}
                    />
                  </BarChart>
                </ChartContainer>
              )}
            </div>
            <div className="mt-3 space-y-1 text-xs text-muted-foreground">
              {revenueReport?.leadingMonth && <p>Tháng cao nhất: {revenueReport.leadingMonth}</p>}
              <p>
                Hiển thị dữ liệu năm {selectedYear}
                {categoryFilter !== 'all' ? ` - Danh mục đã chọn` : ''}
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
