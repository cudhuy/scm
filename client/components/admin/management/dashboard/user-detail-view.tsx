'use client';

import type React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { User, Mail, Phone, Calendar, MapPin, Award, CheckCircle, XCircle } from 'lucide-react';
import { formatDateLocale } from '@/lib/utils';
import { Skeleton } from '@/components/ui/skeleton';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { UserDetailDto } from '@/types/dashboard';
import { memo, useCallback } from 'react';

interface UserDetailViewProps {
  userId: string;
  userDetails: UserDetailDto | null;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (open: boolean) => void;
  isLoadingUserDetails: boolean;
  userDetailError: Error | null;
  onViewUserDetail: (userId: string) => void;
}

export const UserDetailView = memo(function UserDetailView({
  userId,
  userDetails,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  isLoadingUserDetails,
  userDetailError,
  onViewUserDetail,
}: UserDetailViewProps) {
  const handleOpenDialog = useCallback(() => {
    if (!isLoadingUserDetails) {
      onViewUserDetail(userId);
      setIsDetailDialogOpen(true);
    }
  }, [userId, isLoadingUserDetails, onViewUserDetail, setIsDetailDialogOpen]);

  const getInitials = useCallback((name: string | null) => {
    if (!name) return 'U';
    return name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }, []);

  return (
    <Dialog open={isDetailDialogOpen} onOpenChange={setIsDetailDialogOpen}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-xl">
            {isLoadingUserDetails ? <Skeleton className="h-7 w-48" /> : 'Hồ sơ người dùng'}
          </DialogTitle>
        </DialogHeader>

        {isLoadingUserDetails ? (
          <UserDetailSkeleton />
        ) : userDetailError ? (
          <ErrorDisplay error={userDetailError} onRetry={handleOpenDialog} />
        ) : userDetails ? (
          <UserDetailContent userDetails={userDetails} getInitials={getInitials} />
        ) : (
          <EmptyState />
        )}
      </DialogContent>
    </Dialog>
  );
});

export default UserDetailView;

interface UserDetailContentProps {
  userDetails: UserDetailDto;
  getInitials: (name: string | null) => string;
}

const UserDetailContent = memo(function UserDetailContent({
  userDetails,
  getInitials,
}: UserDetailContentProps) {
  const getStatusBadge = (status: string | null) => {
    if (!status) return <Badge variant="outline">N/A</Badge>;

    switch (status.toLowerCase()) {
      case 'active':
        return <Badge variant="default">Đang hoạt động</Badge>;
      case 'inactive':
        return <Badge variant="secondary">Ngừng hoạt động</Badge>;
      case 'suspended':
        return <Badge variant="destructive">Bị đình chỉ</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row items-center sm:items-start gap-4 p-4 bg-muted/30 rounded-lg">
        <Avatar className="h-20 w-20 border-2 border-primary/20">
          <AvatarImage
            src={userDetails.avatar || undefined}
            alt={userDetails.fullName || 'Người dùng'}
          />
          <AvatarFallback className="text-lg">{getInitials(userDetails.fullName)}</AvatarFallback>
        </Avatar>

        <div className="flex flex-col items-center sm:items-start">
          <h2 className="text-xl font-semibold">
            {userDetails.fullName || 'Người dùng không tên'}
          </h2>

          <div className="flex items-center gap-2 mt-1 flex-wrap justify-center sm:justify-start">
            {userDetails.isVerified ? (
              <Badge variant="default" className="flex items-center gap-1">
                <CheckCircle className="h-3 w-3" />
                Đã xác minh
              </Badge>
            ) : (
              <Badge variant="secondary" className="flex items-center gap-1">
                <XCircle className="h-3 w-3" />
                Chưa xác minh
              </Badge>
            )}

            {userDetails.status && getStatusBadge(userDetails.status)}

            {userDetails.rankName && (
              <Badge variant="outline" className="flex items-center gap-1">
                <Award className="h-3 w-3" />
                {userDetails.rankName}
              </Badge>
            )}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <UserInfoCard
          icon={<Mail className="h-5 w-5 text-primary" />}
          label="Thông tin liên hệ"
          items={[
            {
              label: 'Email',
              value: userDetails.email || 'Chưa cung cấp',
              icon: <Mail className="h-4 w-4 text-muted-foreground" />,
            },
            {
              label: 'Số điện thoại',
              value: userDetails.phone || 'Chưa cung cấp',
              icon: <Phone className="h-4 w-4 text-muted-foreground" />,
            },
          ]}
        />

        <UserInfoCard
          icon={<User className="h-5 w-5 text-primary" />}
          label="Thông tin cá nhân"
          items={[
            {
              label: 'Ngày sinh',
              value: userDetails.dob ? formatDateLocale(userDetails.dob) : 'Chưa cung cấp',
              icon: <Calendar className="h-4 w-4 text-muted-foreground" />,
            },
            {
              label: 'Địa chỉ',
              value: userDetails.address || 'Chưa cung cấp',
              icon: <MapPin className="h-4 w-4 text-muted-foreground" />,
            },
          ]}
        />
      </div>
    </div>
  );
});

interface UserInfoCardProps {
  icon: React.ReactNode;
  label: string;
  items: {
    label: string;
    value: React.ReactNode;
    icon?: React.ReactNode;
  }[];
}

const UserInfoCard = memo(function UserInfoCard({ icon, label, items }: UserInfoCardProps) {
  return (
    <div className="border rounded-lg p-4 space-y-4">
      <div className="flex items-center gap-2">
        {icon}
        <h3 className="font-semibold">{label}</h3>
      </div>

      <div className="space-y-3">
        {items.map((item, index) => (
          <div key={index} className="flex items-start gap-3">
            {item.icon}
            <div className="flex flex-col min-w-0 flex-1">
              <span className="text-sm text-muted-foreground">{item.label}</span>
              <span
                className="font-medium truncate"
                title={typeof item.value === 'string' ? item.value : undefined}
              >
                {item.value}
              </span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
});

function UserDetailSkeleton() {
  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row items-center sm:items-start gap-4 p-4 bg-muted/30 rounded-lg">
        <Skeleton className="h-20 react w-20 rounded-full" />
        <div className="flex flex-col items-center sm:items-start gap-2">
          <Skeleton className="h-7 w-48" />
          <div className="flex gap-2">
            <Skeleton className="h-5 w-20" />
            <Skeleton className="h-5 w-20" />
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Skeleton className="h-40 w-full rounded-lg" />
        <Skeleton className="h-40 w-full rounded-lg" />
      </div>
    </div>
  );
}

interface ErrorDisplayProps {
  error: Error;
  onRetry: () => void;
}

function ErrorDisplay({ error, onRetry }: ErrorDisplayProps) {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-red-100">
        <span className="text-red-600 text-xl">!</span>
      </div>
      <h3 className="text-lg font-medium text-destructive">Không thể tải chi tiết người dùng</h3>
      <p className="text-sm text-muted-foreground">
        {error?.message || 'Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.'}
      </p>
      <Button variant="outline" onClick={onRetry}>
        Thử lại
      </Button>
    </div>
  );
}

function EmptyState() {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted">
        <User className="h-6 w-6 text-muted-foreground" />
      </div>
      <h3 className="text-lg font-medium">Không có chi tiết người dùng</h3>
      <p className="text-sm text-muted-foreground">
        Không tìm thấy thông tin người dùng được yêu cầu.
      </p>
    </div>
  );
}
