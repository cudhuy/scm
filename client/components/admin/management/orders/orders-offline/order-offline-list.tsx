/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { useMemo } from 'react';
import { Card, CardContent, CardFooter, CardHeader } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import type { Branch } from '@/types/branch';
import {
  orderAdminService,
  type OrderDetailDto,
  type OrderDto,
  type TableDto,
} from '@/services/orderAdminService';
import { OrderHeader } from '../order-header';
import { OrderTable } from '../orders-table';
import PaginationCustom from '@/components/global/pagination-custom';
import { TableList } from '../tables-inorder-offline/table-list';
import { useSession } from 'next-auth/react';
import { useQuery } from '@tanstack/react-query';
import { useOrderStore } from '@/stores/order-store';

type OrderOfflineListProps = {
  orders: OrderDto[];
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalItems: number;
  error?: Error | null;
  isError: boolean;
  isLoading: boolean;
  isSearching: boolean;
  branches: Branch[];
  branchError?: Error | null;
  isLoadingBranches?: boolean;
  orderDetails: OrderDetailDto;
  isLoadingDetails?: boolean;
  detailsError?: any;
  isAdmin?: boolean;
  userBranchName?: string;
};

export function OrderOfflineList({
  orders,
  currentPage,
  totalPages,
  pageSize,
  totalItems,
  error,
  isError,
  isLoading,
  isSearching,
  branches,
  branchError,
  isLoadingBranches,
  orderDetails,
  isLoadingDetails,
  detailsError,
  isAdmin = false,
  userBranchName,
}: OrderOfflineListProps) {
  // Get state and actions from Zustand store
  const {
    view,
    setView,
    branchFilter,
    tableBranchFilter,
    handlePageChange,
    handleViewDetail,
    isDetailDialogOpen,
    setIsDetailDialogOpen,
    tableCurrentPage,
    tableSearchQuery,
    tableListStatusFilter,
  } = useOrderStore();

  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  // Use effectiveTableStatus to determine filter value for table list view
  const effectiveTableListStatusFilter = useMemo(() => {
    return tableListStatusFilter === 'all' ? undefined : tableListStatusFilter === 'true';
  }, [tableListStatusFilter]);

  // Modified query - use branchFilter when in orders view, and tableBranchFilter when in tables view
  const {
    data: tablesData = [],
    isLoading: isLoadingTables,
    error: tablesError,
  } = useQuery<TableDto[]>({
    queryKey: [
      'tables',
      // Use the appropriate branch filter based on current view
      view === 'orders' ? branchFilter : tableBranchFilter,
      effectiveTableListStatusFilter,
      tableSearchQuery,
      tableCurrentPage,
    ],
    queryFn: () =>
      orderAdminService.getAllTablesInStore(
        // Use the appropriate branch filter based on current view
        view === 'orders' ? branchFilter : tableBranchFilter,
        effectiveTableListStatusFilter,
        tableSearchQuery || undefined,
        accessToken,
      ),
    enabled: !!accessToken,
  });

  if (view === 'tables') {
    return (
      <TableList
        branches={branches}
        branchError={branchError}
        isAdmin={isAdmin}
        userBranchName={userBranchName}
        tablesData={tablesData}
        isLoadingTables={isLoadingTables}
        tablesError={tablesError}
      />
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center mb-4">
          <Button
            variant="ghost"
            onClick={() => {
              setView('tables');
            }}
            className="mr-2"
          >
            <ArrowLeft className="h-4 w-4 mr-1" />
            Quay lại
          </Button>
        </div>
        <OrderHeader
          title="Danh Sách Đơn Hàng Offline"
          description="Xem tất cả đơn hàng offline theo từng chi nhánh"
          type="order-offline"
          branches={branches}
          branchError={branchError}
          isLoadingBranches={isLoadingBranches}
          tableData={tablesData}
          isAdmin={isAdmin}
          userBranchName={userBranchName}
        />
      </CardHeader>
      <CardContent className="space-y-4">
        <OrderTable
          orders={orders}
          type="order-offline"
          error={error}
          isError={isError}
          isLoading={isLoading}
          isSearching={isSearching}
          orderDetails={orderDetails}
          isLoadingDetails={isLoadingDetails}
          detailsError={detailsError}
          onViewDetail={handleViewDetail}
          isDetailDialogOpen={isDetailDialogOpen}
          setIsDetailDialogOpen={setIsDetailDialogOpen}
        />
        {totalPages > 1 && (
          <PaginationCustom
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        )}
      </CardContent>
      <CardFooter>
        <div className="text-sm text-muted-foreground">
          Hiển thị {Math.min(pageSize, orders.length)} trong số {totalItems} đơn hàng
        </div>
      </CardFooter>
    </Card>
  );
}
