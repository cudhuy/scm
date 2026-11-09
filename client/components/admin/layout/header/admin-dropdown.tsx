'use client';

import { useState } from 'react';
import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { LogOut, User, Lock, Clock } from 'lucide-react';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { signOut } from 'next-auth/react';

interface AdminDropdownProps {
  userName?: string;
  userEmail?: string;
  userImage?: string;
  userRole?: string;
}
export function AdminDropdown({
  userName = 'Tommy Nguyen Dev',
  userEmail = 'tommynguyen.dev@gmail.com',
  userImage = 'https://github.com/shadcn.png',
  userRole = 'Unknown',
}: AdminDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);

  const menuItems = [
    {
      label: 'Thông tin cá nhân',
      icon: User,
      href: `/${userRole}/profile`,
    },
    {
      label: 'Thay đổi mật khẩu',
      icon: Lock,
      href: `/${userRole}/change-password`,
    },
    ...(userRole === 'staff'
      ? [
          {
            label: 'Chấm Công',
            icon: Clock,
            href: `/${userRole}/timekeeping`,
          },
        ]
      : []),

    // {
    //   label: 'Settings',
    //   icon: Settings,
    //   href: `/${userRole}/settings`,
    // },
    // {
    //   label: 'Support',
    //   icon: HelpCircle,
    //   href: `/${userRole}/support`,
    // },
  ];

  return (
    <DropdownMenu open={isOpen} onOpenChange={setIsOpen}>
      <DropdownMenuTrigger asChild>
        <div className="flex items-center gap-2 cursor-pointer group">
          <Avatar className="h-9 w-9 transition-all border-2 border-transparent group-hover:border-primary/20">
            <AvatarImage src={userImage} alt={userName} />
            <AvatarFallback>
              {userName
                .split(' ')
                .map((name) => name[0])
                .join('')}
            </AvatarFallback>
          </Avatar>
        </div>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-56">
        <div className="flex items-center justify-start gap-2 p-2">
          <div className="flex h-10 w-10 shrink-0 select-none items-center justify-center rounded-full bg-muted">
            <Avatar className="h-8 w-8">
              <AvatarImage src={userImage} alt={userName} />
              <AvatarFallback>
                {userName
                  .split(' ')
                  .map((name) => name[0])
                  .join('')}
              </AvatarFallback>
            </Avatar>
          </div>
          <div className="flex flex-col space-y-0.5">
            <DropdownMenuLabel className="p-0 text-sm font-medium leading-none">
              {userName}
            </DropdownMenuLabel>
            <p className="text-xs text-muted-foreground leading-none">{userEmail}</p>
          </div>
        </div>
        <DropdownMenuSeparator />
        <AnimatePresence>
          {isOpen && (
            <>
              {menuItems.map((item, index) => (
                <motion.div
                  key={item.label}
                  initial={{ opacity: 0, y: -8 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.05 }}
                >
                  <DropdownMenuItem asChild className="p-0">
                    <Link
                      href={item.href}
                      className="flex w-full items-center gap-2 px-2 py-1.5 cursor-pointer"
                    >
                      <item.icon className="h-4 w-4 text-muted-foreground" />
                      <span>{item.label}</span>
                    </Link>
                  </DropdownMenuItem>
                </motion.div>
              ))}
              <DropdownMenuSeparator />
              <motion.div
                initial={{ opacity: 0, y: -8 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: menuItems.length * 0.05 }}
              >
                <DropdownMenuItem
                  className="text-red-500 focus:text-red-500 focus:bg-red-50 dark:focus:bg-red-950/20 cursor-pointer"
                  onClick={() => signOut({ redirectTo: '/auth/login' })}
                >
                  <LogOut className="h-4 w-4" />
                  <span>Đăng Xuất</span>
                </DropdownMenuItem>
              </motion.div>
            </>
          )}
        </AnimatePresence>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
