import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import {
  Eye,
  Mail,
  Phone,
  Calendar,
  Building,
  MapPin,
  CreditCard,
  Briefcase,
  User,
} from 'lucide-react';
import type { Staff } from '@/types/staff';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { cn } from '@/lib/utils';

interface ViewStaffDialogProps {
  staff: Staff;
}

export function ViewStaffDialog({ staff }: ViewStaffDialogProps) {
  // Lấy chữ cái đầu cho avatar
  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  };

  // Định dạng lương
  const formatSalary = (salary: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(salary);
  };

  // Màu sắc theo trạng thái
  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return 'bg-green-50 text-green-700 border-green-200';
      case 'inactive':
        return 'bg-gray-50 text-gray-700 border-gray-200';
      case 'pending':
        return 'bg-amber-50 text-amber-700 border-amber-200';
      default:
        return 'bg-blue-50 text-blue-700 border-blue-200';
    }
  };

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm" title="Xem chi tiết" className="p-2 md:p-3">
          <Eye className="h-4 w-4" />
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[700px] p-0 overflow-hidden">
        {/* Header Banner */}
        <div className="bg-linear-to-r from-primary/10 to-primary/5 p-6">
          <div className="flex flex-col md:flex-row gap-6 items-center md:items-start">
            <Avatar className="h-24 w-24 border-4 border-background shadow-md">
              <AvatarImage src={staff.avatar} alt={staff.fullName} />
              <AvatarFallback className="text-xl bg-primary/10">
                {getInitials(staff.fullName)}
              </AvatarFallback>
            </Avatar>

            <div className="flex flex-col items-center md:items-start gap-2">
              <h2 className="text-2xl font-bold">{staff.fullName}</h2>
              <div className="flex flex-wrap gap-2 justify-center md:justify-start">
                <Badge
                  className={cn('rounded-full px-3 py-1 font-medium', getStatusColor(staff.status))}
                >
                  {staff.status === 'Active'
                    ? 'Đang làm việc'
                    : staff.status === 'Inactive'
                    ? 'Ngừng làm việc'
                    : staff.status === 'Pending'
                    ? 'Chờ xử lý'
                    : staff.status}
                </Badge>
                <Badge variant="outline" className="rounded-full px-3 py-1">
                  {staff.roleName === 'Staff'
                    ? 'Nhân viên'
                    : staff.roleName === 'Manager'
                    ? 'Quản lý'
                    : staff.roleName}
                </Badge>
              </div>
              <p className="text-muted-foreground mt-1 text-center md:text-left">{staff.title}</p>
            </div>
          </div>
        </div>

        <div className="p-6 space-y-6">
          {/* Thông tin cá nhân */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold flex items-center gap-2">
              <User className="h-5 w-5 text-primary" />
              Thông tin cá nhân
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4 pl-7">
              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Mail className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Email</span>
                </div>
                <p className="font-medium">{staff.email}</p>
              </div>

              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Phone className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Số điện thoại</span>
                </div>
                <p className="font-medium">{staff.phone}</p>
              </div>

              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Ngày sinh</span>
                </div>
                <p className="font-medium">{staff.dob}</p>
              </div>

              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Briefcase className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Loại hợp đồng</span>
                </div>
                <p className="font-medium">{staff.type}</p>
              </div>

              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <CreditCard className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">
                    {staff.type === 'Lương Theo Giờ' ? 'Lương theo giờ' : 'Lương tháng'}
                  </span>
                </div>
                <p className="font-medium text-primary">{formatSalary(staff.salaryBase)}</p>
              </div>
            </div>
          </div>

          <Separator />

          {/* Thông tin chi nhánh */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold flex items-center gap-2">
              <Building className="h-5 w-5 text-primary" />
              Thông tin chi nhánh
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4 pl-7">
              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Building className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Tên chi nhánh</span>
                </div>
                <p className="font-medium">{staff.branchName}</p>
              </div>

              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <Building className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Loại chi nhánh</span>
                </div>
                <p className="font-medium">
                  {staff.typeBranch === 'Sub' ? 'Chi Nhánh Con' : 'Chi Nhánh Chính'}
                </p>
              </div>

              <div className="space-y-1 md:col-span-2">
                <div className="flex items-center gap-2">
                  <MapPin className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium text-muted-foreground">Địa chỉ</span>
                </div>
                <p className="font-medium">{staff.branchAddress}</p>
              </div>
            </div>
          </div>

          {/* Mô tả */}
          <>
            <Separator />
            <div className="space-y-3">
              <h3 className="text-lg font-semibold">Mô tả</h3>
              <div className="bg-muted/50 p-4 rounded-lg">
                <p className="text-sm leading-relaxed">{staff.description}</p>
              </div>
            </div>
          </>
        </div>
      </DialogContent>
    </Dialog>
  );
}
