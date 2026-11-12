'use client';

import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
  FormDescription,
} from '@/components/ui/form';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Plus } from 'lucide-react';
import Loader from '@/components/global/loader';
import { Branch } from '@/types/branch';
import { CreateNewStaffDto } from '@/types/staff';
import { addStaffSchema } from '@/schema';
import { SALARY_TYPES } from '@/constants/salary-types';
import { useSession } from 'next-auth/react';

type FormValues = z.infer<typeof addStaffSchema>;

interface AddStaffDialogProps {
  branches: Branch[];
  isLoadingBranches?: boolean;
  branchesError?: Error | null;
  onCreate: (staff: CreateNewStaffDto) => void;
  isCreatePending?: boolean;
}

export function AddStaffDialog({
  branches,
  isLoadingBranches = false,
  branchesError,
  onCreate,
  isCreatePending = false,
}: AddStaffDialogProps) {
  const [open, setOpen] = useState(false);
  const { data: session } = useSession();
  const isManager = session?.user.roleName === 'Quản lý';

  const form = useForm<FormValues>({
    resolver: zodResolver(addStaffSchema),
    defaultValues: {
      fullName: '',
      email: '',
      phone: '',
      isStaff: true,
      branchId: '',
      title: '',
      type: SALARY_TYPES.HOURLY,
      salaryBase: 0,
    },
    mode: 'onChange',
  });

  const onSubmit = (values: FormValues) => {
    const newStaff: CreateNewStaffDto = {
      fullname: values.fullName,
      email: values.email,
      phone: values.phone,
      isStaff: values.isStaff,
      branchId: values.branchId,
      title: values.title,
      type: values.type,
      salaryBase: values.salaryBase,
    };

    onCreate(newStaff);
  };

  useEffect(() => {
    if (!isCreatePending) {
      setOpen(false);
    }
  }, [isCreatePending]);

  const handleOpenChange = (isOpen: boolean) => {
    setOpen(isOpen);

    if (!isOpen) {
      form.reset({
        fullName: '',
        email: '',
        phone: '',
        isStaff: true,
        branchId: '',
        title: '',
        type: SALARY_TYPES.HOURLY,
        salaryBase: 0,
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button className="gap-2">
          <Plus className="h-5 w-5" />
          Thêm nhân viên
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[500px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Tạo nhân viên mới</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-3">
            <FormField
              control={form.control}
              name="fullName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Họ và tên</FormLabel>
                  <FormControl>
                    <Input placeholder="Nhập họ tên đầy đủ" {...field} />
                  </FormControl>
                  <FormDescription>Tên đầy đủ của nhân viên</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="Nhập địa chỉ email" type="email" {...field} />
                  </FormControl>
                  <FormDescription>Địa chỉ email của nhân viên</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="phone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Số điện thoại</FormLabel>
                  <FormControl>
                    <Input placeholder="Nhập số điện thoại" {...field} />
                  </FormControl>
                  <FormDescription>Số liên hệ của nhân viên</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="isStaff"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Vai trò</FormLabel>
                  <FormControl>
                    <Select
                      value={field.value ? 'Nhân viên' : 'Quản lý'}
                      onValueChange={(value) => field.onChange(value === 'Nhân viên')}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Chọn vai trò" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Nhân viên">Nhân viên</SelectItem>
                        <SelectItem value="Quản lý">Quản lý</SelectItem>
                      </SelectContent>
                    </Select>
                  </FormControl>
                  <FormDescription>Vai trò của nhân viên trong hệ thống</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Chức danh</FormLabel>
                  <FormControl>
                    <Input placeholder="Nhập chức danh" {...field} />
                  </FormControl>
                  <FormDescription>Chức danh công việc của nhân viên</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Hình thức lương</FormLabel>
                  <FormControl>
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger>
                        <SelectValue placeholder="Chọn hình thức lương" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value={SALARY_TYPES.HOURLY}>Lương theo giờ</SelectItem>
                        <SelectItem value={SALARY_TYPES.FIXED}>Lương cứng</SelectItem>
                      </SelectContent>
                    </Select>
                  </FormControl>
                  <FormDescription>Lương theo giờ hoặc lương tháng cố định</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="salaryBase"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    {form.watch('type') === SALARY_TYPES.HOURLY
                      ? 'Lương theo giờ'
                      : 'Lương tháng cố định'}
                  </FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      placeholder={
                        form.watch('type') === SALARY_TYPES.HOURLY
                          ? 'Nhập lương theo giờ'
                          : 'Nhập lương tháng'
                      }
                      {...field}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                    />
                  </FormControl>
                  <FormDescription>
                    {form.watch('type') === SALARY_TYPES.HOURLY
                      ? 'Số tiền thanh toán mỗi giờ'
                      : 'Số tiền lương cố định mỗi tháng'}
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="branchId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Chi nhánh</FormLabel>
                  <FormControl>
                    {isLoadingBranches ? (
                      <div className="h-10 px-3 py-2 border rounded-md flex items-center text-muted-foreground">
                        Đang tải chi nhánh...
                      </div>
                    ) : branchesError ? (
                      <div className="h-10 px-3 py-2 border rounded-md flex items-center text-destructive">
                        Lỗi khi tải chi nhánh
                      </div>
                    ) : isManager ? (
                      <Input {...field} disabled />
                    ) : (
                      <Select value={field.value} onValueChange={field.onChange}>
                        <SelectTrigger>
                          <SelectValue placeholder="Chọn chi nhánh" />
                        </SelectTrigger>
                        <SelectContent>
                          {branches.map((branch) => (
                            <SelectItem key={branch.id} value={branch.id}>
                              {branch.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    )}
                  </FormControl>
                  {isManager && (
                    <FormDescription>Chi nhánh được tự động thiết lập cho Quản lý</FormDescription>
                  )}
                  <FormMessage />
                </FormItem>
              )}
            />

            <DialogFooter className="gap-2 pt-4">
              <Button
                type="button"
                variant="outline"
                onClick={() => handleOpenChange(false)}
                disabled={isCreatePending}
                className="w-full sm:w-auto"
              >
                Hủy bỏ
              </Button>
              <Button
                type="submit"
                disabled={isCreatePending || !form.formState.isValid}
                className="w-full sm:w-auto bg-linear-to-r from-indigo-500 to-purple-500 hover:from-indigo-600 hover:to-purple-600 text-white"
              >
                <Loader state={isCreatePending} textLoading="Đang tạo...">
                  Tạo nhân viên
                </Loader>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
