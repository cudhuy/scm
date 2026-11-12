/* eslint-disable @typescript-eslint/no-unused-vars */
// app/page.jsx
'use client';

import { useEffect, useState, useMemo } from 'react';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import ChartsSection from './charts-section';
import TabsSection from './tabs-section';
import { useSession } from 'next-auth/react';
import { categoryService } from '@/services/categoryService';
import { useQuery } from '@tanstack/react-query';
import { branchService } from '@/services/branchService';
import { dashboardService } from '@/services/dashboardService';
import { getCurrentMonths, getWeeksInMonth } from '@/lib/utils';
import StatsSection from './starts-section';
import { orderAdminService, OrderDetailDto } from '@/services/orderAdminService';
import { ProductDetail } from '@/types/product-admin';
import { productAdminService } from '@/services/productAdminService';
import { UserDetailDto } from '@/types/dashboard';

export default function DashboardManagement() {
  const [branchFilter, setBranchFilter] = useState<string>('all');
  const [categoryFilter, setCategoryFilter] = useState<string>('all');
  const [isDetailDialogOpen, setIsDetailDialogOpen] = useState(false);
  const [selectedYear, setSelectedYear] = useState('');
  const [selectedMonth, setSelectedMonth] = useState('');
  const [selectedWeek, setSelectedWeek] = useState('');
  const [activeTab, setActiveTab] = useState('recent-orders');

  const [availableYears, setAvailableYears] = useState<{ id: string; name: string }[]>([]);
  const [months, setMonths] = useState<{ id: string; name: string }[]>([]);
  const [weeks, setWeeks] = useState<{ id: string; name: string }[]>([]);

  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const { data: session } = useSession();
  const accessToken = session?.user?.accessToken;

  const effectiveBranchFilter = useMemo(
    () => (branchFilter === 'all' ? '' : branchFilter),
    [branchFilter],
  );

  const effectiveCategoryFilter = useMemo(
    () => (categoryFilter === 'all' ? '' : categoryFilter),
    [categoryFilter],
  );

  useEffect(() => {
    const now = new Date();
    const currentYear = now.getFullYear();
    const currentYearStr = currentYear.toString();
    const prevYearStr = (currentYear - 1).toString();
    const prevYear2Str = (currentYear - 2).toString();

    setAvailableYears([
      { id: prevYear2Str, name: prevYear2Str },
      { id: prevYearStr, name: prevYearStr },
      { id: currentYearStr, name: currentYearStr },
    ]);

    setSelectedYear(currentYearStr);

    const currentMonth = (now.getMonth() + 1).toString();
    setSelectedMonth(currentMonth);

    // Determine current week based on day of month
    const currentDay = now.getDate();
    let currentWeekId = '1';
    if (currentDay <= 7) currentWeekId = '1';
    else if (currentDay <= 14) currentWeekId = '2';
    else if (currentDay <= 21) currentWeekId = '3';
    else currentWeekId = '4';
    setSelectedWeek(currentWeekId);
  }, []);

  // Update months when year changes
  useEffect(() => {
    if (!selectedYear) return;

    const currentMonths = getCurrentMonths(selectedYear);
    setMonths(currentMonths);

    if (currentMonths.length === 0) {
      setSelectedMonth('');
      setWeeks([]);
      setSelectedWeek('');
    } else if (!selectedMonth || !currentMonths.some((m) => m.id === selectedMonth)) {
      setSelectedMonth(currentMonths[currentMonths.length - 1].id);
    }
  }, [selectedYear, selectedMonth]);

  // Update weeks when month changes
  useEffect(() => {
    if (selectedMonth && selectedYear) {
      const weeksForMonth = getWeeksInMonth(selectedMonth, selectedYear);
      setWeeks(weeksForMonth);

      if (weeksForMonth.length > 0) {
        const now = new Date();
        const currentYear = now.getFullYear().toString();
        const currentMonth = (now.getMonth() + 1).toString();

        if (selectedYear === currentYear && selectedMonth === currentMonth) {
          const currentDay = now.getDate();
          let currentWeekId = '1';
          if (currentDay <= 7) currentWeekId = '1';
          else if (currentDay <= 14) currentWeekId = '2';
          else if (currentDay <= 21) currentWeekId = '3';
          else currentWeekId = '4';
          setSelectedWeek(
            weeksForMonth.some((w) => w.id === currentWeekId) ? currentWeekId : weeksForMonth[0].id,
          );
        } else {
          setSelectedWeek(weeksForMonth[0].id);
        }
      } else {
        setSelectedWeek('');
      }
    }
  }, [selectedMonth, selectedYear]);

  // Fetch categories data
  const {
    data: categories = [],
    error: errorCategories,
    isError: isErrorCategories,
    isLoading: isLoadingCategories,
  } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoryService.getAllCategories(accessToken),
    enabled: !!accessToken,
  });

  const {
    data: reportSummary,
    error: errorReportSummary,
    isError: isErrorReportSummary,
    isLoading: isLoadingReportSummary,
  } = useQuery({
    queryKey: ['reportSummary', effectiveBranchFilter],
    queryFn: () => dashboardService.getReportSummary(effectiveBranchFilter, accessToken),
    enabled: !!accessToken,
  });

  const {
    data: branches = [],
    error: errorBranches,
    isError: isErrorBranches,
    isLoading: isLoadingBranches,
  } = useQuery({
    queryKey: ['branches'],
    queryFn: () => branchService.getAllBranches(accessToken),
    enabled: !!accessToken,
  });

  const {
    data: revenueReport,
    error: errorRevenueReport,
    isError: isErrorRevenueReport,
    isLoading: isLoadingRevenueReport,
  } = useQuery({
    queryKey: ['revenueReport', selectedYear, effectiveBranchFilter, effectiveCategoryFilter],

    queryFn: () =>
      dashboardService.getRevenueReport(
        Number(selectedYear),
        effectiveBranchFilter,
        effectiveCategoryFilter,
        accessToken,
      ),
    enabled: !!accessToken && !!selectedYear,
  });

  const {
    data: orderStatistics,
    error: errorOrderStatistics,
    isError: isErrorOrderStatistics,
    isLoading: isLoadingOrderStatistics,
  } = useQuery({
    queryKey: ['orderStatistics', effectiveBranchFilter, selectedWeek, selectedMonth, selectedYear],
    queryFn: () =>
      dashboardService.getOrderStatistics(
        effectiveBranchFilter,
        Number(selectedWeek),
        Number(selectedMonth),
        Number(selectedYear),
        accessToken,
      ),
    enabled: !!accessToken && !!selectedYear && !!selectedMonth && !!selectedWeek,
  });

  const {
    data: latestOrders = [],
    error: errorLatestOrders,
    isError: isErrorLatestOrders,
    isLoading: isLoadingLatestOrders,
  } = useQuery({
    queryKey: ['latestOrders', effectiveBranchFilter],
    queryFn: () => dashboardService.getLatestFiveOrders(effectiveBranchFilter, accessToken),
    enabled: !!accessToken && activeTab === 'recent-orders',
  });

  const {
    data: topSellingProducts = [],
    error: errorTopSellingProducts,
    isError: isErrorTopSellingProducts,
    isLoading: isLoadingTopSellingProducts,
  } = useQuery({
    queryKey: ['topSellingProducts', effectiveBranchFilter],
    queryFn: () => dashboardService.getTopSellingProducts(effectiveBranchFilter, accessToken),
    enabled: !!accessToken && activeTab === 'best-selling',
  });

  const {
    data: recentCustomers = [],
    error: errorRecentCustomers,
    isError: isErrorRecentCustomers,
    isLoading: isLoadingRecentCustomers,
  } = useQuery({
    queryKey: ['recentCustomers', effectiveBranchFilter],
    queryFn: () => dashboardService.getRecentCustomers(effectiveBranchFilter, accessToken),
    enabled: !!accessToken && activeTab === 'recent-customers',
  });

  const {
    data: orderDetails,
    isLoading: isLoadingOrderDetails,
    error: detailsErrorOrder,
  } = useQuery<OrderDetailDto | null>({
    queryKey: ['orderDetail', selectedOrderId],
    queryFn: () => orderAdminService.getOrderById(selectedOrderId || '', accessToken),
    enabled: !!selectedOrderId && !!accessToken && isDetailDialogOpen,
  });

  const {
    data: productDetails,
    isLoading: isLoadingProductDetail,
    error: productDetailError,
  } = useQuery<ProductDetail>({
    queryKey: ['productDetail', selectedProductId],
    queryFn: () => productAdminService.getProductDetail(selectedProductId || '', accessToken),
    enabled: !!selectedProductId && !!accessToken && isDetailDialogOpen,
  });

  const {
    data: userDetails,
    isLoading: isLoadingUserDetail,
    error: userDetailError,
  } = useQuery<UserDetailDto>({
    queryKey: ['userDetail', selectedUserId],
    queryFn: () => dashboardService.getDetailUser(selectedUserId || '', accessToken),
    enabled: !!selectedUserId && !!accessToken && isDetailDialogOpen,
  });

  const handleViewOrderDetail = (orderId: string) => {
    setSelectedOrderId(orderId);
    setIsDetailDialogOpen(true);
  };

  const handleViewProductDetail = (productId: string) => {
    setSelectedProductId(productId);
    setIsDetailDialogOpen(true);
  };

  const handleViewUserDetail = (userId: string) => {
    setSelectedUserId(userId);
    setIsDetailDialogOpen(true);
  };

  return (
    <div className="flex-1 space-y-4 p-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-3xl font-bold tracking-tight">Tổng quan</h2>
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium hidden md:block">Lọc theo chi nhánh và năm:</span>
          <Select value={branchFilter} onValueChange={setBranchFilter}>
            <SelectTrigger className="w-[200px]">
              <SelectValue placeholder="Chọn chi nhánh" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Tất cả chi nhánh</SelectItem>
              {branches?.map((branch) => (
                <SelectItem key={branch.id} value={branch.id}>
                  {branch.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select value={selectedYear} onValueChange={setSelectedYear}>
            <SelectTrigger className="w-[180px] sm:w-[200px]">
              <SelectValue placeholder="Chọn năm" />
            </SelectTrigger>
            <SelectContent>
              {availableYears.map((year) => (
                <SelectItem key={year.id} value={year.id}>
                  {year.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>
      <StatsSection
        reportSummary={reportSummary}
        isLoading={isLoadingReportSummary}
        isError={isErrorReportSummary}
        error={errorReportSummary}
      />
      <ChartsSection
        categories={categories}
        errorCategories={errorCategories}
        isErrorCategories={isErrorCategories}
        isLoadingCategories={isLoadingCategories}
        categoryFilter={categoryFilter}
        setCategoryFilter={setCategoryFilter}
        branchFilter={branchFilter}
        selectedYear={selectedYear}
        selectedMonth={selectedMonth}
        selectedWeek={selectedWeek}
        setSelectedYear={setSelectedYear}
        setSelectedMonth={setSelectedMonth}
        setSelectedWeek={setSelectedWeek}
        months={months}
        weeks={weeks}
        revenueReport={revenueReport}
        errorRevenueReport={errorRevenueReport}
        isErrorRevenueReport={isErrorRevenueReport}
        isLoadingRevenueReport={isLoadingRevenueReport}
        orderStatistics={orderStatistics}
        errorOrderStatistics={errorOrderStatistics}
        isErrorOrderStatistics={isErrorOrderStatistics}
        isLoadingOrderStatistics={isLoadingOrderStatistics}
      />
      <TabsSection
        latestOrders={latestOrders}
        isLoadingLatestOrders={isLoadingLatestOrders}
        isErrorLatestOrders={isErrorLatestOrders}
        errorLatestOrders={errorLatestOrders}
        topSellingProducts={topSellingProducts}
        isLoadingTopSellingProducts={isLoadingTopSellingProducts}
        isErrorTopSellingProducts={isErrorTopSellingProducts}
        errorTopSellingProducts={errorTopSellingProducts}
        recentCustomers={recentCustomers}
        isLoadingRecentCustomers={isLoadingRecentCustomers}
        isErrorRecentCustomers={isErrorRecentCustomers}
        errorRecentCustomers={errorRecentCustomers}
        activeTab={activeTab}
        setActiveTab={setActiveTab}
        isDetailDialogOpen={isDetailDialogOpen}
        setIsDetailDialogOpen={setIsDetailDialogOpen}
        orderDetails={orderDetails || ({} as OrderDetailDto)}
        isLoadingOrderDetails={isLoadingOrderDetails}
        detailsErrorOrder={detailsErrorOrder}
        productDetails={productDetails || ({} as ProductDetail)}
        isLoadingProductDetail={isLoadingProductDetail}
        productDetailError={productDetailError}
        userDetails={userDetails || ({} as UserDetailDto)}
        isLoadingUserDetail={isLoadingUserDetail}
        userDetailError={userDetailError}
        onViewUserDetail={handleViewUserDetail}
        onViewOrderDetail={handleViewOrderDetail}
        onViewProductDetail={handleViewProductDetail}
      />
    </div>
  );
}
