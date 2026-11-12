'use client';

import React, { useCallback } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Staff, UpdateStaffDto } from '@/types/staff';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { staffService } from '@/services/staffService';
import { Badge } from '@/components/ui/badge';
import { EditStaffDialog } from './edit-staff-dialog';
import { Branch } from '@/types/branch';
import { ViewStaffDialog } from './view-staff-dialog';
import { useSession } from 'next-auth/react';
import { Lock } from 'lucide-react';
import { ROLE_STYLES_MAP, DEFAULT_ROLE_STYLES } from '@/constants/role-styles';
import { CustomToggle } from '@/components/ui/custom-toggle';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Button } from '@/components/ui/button';

interface StaffListProps {
  staffs: Staff[];
  branches: Branch[];
  isLoadingBranches?: boolean;
  branchesError?: Error | null;
  isSearching: boolean;
  onToggleStatus: (staffId: string) => void;
  isTogglePending: boolean;
}

export function StaffList({
  staffs,
  branches,
  isLoadingBranches = false,
  branchesError,
  isSearching,
  onToggleStatus,
  isTogglePending,
}: StaffListProps) {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const { mutate: updateStaff, isPending: isUpdatePending } = useSimpleMutation<
    { staff: UpdateStaffDto },
    void
  >({
    mutationKey: ['updateStaff'],
    mutationFn: ({ staff }) => staffService.updateStaff(staff, accessToken),
    queryKey: ['staffs'],
    successMessage: 'Cập nhật nhân viên thành công',
  });

  const handleUpdateStaff = useCallback(
    (updatedStaff: UpdateStaffDto) => {
      if (updatedStaff.userId) {
        updateStaff({ staff: updatedStaff });
      } else {
        console.error('ID nhân viên không xác định');
      }
    },
    [updateStaff],
  );

  const handleToggleStatus = (staffId: string) => {
    onToggleStatus(staffId);
  };

  const currentColSpan = 7;

  return (
    <div className="rounded-md border overflow-x-auto">
      <Table className="min-w-full">
        <TableHeader>
          <TableRow>
            <TableHead className="w-[20%] font-semibold">Họ tên</TableHead>
            <TableHead className="w-[20%]">Thông tin liên hệ</TableHead>
            <TableHead className="w-[10%]">Vai trò</TableHead>
            <TableHead className="w-[15%] hidden md:table-cell">Chi nhánh</TableHead>
            <TableHead className="w-[15%] hidden lg:table-cell">Chức danh</TableHead>
            <TableHead className="w-[10%]">Trạng thái</TableHead>
            <TableHead className="w-[10%] text-right">Thao tác</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {staffs.length ? (
            staffs.map((staff) => (
              <TableRow
                key={staff.userId}
                className={`hover:bg-muted/50 ${
                  staff.isDeleted ? 'opacity-60 bg-gray-200 dark:bg-gray-900/60' : ''
                }`}
              >
                <TableCell className="font-medium py-3">
                  <span className="line-clamp-1">{staff.fullName}</span>
                </TableCell>
                <TableCell className="py-3">
                  <div className="flex flex-col">
                    <span className="line-clamp-1">{staff.email}</span>
                    <span className="text-sm text-muted-foreground mt-1">{staff.phone}</span>
                  </div>
                </TableCell>
                <TableCell className="py-3">
                  {staff.roleName && (
                    <Badge
                      variant="outline"
                      className={`${
                        ROLE_STYLES_MAP[staff.roleName]?.color || DEFAULT_ROLE_STYLES.color
                      } 
                                 bg-linear-to-r ${
                                   ROLE_STYLES_MAP[staff.roleName]?.bgGradient ||
                                   DEFAULT_ROLE_STYLES.bgGradient
                                 }`}
                    >
                      {staff.roleName}
                    </Badge>
                  )}
                </TableCell>
                <TableCell className="py-3 hidden md:table-cell">{staff.branchName}</TableCell>
                <TableCell className="py-3 hidden lg:table-cell" title={staff.description}>
                  <span className="line-clamp-2">{staff.description}</span>
                </TableCell>
                <TableCell className="py-3">
                  <Badge
                    variant={staff.isDeleted ? 'outline-solid' : 'default'}
                    className={
                      staff.isDeleted
                        ? 'text-red-500 border-red-200 dark:border-red-800/30'
                        : 'bg-green-100 text-green-800 hover:bg-green-100 dark:bg-green-900/30 dark:text-green-400'
                    }
                  >
                    {staff.isDeleted ? 'Không hoạt động' : 'Hoạt động'}
                  </Badge>
                </TableCell>
                <TableCell className="py-3">
                  <div className="flex justify-end items-center gap-2">
                    <div className="flex items-center">
                      <CustomToggle
                        checked={!staff.isDeleted}
                        onCheckedChange={() => handleToggleStatus(staff.userId)}
                        disabled={isTogglePending}
                      />
                    </div>

                    <ViewStaffDialog staff={staff} />

                    {staff.isDeleted ? (
                      <TooltipProvider>
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button
                              variant="outline"
                              size="sm"
                              title="Chỉnh sửa"
                              className="p-2 md:p-3 bg-gray-100 dark:bg-gray-800 rounded-md border border-gray-200 dark:border-gray-700 cursor-not-allowed "
                            >
                              <Lock className="h-3 w-3 text-black dark:text-gray-400" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>
                            <p>Kích hoạt nhân viên để chỉnh sửa</p>
                          </TooltipContent>
                        </Tooltip>
                      </TooltipProvider>
                    ) : (
                      <EditStaffDialog
                        branches={branches}
                        isLoadingBranches={isLoadingBranches}
                        branchesError={branchesError}
                        staff={staff}
                        isUpdatePending={isUpdatePending}
                        onUpdate={handleUpdateStaff}
                      />
                    )}
                  </div>
                </TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={currentColSpan} className="h-24 text-center">
                {isSearching ? 'Không tìm thấy kết quả phù hợp.' : 'Danh sách trống.'}
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}

export default StaffList;
