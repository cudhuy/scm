'use client';

import { motion } from 'framer-motion';
import { Shield, LockKeyhole } from 'lucide-react';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import PasswordStrength from '@/components/admin/change-password/password-strength';
import PasswordForm from '@/components/admin/change-password/password-form';
import { useState } from 'react';
import { useSession } from 'next-auth/react';

const container = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1,
    },
  },
} as const;

const item = {
  hidden: { opacity: 0, y: 20 },
  show: {
    opacity: 1,
    y: 0,
    transition: {
      type: 'spring',
      stiffness: 300,
      damping: 30,
    },
  },
} as const;

export default function ChangePassword() {
  const { data: session } = useSession();
  const [newPassword, setNewPassword] = useState('');

  return (
    <div className="container max-w-6xl py-10">
      <motion.div
        className="mb-8 text-center"
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <div className="inline-flex items-center justify-center p-2 rounded-full bg-primary/10 mb-4">
          <LockKeyhole className="h-6 w-6 text-primary" />
        </div>
        <h1 className="text-3xl font-bold">Đổi mật khẩu</h1>
        <p className="text-muted-foreground mt-2 max-w-md mx-auto">
          Đảm bảo tài khoản của bạn luôn an toàn bằng cách chọn một mật khẩu mạnh
        </p>
      </motion.div>

      <motion.div
        className="grid gap-8 md:grid-cols-2"
        variants={container}
        initial="hidden"
        animate="show"
      >
        <motion.div className="space-y-6" variants={item}>
          <Alert className="border-primary/20 bg-primary/5">
            <Shield className="h-4 w-4 text-primary" />
            <AlertTitle>Mẹo bảo mật mật khẩu</AlertTitle>
            <AlertDescription className="mt-4">
              <ul className="list-disc list-inside space-y-1 text-sm">
                <li>Sử dụng một mật khẩu duy nhất cho mỗi tài khoản</li>
                <li>Tránh sử dụng thông tin cá nhân</li>
                <li>Cân nhắc sử dụng trình quản lý mật khẩu</li>
                <li>Thay đổi mật khẩu thường xuyên</li>
              </ul>
            </AlertDescription>
          </Alert>

          <PasswordStrength password={newPassword} />
        </motion.div>

        <motion.div variants={item}>
          <PasswordForm onPasswordChange={setNewPassword} email={session?.user?.email || ''} />
        </motion.div>
      </motion.div>
    </div>
  );
}
