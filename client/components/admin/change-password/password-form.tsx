'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Eye, EyeOff, Lock, Unlock, ShieldCheck, CheckCircle2, AlertCircle } from 'lucide-react';
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  CardFooter,
} from '@/components/ui/card';
import { cn } from '@/lib/utils';
import { ResetPasswordDto } from '@/types/auth';
import { useSimpleMutation } from '@/hooks/use-mutation-data';
import { userService } from '@/services/userService';

const formSchema = z
  .object({
    newPassword: z
      .string()
      .min(8, 'Mật khẩu phải dài ít nhất 8 ký tự')
      .regex(/[A-Z]/, 'Mật khẩu phải chứa ít nhất một chữ cái in hoa')
      .regex(/[a-z]/, 'Mật khẩu phải chứa ít nhất một chữ cái thường')
      .regex(/\d/, 'Mật khẩu phải chứa ít nhất một số')
      .regex(/[!@#$%^&*(),.?":{}|<>]/, 'Mật khẩu phải chứa ít nhất một ký tự đặc biệt'),
    confirmPassword: z.string().min(1, 'Vui lòng xác nhận mật khẩu mới'),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: 'Mật khẩu không khớp',
    path: ['confirmPassword'],
  });

interface PasswordFormProps {
  onPasswordChange?: (password: string) => void;
  email: string;
}

export default function PasswordForm({ onPasswordChange, email }: PasswordFormProps) {
  const [showPasswords, setShowPasswords] = useState({
    new: false,
    confirm: false,
  });

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      newPassword: '',
      confirmPassword: '',
    },
  });

  const newPassword = form.watch('newPassword');

  useEffect(() => {
    if (onPasswordChange) onPasswordChange(newPassword || '');
  }, [newPassword, onPasswordChange]);

  const {
    mutate,
    isPending: isLoading,
    isSuccess,
  } = useSimpleMutation({
    mutationKey: ['resetPassword'],
    mutationFn: async (values: z.infer<typeof formSchema>) => {
      if (!email) throw new Error('Email not found in session');

      const resetPasswordData: ResetPasswordDto = {
        email: email,
        newPassword: values.newPassword,
        confirmPassword: values.confirmPassword,
      };

      return await userService.resetPassword(resetPasswordData);
    },
    onSuccess: () => {
      form.reset();
      if (onPasswordChange) onPasswordChange('');
    },
    onError: (error) => console.error('Password reset error:', error),
    successMessage: 'Đổi mật khẩu thành công!',
  });

  const togglePasswordVisibility = (field: keyof typeof showPasswords) => {
    setShowPasswords((prev) => ({ ...prev, [field]: !prev[field] }));
  };

  const handleSubmit = (values: z.infer<typeof formSchema>) => {
    mutate(values);
  };

  const passwordsMatch = form.watch('newPassword') === form.watch('confirmPassword');
  const bothPasswordsEntered = !!form.watch('newPassword') && !!form.watch('confirmPassword');

  return (
    <Card className="border-0 shadow-md overflow-hidden">
      <div className="absolute top-0 left-0 w-full" />

      <CardHeader>
        <CardTitle>Cập nhật mật khẩu</CardTitle>
        <CardDescription>Vui lòng chọn mật khẩu mới của bạn</CardDescription>
      </CardHeader>

      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
            <div className="space-y-4">
              <FormField
                control={form.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      <span className="flex items-center gap-2">
                        <Unlock className="h-4 w-4 text-primary" />
                        Mật khẩu mới
                      </span>
                    </FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type={showPasswords.new ? 'text' : 'password'}
                          className={cn(
                            'pr-10 transition-all',
                            field.value && 'border-primary/50 focus-visible:ring-primary/20',
                          )}
                          {...field}
                        />
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent"
                          onClick={() => togglePasswordVisibility('new')}
                        >
                          {showPasswords.new ? (
                            <EyeOff className="h-4 w-4 text-muted-foreground" />
                          ) : (
                            <Eye className="h-4 w-4 text-muted-foreground" />
                          )}
                        </Button>
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="confirmPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      <span className="flex items-center gap-2">
                        <Lock className="h-4 w-4 text-primary" />
                        Xác nhận mật khẩu mới
                      </span>
                    </FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type={showPasswords.confirm ? 'text' : 'password'}
                          className={cn(
                            'pr-10 transition-all',
                            field.value && 'border-primary/50 focus-visible:ring-primary/20',
                            bothPasswordsEntered &&
                              !passwordsMatch &&
                              'border-red-500 focus-visible:ring-red-500',
                          )}
                          {...field}
                        />
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent"
                          onClick={() => togglePasswordVisibility('confirm')}
                        >
                          {showPasswords.confirm ? (
                            <EyeOff className="h-4 w-4 text-muted-foreground" />
                          ) : (
                            <Eye className="h-4 w-4 text-muted-foreground" />
                          )}
                        </Button>
                      </div>
                    </FormControl>
                    <FormMessage />

                    {bothPasswordsEntered && !passwordsMatch && (
                      <motion.div
                        className="flex items-center gap-1 text-sm text-red-500 mt-1"
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                      >
                        <AlertCircle className="h-4 w-4" />
                        Mật khẩu không khớp
                      </motion.div>
                    )}

                    {bothPasswordsEntered && passwordsMatch && (
                      <motion.div
                        className="flex items-center gap-1 text-sm text-green-500 mt-1"
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                      >
                        <CheckCircle2 className="h-4 w-4" />
                        Mật khẩu khớp
                      </motion.div>
                    )}
                  </FormItem>
                )}
              />
            </div>

            <CardFooter className="px-0 pt-2">
              <Button
                type="submit"
                disabled={isLoading || isSuccess}
                className={cn(
                  'w-full bg-linear-to-r from-primary to-primary/80 text-primary-foreground hover:from-primary/90 hover:to-primary/70 shadow-md hover:shadow-lg transition-all',
                  isSuccess && 'bg-green-500',
                )}
              >
                <AnimatePresence mode="wait">
                  {isLoading ? (
                    <motion.div
                      key="loading"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      exit={{ opacity: 0 }}
                      className="flex items-center"
                    >
                      <motion.div
                        animate={{ rotate: 360 }}
                        transition={{
                          duration: 1,
                          repeat: Number.POSITIVE_INFINITY,
                          ease: 'linear',
                        }}
                        className="mr-2"
                      >
                        <ShieldCheck className="h-4 w-4" />
                      </motion.div>
                      Đang cập nhật...
                    </motion.div>
                  ) : isSuccess ? (
                    <motion.div
                      key="success"
                      initial={{ scale: 0.8, opacity: 0 }}
                      animate={{ scale: 1, opacity: 1 }}
                      exit={{ scale: 1.2, opacity: 0 }}
                      className="flex items-center"
                    >
                      <CheckCircle2 className="mr-2 h-4 w-4" />
                      Đã cập nhật mật khẩu!
                    </motion.div>
                  ) : (
                    <motion.span
                      key="default"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      exit={{ opacity: 0 }}
                    >
                      Cập nhật mật khẩu
                    </motion.span>
                  )}
                </AnimatePresence>
              </Button>
            </CardFooter>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
