'use client';

import { motion } from 'framer-motion';
import type React from 'react';
import type { MembershipRank, UpdateMembershipRankDto } from '@/types/membership-rank';
import { DeleteDialog } from '@/components/admin/common/delete-dialog';
import {
  DEFAULT_MEMBERSHIP_RANK_STYLES,
  MEMBERSHIP_RANK_STYLES_MAP,
} from '@/constants/membership-rank-styles';
import { EditMembershipRankDialog } from './edit-rank-dialog';

const container = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1,
    },
  },
};

interface MembershipRankListProps {
  ranks: MembershipRank[];
  handleUpdateRank: (updatedRank: UpdateMembershipRankDto) => void;
  handleDeleteRank: (rankId: string) => void;
  isUpdateRankPending?: boolean;
  isDeleteRankPending?: boolean;
}

export function MembershipRankList({
  ranks,
  handleUpdateRank,
  handleDeleteRank,
  isUpdateRankPending = false,
  isDeleteRankPending = false,
}: MembershipRankListProps) {
  return (
    <motion.div
      className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3"
      variants={container}
      initial="hidden"
      animate="show"
    >
      {ranks.map((rank) => {
        const {
          icon: RankIcon,
          color,
          bgGradient,
        } = MEMBERSHIP_RANK_STYLES_MAP[rank.name] || DEFAULT_MEMBERSHIP_RANK_STYLES;

        return (
          <motion.div
            key={rank.id}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
            whileHover={{ scale: 1.02 }}
            className="group flex flex-col"
          >
            <div
              className={`relative rounded-2xl overflow-hidden ${bgGradient} border border-gray-200/50 dark:border-gray-800/50 p-6 flex flex-col flex-1 min-h-[200px]`}
            >
              <div className="absolute top-0 right-0 w-24 h-24 rounded-full bg-white/5 -mt-12 -mr-12"></div>
              <div className="absolute bottom-0 left-0 w-16 h-16 rounded-full bg-white/5 -mb-8 -ml-8"></div>

              <div className="relative flex flex-col flex-1">
                <div
                  className={`${color} bg-white dark:bg-gray-900 p-3 rounded-xl inline-flex max-w-12 shadow-xs mb-4`}
                >
                  <RankIcon className="h-6 w-6" />
                </div>

                <div className="mb-6">
                  <h3 className="text-xl font-bold mb-1">{rank.name}</h3>
                  <p className="text-sm text-white/70">
                    {rank.description || 'Chưa cung cấp mô tả'}
                  </p>
                </div>

                <div className="flex justify-end gap-2 mt-auto">
                  <EditMembershipRankDialog
                    rank={rank}
                    onUpdate={handleUpdateRank}
                    isUpdatePending={isUpdateRankPending}
                  />
                  <DeleteDialog
                    entityId={rank.id}
                    entityName={rank.name}
                    onDelete={handleDeleteRank}
                    isDeletePending={isDeleteRankPending}
                  />
                </div>
              </div>
            </div>
          </motion.div>
        );
      })}
    </motion.div>
  );
}
