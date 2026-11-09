'use client';

import type React from 'react';

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Trash2 } from 'lucide-react';
import { DialogClose } from '@radix-ui/react-dialog';
import Loader from '@/components/global/loader';
import { cn } from '@/lib/utils';

interface DeleteDialogProps {
  entityId: string;
  entityName?: string;
  entityType?: string;
  onDelete: (entityId: string) => void;
  isDeletePending?: boolean;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
  children?: React.ReactNode;
}

export function DeleteDialog({
  entityId,
  entityName,
  onDelete,
  entityType = 'item',
  isDeletePending = false,
  open,
  onOpenChange,
  children,
}: DeleteDialogProps) {
  const handleDelete = () => {
    onDelete(entityId);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogTrigger asChild>
        {children ? (
          children
        ) : (
          <Button
            variant="outline"
            size="sm"
            className={cn(
              'bg-white/80 dark:bg-gray-900/80 backdrop-blur-xs',
              'text-red-500 hover:text-red-600',
              'hover:bg-red-50 dark:hover:bg-red-500/20',
              'border-red-200 dark:border-red-800/30',
              'p-2 sm:p-3',
            )}
          >
            <Trash2 className="h-4 w-4 sm:h-5 sm:w-5 mr-1" />
            Xóa
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px] max-w-[90vw] p-4 sm:p-6">
        <DialogHeader>
          <DialogTitle className="flex items-center space-x-2 text-base sm:text-lg mt-4 md:mt-0">
            <Trash2 className="h-5 w-5 sm:h-6 sm:w-6 text-red-500 text-sm sm:text-base" />
            <span className="text-sm">Bạn có chắc chắn muốn xóa {entityType} này không?</span>
          </DialogTitle>
          <DialogDescription className="text-sm">
            Hành động này sẽ xóa vĩnh viễn {entityType}{' '}
            <span className="font-bold text-red-500 ">{entityName ? entityName : ''}</span>. Hành
            động này không thể hoàn tác.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className="flex flex-col sm:flex-row gap-2 sm:gap-0 mt-4 sm:mt-0">
          <DialogClose asChild>
            <Button variant="outline" className="w-full sm:w-auto">
              Hủy
            </Button>
          </DialogClose>
          <Button
            onClick={handleDelete}
            variant="destructive"
            className="w-full sm:w-auto"
            disabled={isDeletePending}
          >
            <Loader state={isDeletePending} textLoading="Đang xóa...">
              Xóa
            </Loader>
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
