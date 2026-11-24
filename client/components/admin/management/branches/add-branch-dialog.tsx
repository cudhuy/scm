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
import { PlusCircle } from 'lucide-react';
import Loader from '@/components/global/loader';
import { CreateNewBranchDto } from '@/types/branch';

const branchSchema = z.object({
  name: z.string().min(1, 'Tên chi nhánh là bắt buộc'),
  address: z.string().min(1, 'Địa chỉ là bắt buộc'),
  phone: z.string().min(1, 'Số điện thoại là bắt buộc'),
  email: z.string().email('Địa chỉ email không hợp lệ'),
});

interface AddBranchDialogProps {
  onCreate: (branch: CreateNewBranchDto) => void;
  isCreatePending?: boolean;
}

export function AddBranchDialog({ onCreate, isCreatePending = false }: AddBranchDialogProps) {
  const [open, setOpen] = useState(false);

  const form = useForm<z.infer<typeof branchSchema>>({
    resolver: zodResolver(branchSchema),
    defaultValues: {
      name: '',
      address: '',
      phone: '',
      email: '',
    },
  });

  useEffect(() => {
    if (!isCreatePending) {
      setOpen(false);
    }
  }, [isCreatePending]);

  const onSubmit = (data: z.infer<typeof branchSchema>) => {
    const newBranch: CreateNewBranchDto = {
      name: data.name,
      address: data.address,
      phone: data.phone,
      email: data.email,
    };

    onCreate(newBranch);
  };

  const handleOpenChange = (isOpen: boolean) => {
    setOpen(isOpen);
    if (!isOpen) {
      form.reset({
        name: '',
        address: '',
        phone: '',
        email: '',
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button className="gap-2">
          <PlusCircle className="h-4 w-4" />
          Thêm Mới
        </Button>
      </DialogTrigger>
      <DialogContent className="w-full max-w-[90vw] sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Thêm Chi Nhánh Mới</DialogTitle>
          <DialogDescription>Tạo một chi nhánh mới cho hệ thống.</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <div className="space-y-4">
              {/* Tên Chi Nhánh */}
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tên Chi Nhánh</FormLabel>
                    <FormControl>
                      <Input placeholder="Nhập tên chi nhánh" {...field} className="w-full" />
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
                      <Textarea
                        placeholder="Nhập địa chỉ chi nhánh"
                        {...field}
                        className="w-full"
                      />
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
                      <Input placeholder="Nhập số điện thoại" {...field} className="w-full" />
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
                      <Input placeholder="Nhập địa chỉ email" {...field} className="w-full" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <DialogFooter className="flex justify-end gap-2">
              <Button type="submit" disabled={isCreatePending}>
                <Loader state={isCreatePending} textLoading="Đang tạo...">
                  Thêm Mới
                </Loader>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
