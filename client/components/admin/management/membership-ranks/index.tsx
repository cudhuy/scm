'use client';

import { motion } from 'framer-motion';
import { useSession } from 'next-auth/react';
import { useQuery } from '@tanstack/react-query';
import { Award, Crown, LucideIcon, Medal, Star } from 'lucide-react';

import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { membershipRankService } from '@/services/membershipRankService';
import {
  MembershipRank,
  MembershipRankCount,
  UpdateMembershipRankDto,
} from '@/types/membership-rank';
import AlertCustom from '@/components/global/alert-custom';
import MembershipRankSkeleton from './rank-skeleton';
import { MembershipRankList } from './rank-list';
import { EmptyState } from '../../common/empty-state';

// Constants
interface MembershipRankStatConfig {
  title: string;
  icon: LucideIcon;
  gradient: string;
  rankName?: string;
  count?: number;
}

const MEMBERSHIP_RANK_STATS_CONFIG: MembershipRankStatConfig[] = [
  {
    title: 'Tổng Số Thành Viên',
    icon: Crown,
    gradient: 'from-purple-600 to-indigo-600',
  },
  {
    title: 'Hạng STARS',
    icon: Star,
    gradient: 'from-amber-600 to-amber-400',
    rankName: 'STARS',
  },
  {
    title: 'Hạng VIPS',
    icon: Award,
    gradient: 'from-purple-600 to-purple-400',
    rankName: 'VIPS',
  },
  {
    title: 'Hạng REGULARS',
    icon: Medal,
    gradient: 'from-green-600 to-green-400',
    rankName: 'REGULARS',
  },
];

const mapStatsWithCounts = (stats: MembershipRankStatConfig[], counts?: MembershipRankCount) => {
  return stats.map((stat) => ({
    ...stat,
    count:
      stat.rankName === 'STARS'
        ? counts?.totalCustomerStar
        : stat.rankName === 'VIPS'
        ? counts?.totalCustomerVip
        : stat.rankName === 'REGULARS'
        ? counts?.totalCustomerRankRegular
        : counts?.totalCustomer,
  }));
};

// Main Component
export default function MembershipRankManagement() {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const {
    data: ranks,
    error,
    isLoading,
  } = useQuery<MembershipRank[]>({
    queryKey: ['membership-ranks'],
    queryFn: () => membershipRankService.getAllRanks(accessToken),
    enabled: !!accessToken,
  });

  const { data: memberCountsByRank } = useQuery<MembershipRankCount>({
    queryKey: ['memberCountsByRank'],
    queryFn: () => membershipRankService.getRanksCount(accessToken),
    enabled: !!ranks,
  });

  const updateRankMutation = useSimpleMutation<UpdateMembershipRankDto, void>({
    mutationKey: ['updateMembershipRank'],
    mutationFn: (rank) => membershipRankService.updateRank(rank.id, rank, accessToken),
    queryKey: ['membership-ranks'],
    successMessage: 'Membership rank updated successfully',
    errorMessage: 'Failed to update membership rank',
  });

  const deleteRankMutation = useSimpleMutation<string, void>({
    mutationKey: ['deleteMembershipRank'],
    mutationFn: (id) => membershipRankService.deleteRank(id, accessToken),
    queryKey: ['membership-ranks'],
    successMessage: 'Membership rank deleted successfully',
    errorMessage: 'Failed to delete membership rank',
  });

  const handleUpdateRank = (updatedRank: UpdateMembershipRankDto) => {
    updateRankMutation.mutate(updatedRank);
  };

  const handleDeleteRank = (rankId: string) => {
    deleteRankMutation.mutate(rankId);
  };

  const rankStats = mapStatsWithCounts(MEMBERSHIP_RANK_STATS_CONFIG, memberCountsByRank);

  return (
    <div className="mx-auto p-6 lg:p-10 space-y-8 max-w-[1600px]">
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between"
      >
        <div className="space-y-1">
          <h1 className="text-lg sm:text-xl md:text-2xl lg:text-3xl font-bold text-[#5c3d2e] dark:text-[#e6ccb2]">
            Quản Lý Hạng Thành Viên
          </h1>
          <p className="text-muted-foreground text-sm sm:text-base">
            Tạo và quản lý các hạng thành viên và đặc quyền
          </p>
        </div>
      </motion.div>

      {isLoading ? (
        <MembershipRankSkeleton />
      ) : error ? (
        error.status === 404 ? (
          <EmptyState
            title="Không tìm thấy dữ liệu"
            description="Hiện tại chưa có thứ hạng thành viên nào trong hệ thống. Bạn có thể thêm thứ hạng thành viên hoặc liên hệ hỗ trợ nếu cần."
            className="mt-10"
          />
        ) : (
          <AlertCustom message={error.message} className="mt-6" />
        )
      ) : (
        <>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
            {rankStats.map((stat, index) => (
              <motion.div
                key={stat.title}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3, delay: index * 0.1 }}
                className="h-full"
              >
                <div className="flex h-full flex-col justify-between rounded-xl border border-border/50 bg-card p-4 shadow-xs transition-shadow hover:shadow-md">
                  <div className="flex items-start justify-between">
                    <div>
                      <h3 className="text-sm font-medium text-muted-foreground">{stat.title}</h3>
                      <p className="mt-1 text-2xl font-bold">{stat.count ?? 0}</p>
                    </div>
                    <div
                      className={`rounded-lg bg-linear-to-br p-2.5 text-white ${stat.gradient}`}
                    >
                      <stat.icon className="h-5 w-5" />
                    </div>
                  </div>
                </div>
              </motion.div>
            ))}
          </div>

          <MembershipRankList
            ranks={ranks || []}
            handleUpdateRank={handleUpdateRank}
            handleDeleteRank={handleDeleteRank}
            isUpdateRankPending={updateRankMutation.isPending}
            isDeleteRankPending={deleteRankMutation.isPending}
          />
        </>
      )}
    </div>
  );
}
