'use client';

import { motion } from 'framer-motion';
import { BranchList } from '@/components/admin/management/branches/branch-list';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { branchService } from '@/services/branchService';
import { Branch, CreateNewBranchDto, UpdateBranchDto } from '@/types/branch';
import { useQuery } from '@tanstack/react-query';
import { AddBranchDialog } from '@/components/admin/management/branches/add-branch-dialog';
import AlertCustom from '@/components/global/alert-custom';
import BranchSkeleton from '../roles/role-skeleton';
import { useSession } from 'next-auth/react';
import { EmptyState } from '../../common/empty-state';

export default function BranchManagement() {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const {
    data: branches,
    error,
    isLoading,
  } = useQuery<Branch[]>({
    queryKey: ['branches'],
    queryFn: () => branchService.getAllBranches(accessToken),
  });

  const addBranchMutation = useSimpleMutation<CreateNewBranchDto, Branch>({
    mutationKey: ['addBranch'],
    mutationFn: (branch) => branchService.createBranch(branch, accessToken),
    queryKey: ['branches'],
  });

  const updateBranchMutation = useSimpleMutation<UpdateBranchDto, void>({
    mutationKey: ['updateBranch'],
    mutationFn: (branch) => branchService.updateBranch(branch.id, branch, accessToken),
    queryKey: ['branches'],
  });

  const handleAddBranch = (newBranch: CreateNewBranchDto) => {
    addBranchMutation.mutate(newBranch);
  };

  const handleUpdateBranch = (updatedBranch: UpdateBranchDto) => {
    updateBranchMutation.mutate(updatedBranch);
  };

  return (
    <div className="mx-auto p-6 lg:p-10 space-y-8 max-w-[1600px]">
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="flex flex-col md:flex-row md:items-center md:justify-between gap-4"
      >
        <div className="space-y-1">
          <h1 className="text-lg sm:text-xl md:text-2xl lg:text-3xl font-bold text-[#5c3d2e] dark:text-[#e6ccb2]">
            Quản Lý Chi Nhánh
          </h1>
          <p className="text-muted-foreground text-sm sm:text-base">
            Tạo và quản lý thông tin các chi nhánh
          </p>
        </div>
        <AddBranchDialog onCreate={handleAddBranch} isCreatePending={addBranchMutation.isPending} />
      </motion.div>
      {isLoading ? (
        <BranchSkeleton />
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
        <BranchList
          branches={branches || []}
          handleUpdateBranch={handleUpdateBranch}
          isUpdateBranchPending={updateBranchMutation.isPending}
        />
      )}
    </div>
  );
}
