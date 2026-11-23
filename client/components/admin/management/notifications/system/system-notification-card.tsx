import { Percent, Tag, Coffee } from 'lucide-react';
import { DeleteDialog } from '@/components/admin/common/delete-dialog';
import type { Notification } from '@/types/notification';
import { cn, formatDateTimeVietnamese } from '@/lib/utils';

const priorityColors = {
  High: {
    border: 'border-red-500/20',
    bg: 'bg-red-700/30',
    text: 'text-red-700 dark:text-red-400',
    icon: 'text-red-500',
    badge: 'bg-red-500 text-white dark:bg-red-600 dark:text-white',
    shadow: 'shadow-xs shadow-red-500/20',
    iconComponent: <Percent className="h-4 w-4 sm:h-5 sm:w-5" />,
  },
  Medium: {
    border: 'border-amber-500/20',
    bg: 'bg-yellow-500/30',
    text: 'text-yellow-700 dark:text-yellow-400',
    icon: 'text-amber-500',
    badge: 'bg-amber-500 dark:bg-yellow-500 text-white dark:bg-yellow-600 dark:text-white',
    shadow: 'shadow-xs shadow-yellow-500/20',
    iconComponent: <Tag className="h-4 w-4 sm:h-5 sm:w-5" />,
  },
  Low: {
    border: 'border-green-500/20',
    bg: 'bg-blue-500/10',
    text: 'text-blue-700 dark:text-blue-400',
    icon: 'text-green-500',
    badge: 'bg-green-500 text-white dark:bg-blue-500 dark:bg-blue-600 dark:text-white',
    shadow: 'shadow-xs shadow-blue-500/20',
    iconComponent: <Coffee className="h-4 w-4 sm:h-5 sm:w-5" />,
  },
};

interface VirtualItem {
  index: number;
  size: number;
  start: number;
}

interface SystemNotificationCardProps {
  notification: Notification | null;
  virtualItem: VirtualItem;
  handleDelete: (id: string) => void;
  isDeletePending?: boolean;
}

export default function SystemNotificationCard({
  notification,
  virtualItem,
  handleDelete,
  isDeletePending = false,
}: SystemNotificationCardProps) {
  const priority = (notification?.priorityEnum as keyof typeof priorityColors) || 'Low';
  const priorityStyle = priorityColors[priority];
  const priorityText = {
    High: 'Đặc biệt',
    Medium: 'Ưu đãi',
    Low: 'Thường xuyên',
  }[priority];

  return (
    <div
      className={cn(
        'h-full rounded-lg border shadow-xs backdrop-blur-xs',
        'transition-all duration-300 hover:shadow-md',
        priorityStyle.border,
        priorityStyle.bg,
        priorityStyle.shadow,
      )}
    >
      <div className="p-3 sm:p-4 flex flex-col h-full justify-between">
        {/* Header row with icon, title and delete button */}
        <div className="flex items-start justify-between gap-1 sm:gap-2">
          <div className="flex items-start gap-2 sm:gap-3">
            <div
              className={cn(
                'p-1.5 sm:p-2 rounded-full',
                'bg-background/90 backdrop-blur-xs',
                priorityStyle.icon,
              )}
            >
              {priorityStyle.iconComponent}
            </div>
            <div className="flex flex-col">
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

          <DeleteDialog
            entityId={notification?.id || ''}
            entityName={notification?.title || 'Thông báo'}
            onDelete={handleDelete}
            isDeletePending={isDeletePending}
          />
        </div>

        {/* Content row */}
        <div className="mt-3 sm:mt-4 grow">
          <p className="text-sm sm:text-base line-clamp-2 wrap-break-word">
            {notification?.content ||
              'Nội dung thông báo mẫu. Đây sẽ là nội dung thực tế của thông báo hệ thống.'}
          </p>
        </div>

        {/* Footer with metadata */}
        <div className="mt-2 pt-2 sm:pt-3 border-t border-border/40 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-2 sm:gap-0">
          <span
            className={cn(
              'text-xs sm:text-sm font-bold px-2 py-0.5 sm:py-1 rounded-md',
              priorityStyle.badge,
              priority === 'High'
                ? 'ring-red-300'
                : priority === 'Medium'
                ? 'ring-yellow-300'
                : 'ring-blue-300',
              'transform -translate-y-1',
            )}
          >
            {priorityText}
          </span>
        </div>
      </div>
    </div>
  );
}
