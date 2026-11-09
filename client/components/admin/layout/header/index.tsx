'use client';

import { motion } from 'framer-motion';
import { SidebarTrigger } from '@/components/ui/sidebar';
import { capitalize } from '@/lib/utils';
import { ThemeToggle } from '@/components/admin/layout/header/theme-toggle';
import { useSession } from 'next-auth/react';
import { TimeDisplay } from './time-display';
import { Breadcrumb } from './breadcrumb';
import { AdminDropdown } from './admin-dropdown';

export default function HeaderAdmin() {
  const session = useSession();
  const userName = capitalize(session?.data?.user?.name || '');
  const userEmail = session?.data?.user?.email;
  const userImage = session?.data?.user?.avatar;
  const userRole = session?.data?.user?.roleName?.toLowerCase();

  return (
    <motion.div
      initial={{ y: -20, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.3 }}
      className="sticky top-0 z-10 flex h-16 items-center justify-between gap-4 border-b bg-background/95 px-4 backdrop-blur-sm supports-backdrop-filter:bg-background/60"
    >
      <div className="flex items-center gap-3 py-4">
        <SidebarTrigger className="text-muted-foreground hover:text-foreground" />

        <div className="hidden md:flex items-center gap-3 ml-2">
          <TimeDisplay />

          <div className="hidden lg:block ml-2">
            <Breadcrumb />
          </div>
        </div>
      </div>

      <div className="flex items-center gap-2">
        <ThemeToggle />

        <AdminDropdown
          userName={userName || ''}
          userEmail={userEmail || ''}
          userImage={userImage || ''}
          userRole={userRole || ''}
        />
      </div>
    </motion.div>
  );
}
