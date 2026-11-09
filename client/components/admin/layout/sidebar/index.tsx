import React from 'react';
import { Sidebar, SidebarRail } from '@/components/ui/sidebar';
import SidebarHeaderAdmin from '@/components/admin/layout/sidebar/sidebar-header';
import SidebarContentAdmin from '@/components/admin/layout/sidebar/sidebar-content';
import SidebarFooterAdmin from '@/components/admin/layout/sidebar/sidebar-footer';

export default function SidebarAdmin({ role }: { role: string }) {
  return (
    <Sidebar collapsible="icon">
      <SidebarHeaderAdmin role={role} />
      <SidebarContentAdmin role={role} />
      <SidebarFooterAdmin />
      <SidebarRail />
    </Sidebar>
  );
}
