'use client';

import { motion } from 'framer-motion';
import { Store } from 'lucide-react';
import type React from 'react';
import { Branch, UpdateBranchDto } from '@/types/branch';
import { EditBranchDialog } from './edit-branch-dialog';

// Animation variants
const containerVariants = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1,
    },
  },
};

const cardVariants = {
  hidden: { opacity: 0, y: 20 },
  show: { opacity: 1, y: 0 },
};

interface BranchListProps {
  branches: Branch[];
  handleUpdateBranch: (updatedBranch: UpdateBranchDto) => void;
  isUpdateBranchPending?: boolean;
}

export function BranchList({
  branches,
  handleUpdateBranch,
  isUpdateBranchPending = false,
}: BranchListProps) {
  const cardStyles = {
    gradient: 'bg-linear-to-br from-blue-50 to-indigo-50 dark:from-gray-900 dark:to-gray-800',
    border: 'border border-gray-200/50 dark:border-gray-800/50',
    iconColor: 'text-indigo-600',
  };

  return (
    <motion.div
      variants={containerVariants}
      initial="hidden"
      animate="show"
      className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3"
    >
      {branches.map((branch) => (
        <motion.div
          key={branch.id}
          variants={cardVariants}
          transition={{ duration: 0.3 }}
          whileHover={{ scale: 1.02 }}
          className="group flex flex-col"
        >
          <div
            className={`relative overflow-hidden rounded-2xl ${cardStyles.gradient} ${cardStyles.border} p-6 flex flex-col flex-1 min-h-[300px]`}
          >
            {/* Decorative elements */}
            <div className="absolute -mt-12 -mr-12 h-24 w-24 rounded-full bg-white/5 right-0 top-0" />
            <div className="absolute -mb-8 -ml-8 h-16 w-16 rounded-full bg-white/5 bottom-0 left-0" />

            <div className="relative flex flex-col flex-1">
              <div
                className={`${cardStyles.iconColor} bg-white dark:bg-gray-900 p-3 rounded-xl inline-flex shadow-xs mb-4`}
              >
                <Store className="h-6 w-6" />
              </div>
              <div className="mb-6">
                <h3 className="mb-1 text-xl font-bold">{branch.name}</h3>
                <p className="text-sm text-muted-foreground">
                  {branch.address || 'Chưa cung cấp địa chỉ'}
                </p>
                <p className="text-sm text-muted-foreground mt-1">
                  Số điện thoại: {branch.phone || 'Chưa cung cấp số điện thoại'}
                </p>
                <p className="text-sm text-muted-foreground mt-1">
                  Email: {branch.email || 'Chưa cung cấp email'}
                </p>
                <p className="text-sm text-muted-foreground mt-1">
                  Loại: {branch.type || 'Chưa xác định'}
                </p>
              </div>
              <div className="flex justify-end gap-2 mt-auto">
                <EditBranchDialog
                  branch={branch}
                  onUpdate={handleUpdateBranch}
                  isUpdatePending={isUpdateBranchPending}
                />
              </div>
            </div>
          </div>
        </motion.div>
      ))}
    </motion.div>
  );
}
