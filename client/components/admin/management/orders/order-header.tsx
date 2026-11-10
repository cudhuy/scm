import { CardDescription, CardTitle } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Input } from '@/components/ui/input';
import { Search } from 'lucide-react';
import { Branch } from '@/types/branch';
import { TableDto } from '@/services/orderAdminService';
import { useOrderStore } from '@/stores/order-store';

type OrderHeaderProps = {
  title: string;
  description: string;
  type?: 'order-online' | 'order-offline';
  branches: Branch[];
  branchError?: Error | null;
  isLoadingBranches?: boolean;
  tableData?: TableDto[];
  isAdmin?: boolean;
  userBranchName?: string;
};

export function OrderHeader({
  title,
  description,
  type,
  branches,
  branchError,
  isLoadingBranches,
  tableData,
  isAdmin = false,
  userBranchName,
}: OrderHeaderProps) {
  // Get state and actions from Zustand store
  const {
    searchTerm,
    branchFilter,
    statusFilter,
    tableNumber,
    handleBranchFilterChange,
    handleStatusFilterChange,
    handleTableNumberChange,
    handleSearch,
  } = useOrderStore();

  return (
    <div className="space-y-4">
      {/* Title and Description */}
      <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-2">
        <div>
          <CardTitle className="text-lg sm:text-xl">{title}</CardTitle>
          <CardDescription className="text-sm">{description}</CardDescription>
        </div>
      </div>

      {/* Filters */}
      <div className="flex flex-col gap-2 sm:flex-row sm:flex-wrap sm:gap-3">
        {isAdmin ? (
          <Select value={branchFilter} onValueChange={handleBranchFilterChange}>
            <SelectTrigger className="w-full sm:w-48 h-9 text-sm">
              <SelectValue placeholder="Chọn chi nhánh" />
            </SelectTrigger>
            <SelectContent>
              {isLoadingBranches ? (
                <SelectItem value="loading" disabled>
                  Đang tải chi nhánh...
                </SelectItem>
              ) : branchError ? (
                <SelectItem value="error" disabled>
                  Lỗi tải chi nhánh
                </SelectItem>
              ) : (
                <>
                  {branches.map((branch) => (
                    <SelectItem key={branch.id} value={branch.id}>
                      {branch.name}
                    </SelectItem>
                  ))}
                </>
              )}
            </SelectContent>
          </Select>
        ) : (
          <div className="text-sm font-medium flex items-center h-10 px-4 border rounded-md bg-background w-full sm:w-48">
            {userBranchName || 'Chi nhánh của bạn'}
          </div>
        )}

        {type === 'order-offline' ? (
          <Select value={tableNumber} onValueChange={handleTableNumberChange}>
            <SelectTrigger className="w-full sm:w-48 h-10">
              <SelectValue placeholder="Chọn bàn" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Tất cả bàn</SelectItem>
              {tableData?.map((table) => (
                <SelectItem key={table.tableId} value={table.tableId}>
                  Bàn {table.tableNumber}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        ) : (
          <Select
            value={statusFilter !== undefined ? String(statusFilter) : undefined}
            onValueChange={handleStatusFilterChange}
          >
            <SelectTrigger className="w-full sm:w-48 h-10">
              <SelectValue placeholder="Trạng thái đơn" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Tất cả</SelectItem>
              <SelectItem value="0">Chờ xác nhận</SelectItem>
              <SelectItem value="1">Đang chuyển</SelectItem>
              <SelectItem value="2">Hoàn thành</SelectItem>
              <SelectItem value="3">Hủy</SelectItem>
            </SelectContent>
          </Select>
        )}
      </div>

      {/* Search Bar */}
      <div className="relative">
        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Tìm kiếm đơn hàng..."
          className="pl-10 h-10 w-full max-w-md"
          value={searchTerm}
          onChange={handleSearch}
        />
      </div>
    </div>
  );
}
