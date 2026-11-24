'use client';

import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Edit } from 'lucide-react';
import { toast } from 'sonner';
import Loader from '@/components/global/loader';
import { Branch, UpdateBranchDto } from '@/types/branch';

const branchSchema = z.object({
  name: z.string().min(1, 'Tên chi nhánh là bắt buộc'),
  address: z.string().min(1, 'Địa chỉ là bắt buộc'),
  phone: z.string().min(1, 'Số điện thoại là bắt buộc'),
  email: z.string().email('Địa chỉ email không hợp lệ'),
});

interface EditBranchDialogProps {
  branch: Branch;
  onUpdate: (branch: UpdateBranchDto) => void;
  isUpdatePending?: boolean;
}

export function EditBranchDialog({
  branch,
  onUpdate,
  isUpdatePending = false,
}: EditBranchDialogProps) {
  const [open, setOpen] = useState(false);

  const form = useForm<z.infer<typeof branchSchema>>({
    resolver: zodResolver(branchSchema),
    defaultValues: {
      name: branch.name,
      address: branch.address,
      phone: branch.phone,
      email: branch.email,
    },
  });

  useEffect(() => {
    if (!isUpdatePending) {
      setOpen(false);
    }
  }, [isUpdatePending]);

  const onSubmit = (data: z.infer<typeof branchSchema>) => {
    if (
      data.name === branch.name &&
      data.address === branch.address &&
      data.phone === branch.phone &&
      data.email === branch.email
    ) {
      toast.info('Không có thay đổi nào được thực hiện');
      setOpen(false);
      return;
    }

    const updatedBranch: UpdateBranchDto = {
      id: branch.id,
      name: data.name,
      address: data.address,
      phone: data.phone,
      email: data.email,
    };

    onUpdate(updatedBranch);
  };

  const handleOpenChange = (isOpen: boolean) => {
    setOpen(isOpen);
    if (!isOpen) {
      form.reset({
        name: branch.name,
        address: branch.address,
        phone: branch.phone,
        email: branch.email,
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button variant="default" size="sm" className="gap-1" onClick={() => setOpen(true)}>
          <Edit className="h-4 w-4" />
          <span className="md:block hidden">Chỉnh Sửa</span>
        </Button>
      </DialogTrigger>
      <DialogContent className="w-full max-w-[90vw] sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Chỉnh Sửa Chi Nhánh</DialogTitle>
          <DialogDescription>Cập nhật thông tin chi nhánh bên dưới.</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <div className="grid gap-4 py-4">
              {/* Tên Chi Nhánh */}
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tên Chi Nhánh</FormLabel>
                    <FormControl>
                      <Input {...field} className="w-full" required />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Địa Chỉ */}
              <FormField
                control={form.control}
                name="address"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Địa Chỉ</FormLabel>
                    <FormControl>
                      <Textarea {...field} className="w-full" required />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Số Điện Thoại */}
              <FormField
                control={form.control}
                name="phone"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Số Điện Thoại</FormLabel>
                    <FormControl>
                      <Input {...field} className="w-full" required />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Email */}
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Email</FormLabel>
                    <FormControl>
                      <Input {...field} className="w-full" required />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <DialogFooter className="flex justify-end gap-2">
              <Button type="submit" disabled={isUpdatePending}>
                <Loader state={isUpdatePending} textLoading="Đang cập nhật...">
                  Cập Nhật
                </Loader>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
