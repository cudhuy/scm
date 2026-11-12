'use client';

import { useState, useEffect, useMemo, useCallback } from 'react';
import { useQuery } from '@tanstack/react-query';

import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

import { StaffList } from '@/components/admin/management/staffs/staff-list';
import PaginationCustom from '@/components/global/pagination-custom';

import { staffService } from '@/services/staffService';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { CreateNewStaffDto, GetStaffsResponse, Staff } from '@/types/staff';
import AlertCustom from '@/components/global/alert-custom';
import SkeletonStaffTable from './skeleton-staff';
import { branchService } from '@/services/branchService';
import { Branch } from '@/types/branch';
import { useSession } from 'next-auth/react';
import { AddStaffDialog } from './add-staff-dialog';
import { EmptyState } from '../../common/empty-state';

export default function StaffManagement() {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;
  const roleName = session?.user.roleName;
  const userBranchId = session?.user.branchId;

  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [branchFilter, setBranchFilter] = useState<string>(
    roleName === 'Manager' ? userBranchId || '' : 'all',
  );
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');

  const effectiveBranchFilter = useMemo(
    () => (branchFilter === 'all' ? '' : branchFilter),
    [branchFilter],
  );

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
      setCurrentPage(1);
    }, 500);
    return () => clearTimeout(timeoutId);
  }, [searchTerm]);

  const {
    data: branches = [],
    error: branchesError,
    isLoading: isLoadingBranches,
  } = useQuery<Branch[]>({
    queryKey: ['branches'],
    queryFn: () => branchService.getAllBranches(accessToken),
    enabled: !!accessToken,
  });

  const {
    data: staffData,
    error,
    isLoading,
    isFetching,
  } = useQuery<GetStaffsResponse>({
    queryKey: [
      'staffs',
      {
        page: currentPage,
        search: debouncedSearchTerm,
        branch: effectiveBranchFilter,
      },
    ],
    queryFn: () =>
      staffService.getStaffs(
        currentPage,
        undefined,
        debouncedSearchTerm,
        effectiveBranchFilter,
        accessToken,
      ),
    enabled: !!accessToken,
  });

  const { mutate: addStaff, isPending: isCreatePending } = useSimpleMutation<
    CreateNewStaffDto,
    Staff
  >({
    mutationKey: ['addStaff'],
    mutationFn: (staff: CreateNewStaffDto) => staffService.createStaff(staff, accessToken),
    queryKey: ['staffs'],
    successMessage: 'Thêm nhân viên thành công',
  });

  // Add toggle staff status mutation
  const { mutate: toggleStaffStatus, isPending: isTogglePending } = useSimpleMutation<string, void>(
    {
      mutationKey: ['toggleStaffStatus'],
      mutationFn: (staffId) => staffService.reactiveStaff(staffId, accessToken),
      queryKey: ['staffs'],
      successMessage: 'Trạng thái nhân viên đã được cập nhật',
    },
  );

  // Handle toggle staff active/inactive status
  const handleToggleStaffStatus = useCallback(
    (staffId: string) => {
      toggleStaffStatus(staffId);
    },
    [toggleStaffStatus],
  );

  // Chi nhánh của quản lý
  const managerBranch = useMemo(
    () => branches.find((branch) => branch.id === userBranchId),
    [branches, userBranchId],
  );

  // Xử lý sự kiện
  const handleAddStaff = (newStaff: CreateNewStaffDto) => {
    addStaff(newStaff);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleBranchFilterChange = (value: string) => {
    setBranchFilter(value);
    setCurrentPage(1);
  };

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const isSearching = isFetching && (debouncedSearchTerm !== '' || branchFilter !== 'all');
  const totalPages = staffData?.totalPages || 1;
  const pageSize = staffData?.pageSize || 10;
  const totalItems = staffData?.totalItems || 0;

  return (
    <div className="mx-auto p-4 md:p-6 lg:p-10 max-w-(--breakpoint-xl) space-y-4">
      <h1 className="text-xl sm:text-2xl md:text-3xl font-bold text-[#5c3d2e] dark:text-[#e6ccb2]">
        Quản lý nhân viên
      </h1>

      {/* Thanh tìm kiếm và bộ lọc */}
      <div className="flex flex-col sm:flex-row justify-between sm:items-center gap-3 sm:gap-2 w-full">
        <div className="flex flex-col xs:flex-row space-y-2 xs:space-y-0 xs:space-x-2 w-full sm:w-auto">
          <Input
            placeholder="Tìm kiếm theo tên..."
            className="w-full xs:w-auto lg:min-w-56 md:min-w-40"
            value={searchTerm}
            onChange={handleSearch}
          />
          <Select
            value={branchFilter}
            onValueChange={handleBranchFilterChange}
            disabled={roleName === 'Quản lý'}
          >
            <SelectTrigger className="w-full xs:w-[180px] md:max-w-[160px]">
              <SelectValue placeholder="Lọc theo chi nhánh" />
            </SelectTrigger>
            <SelectContent>
              {roleName === 'Admin' && <SelectItem value="all">Tất cả chi nhánh</SelectItem>}

              {isLoadingBranches ? (
                <SelectItem value="loading" disabled>
                  Đang tải chi nhánh...
                </SelectItem>
              ) : branchesError ? (
                <SelectItem value="error" disabled>
                  Lỗi khi tải chi nhánh
                </SelectItem>
              ) : roleName === 'Quản lý' && managerBranch ? (
                <SelectItem value={managerBranch.id}>{managerBranch.name}</SelectItem>
              ) : (
                branches.map((branch) => (
                  <SelectItem key={branch.id} value={branch.id}>
                    {branch.name}
                  </SelectItem>
                ))
              )}
            </SelectContent>
          </Select>
        </div>

        <AddStaffDialog
          isCreatePending={isCreatePending}
          isLoadingBranches={isLoadingBranches}
          branchesError={branchesError}
          onCreate={handleAddStaff}
          branches={branches}
        />
      </div>

      {/* Nội dung bảng */}
      {isLoading ? (
        <SkeletonStaffTable />
      ) : error ? (
        error.status === 404 ? (
          <EmptyState
            title="Không tìm thấy dữ liệu"
            description="Hiện tại chưa có chi nhánh nào trong hệ thống. Bạn có thể thêm chi nhánh mới hoặc liên hệ hỗ trợ nếu cần."
            className="mt-10"
          />
        ) : (
          <AlertCustom message={error.message} className="mt-6" />
        )
      ) : (
        <>
          <StaffList
            staffs={staffData?.items || []}
            branches={branches}
            isLoadingBranches={isLoadingBranches}
            branchesError={branchesError}
            isSearching={isSearching}
            onToggleStatus={handleToggleStaffStatus}
            isTogglePending={isTogglePending}
          />

          <p className="text-sm text-muted-foreground">
            Hiển thị {totalItems === 0 ? 0 : (currentPage - 1) * pageSize + 1} -{' '}
            {Math.min(currentPage * pageSize, totalItems)} trong tổng số {totalItems} nhân viên
          </p>

          {totalPages > 1 && (
            <PaginationCustom
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={handlePageChange}
            />
          )}
        </>
      )}
    </div>
  );
}
