import { Bell } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import type { Notification } from '@/types/notification';
import { capitalize, formatDateTimeVietnamese } from '@/lib/utils';
import RankBadge from '@/components/global/rank-badge';

interface VirtualItem {
  index: number;
  size: number;
  start: number;
}

interface UserNotificationCardProps {
  notification: Notification | null;
  virtualItem: VirtualItem;
}

export default function UserNotificationCard({
  notification,
  virtualItem,
}: UserNotificationCardProps) {
  return (
    <div className="h-full rounded-xl bg-white dark:bg-gray-800 shadow-lg hover:shadow-xl transition-all duration-200 border border-gray-200 dark:border-gray-700">
      <div className="p-3 sm:p-4 flex flex-col h-full justify-between">
        {/* Header with icon and title */}
        <div className="flex items-start gap-2 sm:gap-3">
          <div className="p-1.5 sm:p-2 rounded-full bg-background/80 backdrop-blur-xs text-gray-500">
            <Bell className="h-4 w-4 sm:h-5 sm:w-5" />
          </div>
          <div className="flex flex-col flex-1">
            <h3 className="text-sm sm:text-base font-semibold line-clamp-1">
              {notification?.title || `Thông báo ${virtualItem.index + 1}`}
            </h3>
            <p className="text-xs sm:text-sm text-muted-foreground mt-0.5">
              {notification?.createdAt
                ? formatDateTimeVietnamese(notification.createdAt)
                : 'Không rõ ngày'}
            </p>
          </div>
        </div>

        {/* Content */}
        <div className="mt-3 sm:mt-4 grow">
          <p className="text-sm sm:text-base line-clamp-2 md:line-clamp-3">
            {notification?.content ||
              'Nội dung thông báo mẫu. Đây sẽ là nội dung thực tế của thông báo.'}
          </p>
        </div>

        {/* Footer with ranks and metadata */}
        <div className="mt-2 pt-2 sm:pt-3 border-t border-gray-200/40 dark:border-gray-700/40 flex items-center justify-between flex-wrap gap-2">
          <div className="flex flex-wrap gap-2">
            {notification?.rankName ? (
              <RankBadge rank={capitalize(notification?.rankName)} />
            ) : (
              <Badge className="bg-gray-600 text-white shadow-xs">Chưa xác định hạng</Badge>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
