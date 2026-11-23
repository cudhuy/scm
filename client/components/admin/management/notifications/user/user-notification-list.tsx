'use client';

import { useState, useRef, useEffect } from 'react';
import { useSession } from 'next-auth/react';

import { Input } from '@/components/ui/input';
import { Search, Filter, Star } from 'lucide-react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useInfiniteQuery } from '@tanstack/react-query';
import NotificationUserSkeleton from '@/components/admin/management/notifications/user/notification-user-skeleton';
import { notificationService } from '@/services/notificationService';
import UserNotificationCard from './user-notification-card';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuCheckboxItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';
import { MembershipRank } from '@/types/membership-rank';
import { rankIconMap } from '@/components/global/rank-badge';

const rankColorMap: Record<string, string> = {
  regulars: 'yellow',
  vips: 'purple',
  stars: 'blue',
};

interface UserNotificationListProps {
  ranks: MembershipRank[];
  isLoadingRanks: boolean;
  ranksError: unknown;
}

export function UserNotificationList({
  ranks,
  isLoadingRanks,
  ranksError,
}: UserNotificationListProps) {
  const { data: session } = useSession();
  const accessToken = session?.user.accessToken;

  const [searchTerm, setSearchTerm] = useState('');
  const [selectedRanks, setSelectedRanks] = useState<MembershipRank[]>([]);
  const parentRef = useRef<HTMLDivElement>(null);

  const selectedRankIds = selectedRanks.map((rank) => rank.id);

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
    queryKey: ['notifications', 'user', { search: searchTerm, ranks: selectedRankIds }],
    queryFn: ({ pageParam }) =>
      notificationService.getUserNotifications(
        pageParam,
        undefined,
        searchTerm,
        selectedRankIds.length > 0 ? selectedRankIds.join(',') : undefined,
        accessToken,
      ),
    initialPageParam: 0,
    getNextPageParam: (lastPage) => lastPage.nextPage,
    enabled: !!accessToken,
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

  const toggleRank = (rank: MembershipRank) => {
    setSelectedRanks((prev) =>
      prev.some((r) => r.id === rank.id) ? prev.filter((r) => r.id !== rank.id) : [...prev, rank],
    );
  };

  const clearFilters = () => {
    setSelectedRanks([]);
  };

  const getRankIcon = (rank: MembershipRank) => {
    const rankNameLower = rank.name?.toLowerCase();
    return rankNameLower && rankIconMap[rankNameLower] ? rankIconMap[rankNameLower] : Star;
  };

  const getRankColor = (rank: MembershipRank) => {
    const rankNameLower = rank.name?.toLowerCase();
    return rankNameLower && rankColorMap[rankNameLower]
      ? rankColorMap[rankNameLower]
      : 'bg-green-500';
  };

  const renderRankIcon = (rank: MembershipRank) => {
    const IconComponent = getRankIcon(rank);
    const color = getRankColor(rank);

    return <IconComponent className={`h-3 w-3`} fill="currentColor" color={color} />;
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-col sm:flex-row items-start sm:items-center gap-3">
        <div className="relative flex-1 w-full min-w-[200px]">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm kiếm thông báo..."
            className="pl-9 w-full"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        <div className="flex flex-wrap items-center gap-2 w-full sm:w-auto">
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                size="sm"
                className="flex items-center gap-2 w-full sm:w-auto"
              >
                <Filter className="h-4 w-4" />
                <span>Lọc theo hạng</span>
                {selectedRanks.length > 0 && (
                  <Badge variant="secondary" className="ml-1">
                    {selectedRanks.length}
                  </Badge>
                )}
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>Chọn hạng thành viên</DropdownMenuLabel>
              <DropdownMenuSeparator />
              {isLoadingRanks ? (
                <div className="p-2 text-center text-sm text-muted-foreground">
                  Đang tải danh sách hạng...
                </div>
              ) : ranksError ? (
                <div className="p-2 text-center text-sm text-red-500">
                  Lỗi khi tải hạng thành viên
                </div>
              ) : ranks && ranks.length > 0 ? (
                ranks.map((rank) => (
                  <DropdownMenuCheckboxItem
                    key={rank.id}
                    checked={selectedRanks.some((r) => r.id === rank.id)}
                    onCheckedChange={() => toggleRank(rank)}
                    className="flex items-center gap-2"
                  >
                    {renderRankIcon(rank)}
                    <span className="capitalize">{rank.name}</span>
                  </DropdownMenuCheckboxItem>
                ))
              ) : (
                <div className="p-2 text-center text-sm text-muted-foreground">
                  Không tìm thấy hạng
                </div>
              )}
              <DropdownMenuSeparator />
              <div className="p-1">
                <Button
                  variant="ghost"
                  size="sm"
                  className="w-full text-xs"
                  onClick={clearFilters}
                  disabled={selectedRanks.length === 0}
                >
                  Xóa bộ lọc
                </Button>
              </div>
            </DropdownMenuContent>
          </DropdownMenu>

          {selectedRanks.length > 0 && (
            <div className="flex flex-wrap gap-1 items-center">
              {selectedRanks.map((rank) => (
                <Badge
                  key={rank.id}
                  variant="outline"
                  className="flex items-center gap-1 capitalize"
                >
                  {renderRankIcon(rank)}
                  {rank.name}
                  <button
                    className="ml-1 hover:bg-muted rounded-full p-0.5"
                    onClick={() => toggleRank(rank)}
                  >
                    <span className="sr-only">Xóa lọc {rank.name}</span>
                    <svg width="12" height="12" viewBox="0 0 12 12" fill="none">
                      <path
                        d="M9 3L3 9M3 3L9 9"
                        stroke="currentColor"
                        strokeWidth="1.5"
                        strokeLinecap="round"
                      />
                    </svg>
                  </button>
                </Badge>
              ))}
            </div>
          )}
        </div>
      </div>

      {isFetching && !isFetchingNextPage && (
        <div className="text-center text-muted-foreground">Đang tìm kiếm...</div>
      )}

      {isLoading ? (
        <div className="grid gap-4">
          {[...Array(3)].map((_, i) => (
            <NotificationUserSkeleton key={i} />
          ))}
        </div>
      ) : isError ? (
        <div className="text-center text-muted-foreground">Lỗi: {error.message}</div>
      ) : allNotifications.length === 0 ? (
        <div className="text-center py-8 text-muted-foreground">Không tìm thấy thông báo nào</div>
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
                  {isFetchingNextPage && <NotificationUserSkeleton />}
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
                  <UserNotificationCard notification={notification} virtualItem={virtualItem} />
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
