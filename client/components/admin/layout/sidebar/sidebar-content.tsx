/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import React from 'react';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
  SidebarMenuSub,
  SidebarMenuSubItem,
  SidebarContent,
  SidebarSeparator,
} from '@/components/ui/sidebar';
import { ChevronRight } from 'lucide-react';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { ROLE_MENU_DATA } from '@/constants/role-menu-sidebar';

export default function SidebarContentAdmin({ role }: { role: string }) {
  const pathname = usePathname();

  const data = ROLE_MENU_DATA[role.toLowerCase()] || {
    navMain: [],
    projects: [],
    roleLabel: 'Unknown Role',
  };

  // Check if a submenu item is active
  const isSubmenuActive = (item: any) => {
    if (item.items) {
      return item.items.some((subItem: any) => pathname === subItem.url);
    }
    return false;
  };

  return (
    <SidebarContent>
      <SidebarGroup>
        <SidebarGroupLabel className="text-sm">{data.roleLabel}</SidebarGroupLabel>
        <SidebarMenu>
          {data.navMain?.map((item) =>
            item.items ? (
              <Collapsible
                key={item.title}
                asChild
                defaultOpen={isSubmenuActive(item) || item.isActive}
                className="group/collapsible"
              >
                <SidebarMenuItem>
                  <CollapsibleTrigger asChild>
                    <SidebarMenuButton>
                      {item.icon && <item.icon className="h-5 w-5" />}
                      <span>{item.title}</span>
                      <ChevronRight className="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90" />
                    </SidebarMenuButton>
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {item.items?.map((subItem) => (
                        <SidebarMenuSubItem key={subItem.title}>
                          <Link
                            href={subItem.url}
                            className={`w-full text-sm px-4 py-1 rounded-md flex items-center transition-colors hover:bg-slate-100 dark:hover:bg-slate-800 ${
                              pathname === subItem.url
                                ? 'font-medium bg-slate-50 dark:bg-slate-900'
                                : ''
                            }`}
                          >
                            <span>{subItem.title}</span>
                          </Link>
                        </SidebarMenuSubItem>
                      ))}
                    </SidebarMenuSub>
                  </CollapsibleContent>
                </SidebarMenuItem>
              </Collapsible>
            ) : (
              <SidebarMenuItem key={item.title}>
                <Link href={item.url} className="w-full">
                  <SidebarMenuButton
                    isActive={pathname === item.url}
                    className={pathname === item.url ? 'font-medium' : ''}
                  >
                    {item.icon && <item.icon className="h-5 w-5" />}
                    <span>{item.title}</span>
                  </SidebarMenuButton>
                </Link>
              </SidebarMenuItem>
            ),
          )}
        </SidebarMenu>
      </SidebarGroup>

      <SidebarSeparator className="my-2" />
    </SidebarContent>
  );
}
