'use client';

import { useState, useEffect, useCallback } from 'react';
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
import { Edit } from 'lucide-react';
import Loader from '@/components/global/loader';
import { Branch } from '@/types/branch';
import { UpdateStaffDto, Staff } from '@/types/staff';
import { editStaffSchema } from '@/schema';
import { useSession } from 'next-auth/react';

type FormValues = z.infer<typeof editStaffSchema>;

interface EditStaffDialogProps {
  staff: Staff;
  branches: Branch[];
  isLoadingBranches?: boolean;
  branchesError?: Error | null;
  onUpdate: (staff: UpdateStaffDto) => void;
  isUpdatePending?: boolean;
}

export function EditStaffDialog({
  staff,
  branches,
  isLoadingBranches = false,
  branchesError,
  onUpdate,
  isUpdatePending = false,
}: EditStaffDialogProps) {
  const [open, setOpen] = useState(false);
  const { data: session } = useSession();
  const isManager = session?.user.roleName === 'Quản lý';

  const form = useForm<FormValues>({
    resolver: zodResolver(editStaffSchema),
    defaultValues: {
      title: '',
      type: 'Lương Theo Giờ',
      salaryBase: 0,
      branchId: '',
    },
    mode: 'onChange',
  });

  const getBranchIdFromName = useCallback(
    (branchName: string) => {
      return branches.find((b) => b.name === branchName)?.id || '';
    },
    [branches],
  );

  useEffect(() => {
    if (staff && branches.length > 0) {
      const branchId = getBranchIdFromName(staff.branchName);

      form.reset({
        title: staff.title || '',
        type: staff.type || 'Lương Theo Giờ',
        salaryBase: staff.salaryBase || 0,
        branchId: branchId,
      });
    }
  }, [staff, branches, getBranchIdFromName, form]);

  const onSubmit = (values: FormValues) => {
    // Convert type to '1' or '2' before sending to backend
    const typeForUpdate = values.type === 'Lương Theo Giờ' ? '1' : '2';

    const updateStaff: UpdateStaffDto = {
      userId: staff.userId,
      title: values.title,
      type: typeForUpdate,
      salaryBase: values.salaryBase,
      branchId: values.branchId,
    };

    onUpdate(updateStaff);
    setOpen(false);
  };

  const handleOpenChange = (isOpen: boolean) => {
    setOpen(isOpen);

    if (!isOpen && staff) {
      const branchId = getBranchIdFromName(staff.branchName);
      form.reset({
        title: staff.title || '',
        type: staff.type || 'Lương Theo Giờ',
        salaryBase: staff.salaryBase || 0,
        branchId: branchId,
      });
    }
  };

  const getBranchName = (branchId: string) => {
    return branches.find((b) => b.id === branchId)?.name || 'Chọn chi nhánh';
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm" title="Chỉnh sửa" className="p-2 md:p-3">
          <Edit className="h-4 w-4" />
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[500px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Chỉnh sửa nhân viên: {staff.fullName}</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
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
                        <SelectValue>{field.value}</SelectValue>
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Lương Theo Giờ">Lương Theo Giờ</SelectItem>
                        <SelectItem value="Lương Cứng">Lương Cứng</SelectItem>
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
                    {form.watch('type') === 'Lương Theo Giờ' ? 'Lương theo giờ' : 'Lương tháng'}
                  </FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      placeholder={
                        form.watch('type') === 'Lương Theo Giờ'
                          ? 'Nhập lương theo giờ'
                          : 'Nhập lương tháng'
                      }
                      {...field}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                    />
                  </FormControl>
                  <FormDescription>
                    {form.watch('type') === 'Lương Theo Giờ'
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
                    ) : (
                      <Select
                        value={field.value}
                        onValueChange={field.onChange}
                        disabled={isManager}
                      >
                        <SelectTrigger>
                          <SelectValue>{getBranchName(field.value)}</SelectValue>
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
                    <FormDescription>Quản lý không thể thay đổi chi nhánh</FormDescription>
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
                disabled={isUpdatePending}
                className="w-full sm:w-auto"
              >
                Hủy bỏ
              </Button>
              <Button
                type="submit"
                disabled={isUpdatePending}
                className="w-full sm:w-auto bg-linear-to-r from-indigo-500 to-purple-500 hover:from-indigo-600 hover:to-purple-600 text-white"
              >
                <Loader state={isUpdatePending} textLoading="Đang cập nhật...">
                  Cập nhật
                </Loader>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
