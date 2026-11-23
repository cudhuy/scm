'use client';

import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Bell, BellRing, TrendingUp, Users } from 'lucide-react';
import { motion } from 'framer-motion';
import { AddNotificationDialog } from '@/components/admin/management/notifications/add-notification-dialog';
import { UserNotificationList } from '@/components/admin/management/notifications/user/user-notification-list';
import { SystemNotificationList } from '@/components/admin/management/notifications/system/system-notification-list';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { useQuery } from '@tanstack/react-query';
import {
  CreateSystemNotificationDto,
  CreateUserNotificationDto,
  Notification,
  NotificationCounts,
} from '@/types/notification';
import { notificationService } from '@/services/notificationService';
import { membershipRankService } from '@/services/membershipRankService';
import { MembershipRank } from '@/types/membership-rank';
import { SkeletonStats } from './skeleton-stats-notifications';
import { useSession } from 'next-auth/react';

const container = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1,
    },
  },
};

const item = {
  hidden: { opacity: 0, y: 20 },
  show: { opacity: 1, y: 0 },
};

export default function NotificationManagement() {
  const [activeTab, setActiveTab] = useState('user');
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const {
    data: ranks,
    isLoading: isLoadingRanks,
    error: ranksError,
  } = useQuery<MembershipRank[]>({
    queryKey: ['ranks'],
    queryFn: () => membershipRankService.getAllRanks(accessToken),
    enabled: !!accessToken,
  });

  const {
    data: notificationCounts,
    isLoading: isLoadingNotificationCounts,
    error: notificationCountsError,
  } = useQuery<NotificationCounts>({
    queryKey: ['notification-counts'],
    queryFn: () => notificationService.getNotificationCounts(accessToken),
    enabled: !!accessToken,
  });

  const addSystemNotificationMutation = useSimpleMutation<
    CreateSystemNotificationDto,
    Notification
  >({
    mutationKey: ['add-system-notification'],
    mutationFn: (data) => notificationService.createSystemNotification(data, accessToken),
    queryKey: ['notifications', 'system'],
    successMessage: 'Đã thêm thông báo hệ thống thành công',
  });

  const handleAddSystemNotification = (newNotification: CreateSystemNotificationDto) => {
    addSystemNotificationMutation.mutate(newNotification);
  };

  const addUserNotificationMutation = useSimpleMutation<CreateUserNotificationDto, Notification>({
    mutationKey: ['add-user-notification'],
    mutationFn: (data) => notificationService.createUserNotification(data, accessToken),
    queryKey: ['notifications', 'user'],
    successMessage: 'Đã thêm thông báo người dùng thành công',
  });

  const handleAddUserNotification = (newNotification: CreateUserNotificationDto) => {
    addUserNotificationMutation.mutate(newNotification);
  };

  const statsCards = [
    {
      title: 'Tổng số thông báo',
      value: notificationCounts?.totalNotificationsCount?.toLocaleString() || '0',
      icon: BellRing,
      description: 'Thông báo hoạt động',
      gradient: 'from-blue-600 to-blue-400',
    },
    {
      title: 'Thông báo người dùng',
      value: notificationCounts?.userNotificationsCount?.toLocaleString() || '0',
      icon: Users,
      description: 'Thông báo dựa trên thứ hạng',
      gradient: 'from-red-600 to-orange-600',
    },
    {
      title: 'Thông báo hệ thống',
      value: notificationCounts?.systemNotificationsCount?.toLocaleString() || '0',
      icon: Bell,
      description: 'Thông báo toàn hệ thống',
      gradient: 'from-pink-600 to-pink-700',
    },
    {
      title: 'Thông báo hôm nay',
      value: notificationCounts?.notificationsTodayCount?.toLocaleString() || '0',
      icon: TrendingUp,
      description: 'Đã gửi trong 24 giờ qua',
      gradient: 'from-lime-600 to-blue-700',
    },
  ];

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 md:px-8 py-6 md:py-10 space-y-4 md:space-y-8">
      <div className="flex flex-col sm:flex-row justify-between items-start gap-4 sm:items-center">
        <div className="space-y-1">
          <h1 className="text-lg sm:text-xl md:text-2xl lg:text-3xl font-bold text-[#5c3d2e] dark:text-[#e6ccb2]">
            Quản lý thông báo
          </h1>
          <p className="text-muted-foreground text-sm sm:text-base">
            Quản lý và gửi thông báo cho người dùng và thông báo trên toàn hệ thống
          </p>
        </div>
        <div className="w-full sm:w-auto mt-4 sm:mt-0">
          <AddNotificationDialog
            type={activeTab as 'user' | 'system'}
            ranks={ranks || []}
            isLoadingRanks={isLoadingRanks}
            ranksError={ranksError}
            onCreate={(data) => {
              if (activeTab === 'user') {
                handleAddUserNotification(data as CreateUserNotificationDto);
              } else {
                handleAddSystemNotification(data as CreateSystemNotificationDto);
              }
            }}
            isCreatePending={
              activeTab === 'user'
                ? addUserNotificationMutation.isPending
                : addSystemNotificationMutation.isPending
            }
          />
        </div>
      </div>

      {isLoadingNotificationCounts ? (
        <SkeletonStats />
      ) : (
        <motion.div
          className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 md:gap-4 lg:gap-6"
          variants={container}
          initial="hidden"
          animate="show"
        >
          {statsCards.map((card) => (
            <motion.div key={card.title} variants={item}>
              <div className="relative overflow-hidden rounded-xl h-full">
                <div className={`absolute inset-0 bg-linear-to-br ${card.gradient} opacity-10`} />
                <div className="relative p-3 sm:p-4 md:p-6 h-full">
                  <div className="flex items-start justify-between gap-2 sm:gap-3">
                    <div className="space-y-1 sm:space-y-2">
                      <h3 className="font-medium text-xs sm:text-sm text-muted-foreground">
                        {card.title}
                      </h3>
                      <div className="flex items-baseline gap-1 sm:gap-2">
                        <p className="text-xl sm:text-2xl font-semibold">
                          {notificationCountsError ? 'N/A' : card.value}
                        </p>
                      </div>
                    </div>
                    <div
                      className={`shrink-0 p-1.5 sm:p-2.5 rounded-lg bg-linear-to-br ${card.gradient}`}
                    >
                      <card.icon className="w-3 h-3 sm:w-4 sm:h-4 text-white" />
                    </div>
                  </div>
                  <p className="mt-1 sm:mt-2 text-xs text-muted-foreground">
                    {notificationCountsError ? 'Data unavailable' : card.description}
                  </p>
                  <div className="absolute bottom-0 right-0 transform translate-x-1/4 translate-y-1/4">
                    <div
                      className={`w-10 h-10 sm:w-16 sm:h-16 rounded-full bg-linear-to-br ${card.gradient} opacity-20 blur-2xl`}
                    />
                  </div>
                </div>
              </div>
            </motion.div>
          ))}
        </motion.div>
      )}

      <Tabs defaultValue="user" className="w-full" onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-2 max-w-full sm:max-w-md">
          <TabsTrigger
            value="user"
            className="flex items-center gap-1 sm:gap-2 text-xs sm:text-sm data-[state=active]:bg-indigo-500 data-[state=active]:text-white"
          >
            <Users className="h-3 w-3 sm:h-4 sm:w-4" />
            Thông báo người dùng
          </TabsTrigger>
          <TabsTrigger
            value="system"
            className="flex items-center gap-1 sm:gap-2 text-xs sm:text-sm data-[state=active]:bg-indigo-500 data-[state=active]:text-white"
          >
            <BellRing className="h-3 w-3 sm:h-4 sm:w-4" />
            Thông báo hệ thống
          </TabsTrigger>
        </TabsList>
        <TabsContent value="user" className="mt-4 md:mt-6">
          <UserNotificationList
            ranks={ranks || []}
            isLoadingRanks={isLoadingRanks}
            ranksError={ranksError}
          />
        </TabsContent>
        <TabsContent value="system" className="mt-4 md:mt-6">
          <SystemNotificationList />
        </TabsContent>
      </Tabs>
    </div>
  );
}
