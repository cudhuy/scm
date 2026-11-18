export interface Notification {
  id: string;
  title: string;
  content: string;
  rankId?: string;
  rankName?: string;
  priority?: string;
  priorityEnum?: string;
  createdAt: Date;
}

export interface CreateUserNotificationDto {
  title: string;
  content: string;
  rankIds: string[];
}

export interface CreateSystemNotificationDto {
  title: string;
  content: string;
  priorityEnum?: string;
}

export interface NotificationCounts {
  userNotificationsCount: number;
  systemNotificationsCount: number;
  totalNotificationsCount: number;
  notificationsTodayCount: number;
}

export interface CustomerNotification {
  id: string;
  title: string;
  content: string;
  priority: 0;
  priorityEnum: string;
}