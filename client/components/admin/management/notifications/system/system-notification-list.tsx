'use client';

import { useState, useRef, useEffect } from 'react';
import { useSession } from 'next-auth/react';
import { Input } from '@/components/ui/input';
import { Search } from 'lucide-react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useInfiniteQuery } from '@tanstack/react-query';
import NotificationSystemSkeleton from '@/components/admin/management/notifications/system/notification-system-skeleton';
import { notificationService } from '@/services/notificationService';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import SystemNotificationCard from './system-notification-card';

export function SystemNotificationList() {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const [searchTerm, setSearchTerm] = useState('');
  const parentRef = useRef<HTMLDivElement>(null);

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    isFetching,
    isError,
    error,
  } = useInfiniteQuery({
    queryKey: ['notifications', 'system', { search: searchTerm }],
    queryFn: ({ pageParam }) =>
      notificationService.getSystemNotifications(pageParam, undefined, searchTerm, accessToken),
    initialPageParam: 0,
    getNextPageParam: (lastPage) => lastPage.nextPage,
    enabled: !!accessToken,
  });

  const deleteSystemNotificationMutation = useSimpleMutation<string, void>({
    mutationKey: ['delete-system-notification'],
    mutationFn: (id) => notificationService.deleteSystemNotification(id, accessToken),
    queryKey: ['notifications', 'system'],
    successMessage: 'Đã xóa thông báo hệ thống thành công',
  });

  const allNotifications = data?.pages.flatMap((page) => page.items) ?? [];

  const rowVirtualizer = useVirtualizer({
    count: hasNextPage ? allNotifications.length + 1 : allNotifications.length,
    getScrollElement: () => parentRef.current,
    estimateSize: (index) => {
      if (index >= allNotifications.length) return 180;

      const notification = allNotifications[index];
      const contentLength = notification?.content?.length || 0;

      const baseHeight = 170;
      const contentLines = Math.ceil(contentLength / 40);
      const contentHeight = contentLines * 4;

      return baseHeight + contentHeight;
    },
    overscan: 5,
  });

  useEffect(() => {
    const [lastItem] = [...rowVirtualizer.getVirtualItems()].reverse();

    if (!lastItem) {
      return;
    }

    if (lastItem.index >= allNotifications.length - 1 && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [hasNextPage, fetchNextPage, allNotifications.length, isFetchingNextPage, rowVirtualizer]);

  const handleDelete = (id: string) => {
    deleteSystemNotificationMutation.mutate(id);
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        <div className="relative flex-1 w-full max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm kiếm thông báo hệ thống..."
            className="pl-9 w-full"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </div>

      {isFetching && !isFetchingNextPage && (
        <div className="text-center text-muted-foreground">Đang tìm kiếm...</div>
      )}

      {isLoading ? (
        <div className="grid gap-4">
          {[...Array(3)].map((_, i) => (
            <NotificationSystemSkeleton key={i} />
          ))}
        </div>
      ) : isError ? (
        <div className="text-center text-red-500">Lỗi: {error.message}</div>
      ) : allNotifications.length === 0 ? (
        <div className="text-center py-8 text-muted-foreground">
          Không tìm thấy thông báo hệ thống nào
        </div>
      ) : (
        <div
          ref={parentRef}
          className="h-[calc(100vh-220px)] sm:h-[800px] overflow-auto rounded-lg border"
        >
          <div
            style={{
              height: `${rowVirtualizer.getTotalSize()}px`,
              width: '100%',
              position: 'relative',
            }}
          >
            {rowVirtualizer.getVirtualItems().map((virtualItem) => {
              const isLoaderRow = virtualItem.index >= allNotifications.length;
              const notification = isLoaderRow ? null : allNotifications[virtualItem.index];

              return isLoaderRow ? (
                <div
                  key="loader"
                  style={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: '100%',
                    height: `${virtualItem.size}px`,
                    transform: `translateY(${virtualItem.start}px)`,
                  }}
                >
                  {isFetchingNextPage && <NotificationSystemSkeleton />}
                </div>
              ) : (
                <div
                  key={notification?.id || virtualItem.index}
                  style={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: '100%',
                    height: `${virtualItem.size}px`,
                    transform: `translateY(${virtualItem.start}px)`,
                    padding: '6px',
                  }}
                  className="transition-transform duration-200 hover:scale-[1.01]"
                >
                  <SystemNotificationCard
                    notification={notification}
                    virtualItem={virtualItem}
                    handleDelete={handleDelete}
                    isDeletePending={deleteSystemNotificationMutation.isPending}
                  />
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
