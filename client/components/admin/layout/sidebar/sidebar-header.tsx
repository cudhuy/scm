'use client';

import React from 'react';
import {
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
  SidebarHeader,
} from '@/components/ui/sidebar';
import { CoffeeIcon } from 'lucide-react';
import Link from 'next/link';
import { translateRole } from '@/lib/utils';
import Image from 'next/image';

export default function SidebarHeaderAdmin({ role }: { role: string }) {
  return (
    <SidebarHeader>
      <SidebarMenu>
        <SidebarMenuItem>
          <SidebarMenuButton
            asChild
            size="lg"
            className="group transition-all duration-200 hover:bg-sidebar-accent/80 
              data-[state=open]:bg-linear-to-r data-[state=open]:from-sidebar-accent 
              data-[state=open]:to-sidebar-accent/80"
          >
            <Link href={`/${role}`} className="flex items-center gap-3 p-2">
              <div
                className="relative flex aspect-square size-8 items-center justify-center 
                rounded-xl 
                shadow-md transition-all group-hover:scale-105"
              >
                <CoffeeIcon
                  className="size-5 text-white transition-transform 
                  group-hover:rotate-12"
                />
                <Image
                  src="/logo-header-admin.png"
                  alt="Logo"
                  width={32}
                  height={32}
                  className="absolute inset-0 m-auto h-8 w-8 rounded-full object-cover 
                  transition-transform group-hover:scale-105"
                />
              </div>

              <div className="grid flex-1 text-left">
                <span
                  className="truncate text-sm bg-linear-to-r 
                  from-foreground to-foreground/80 bg-clip-text text-transparent 
                  group-hover:from-primary group-hover:to-primary/80 transition-colors font-bold"
                >
                  QUẢN LÝ XÓM CAFE
                </span>
                <div className=" flex items-center gap-1">
                  <span
                    className="truncate text-xs font-medium text-primary/70 
                    group-hover:text-primary transition-colors"
                  >
                    {translateRole(role)}
                  </span>
                  <span
                    className="h-1 w-1 rounded-full bg-primary/50 
                    group-hover:bg-primary animate-pulse"
                  />
                </div>
              </div>
            </Link>
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarHeader>
  );
}
