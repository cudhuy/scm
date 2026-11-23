/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
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
} from '@/components/ui/form';
import { Plus } from 'lucide-react';
import Loader from '@/components/global/loader';
import { CreateSystemNotificationDto, CreateUserNotificationDto } from '@/types/notification';
import { MembershipRank } from '@/types/membership-rank';
import RankBadge from '@/components/global/rank-badge';

// Schema giữ nguyên vì ranks trong DTO là mảng các id (string)
export const notificationSchema = z.object({
  title: z.string().min(1, 'Tiêu đề thông báo là bắt buộc'),
  content: z.string().min(1, 'Nội dung thông báo là bắt buộc'),
  rankIds: z.array(z.string()).optional(),
  priority: z.enum(['0', '1', '2']).optional(),
});

// Priority levels với tiếng Việt
const priorityLevels = [
  {
    displayId: 'high',
    backendValue: '2',
    label: 'Đặc biệt',
    color: 'bg-linear-to-r from-red-700 to-red-600',
  },
  {
    displayId: 'medium',
    backendValue: '1',
    label: 'Ưu đãi',
    color: 'bg-linear-to-r from-yellow-700 to-yellow-500',
  },
  {
    displayId: 'low',
    backendValue: '0',
    label: 'Thường xuyên',
    color: 'bg-linear-to-r from-blue-700 to-blue-500',
  },
];

interface AddNotificationDialogProps {
  type: 'user' | 'system';
  ranks: MembershipRank[];
  isLoadingRanks: boolean;
  ranksError: any;
  onCreate: (data: CreateSystemNotificationDto | CreateUserNotificationDto) => void;
  isCreatePending?: boolean;
}

export function AddNotificationDialog({
  type,
  ranks,
  isLoadingRanks,
  ranksError,
  onCreate,
  isCreatePending = false,
}: AddNotificationDialogProps) {
  const [open, setOpen] = useState(false);

  const form = useForm<z.infer<typeof notificationSchema>>({
    resolver: zodResolver(notificationSchema),
    defaultValues: {
      title: '',
      content: '',
      rankIds: [],
      priority: type === 'system' ? '1' : undefined,
    },
  });

  useEffect(() => {
    if (!isCreatePending) {
      setOpen(false);
    }
  }, [isCreatePending]);

  const onSubmit = (values: z.infer<typeof notificationSchema>) => {
    onCreate(values);
  };

  const handleOpenChange = (isOpen: boolean) => {
    setOpen(isOpen);
    if (!isOpen) {
      form.reset({
        title: '',
        content: '',
        rankIds: [],
        priority: type === 'system' ? '1' : undefined,
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button
          size="lg"
          className="bg-linear-to-r from-indigo-500 to-purple-500 hover:from-indigo-600 hover:to-purple-600 text-white border-0 gap-2"
        >
          <Plus className="h-5 w-5" />
          Tạo thông báo mới
        </Button>
      </DialogTrigger>
      <DialogContent className="max-w-3xl w-[95%] sm:w-full">
        <DialogHeader>
          <DialogTitle>
            {type === 'user' ? 'Thông báo người dùng' : 'Thông báo hệ thống'}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tiêu đề</FormLabel>
                  <FormControl>
                    <Input placeholder="Nhập tiêu đề thông báo" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="content"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nội dung</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Nhập nội dung thông báo..."
                      className="min-h-[150px] resize-none"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {type === 'user' && (
              <FormField
                control={form.control}
                name="rankIds"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Cấp bậc mục tiêu</FormLabel>
                    <FormControl>
                      {isLoadingRanks ? (
                        <div>Đang tải cấp bậc...</div>
                      ) : ranksError ? (
                        <div>
                          Lỗi khi tải cấp bậc: {ranksError?.message || 'Lỗi không xác định'}
                        </div>
                      ) : (
                        <div className="flex flex-wrap gap-2">
                          {ranks.map((rank) => (
                            <div
                              key={rank.id}
                              className={`cursor-pointer rounded-md p-1 ${
                                field.value?.includes(rank.id)
                                  ? 'ring-1 ring-offset-1 ring-indigo-500'
                                  : ''
                              }`}
                              onClick={() => {
                                const newRanks = field.value?.includes(rank.id)
                                  ? field.value.filter((r: string) => r !== rank.id)
                                  : [...(field.value || []), rank.id];
                                field.onChange(newRanks);
                              }}
                            >
                              <RankBadge rank={rank.name} size="sm" />
                            </div>
                          ))}
                        </div>
                      )}
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            {type === 'system' && (
              <FormField
                control={form.control}
                name="priority"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Mức độ ưu tiên</FormLabel>
                    <FormControl>
                      <div className="flex flex-wrap gap-2">
                        {priorityLevels.map((priority) => (
                          <Badge
                            key={priority.displayId}
                            variant={field.value === priority.backendValue ? 'default' : 'outline-solid'}
                            className={`cursor-pointer transition-all hover:scale-105 ${
                              field.value === priority.backendValue
                                ? `${priority.color} text-white`
                                : ''
                            }`}
                            onClick={() => {
                              field.onChange(priority.backendValue);
                            }}
                          >
                            {priority.label}
                          </Badge>
                        ))}
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <DialogFooter className="flex flex-col sm:flex-row gap-2">
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
                disabled={isCreatePending}
                className="w-full sm:w-auto bg-linear-to-r from-indigo-500 to-purple-500 hover:from-indigo-600 hover:to-purple-600 text-white"
              >
                <Loader state={isCreatePending} textLoading="Đang tạo...">
                  Tạo thông báo
                </Loader>
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
