/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { usePathname } from 'next/navigation';
import Link from 'next/link';
import { ChevronRight, Home } from 'lucide-react';
import { capitalize, translateRole } from '@/lib/utils';
import { useSession } from 'next-auth/react';
import { ROLE_MENU_DATA } from '@/constants/role-menu-sidebar';
import { useMemo } from 'react';

// Tìm menu item theo path
const findMenuItem = (path: string, roleData: any, index: number, segments: string[]): string => {
  const userRoleLower = segments[0];

  // Trường hợp đặc biệt: segment đầu tiên là role
  if (index === 0 && path === userRoleLower) {
    return roleData?.roleLabel || capitalize(path);
  }

  // Tạo full path để so sánh
  const fullPath = '/' + segments.slice(0, index + 1).join('/');

  // Tìm trong navMain
  for (const item of roleData?.navMain || []) {
    if (item.url === fullPath) {
      return item.title;
    }
    // Tìm trong sub-items
    if (item.items) {
      const subItem = item.items.find((sub: any) => sub.url === fullPath);
      if (subItem) {
        return subItem.title;
      }
    }
  }

  // Fallback: trả về tên segment đã được format
  return capitalize(path.replace(/-/g, ' '));
};

export function Breadcrumb() {
  const pathname = usePathname();
  const { data: session } = useSession();
  const userRole = session?.user?.roleName?.toLowerCase() || '';

  // Memoize pathSegments và roleData để tránh tính toán lại
  const { pathSegments, roleData } = useMemo(() => {
    const segments = pathname.split('/').filter(Boolean);
    const roleData = ROLE_MENU_DATA[userRole] || { navMain: [], roleLabel: '' };
    return { pathSegments: segments, roleData };
  }, [pathname, userRole]);

  // Không render nếu không có path segments
  if (!pathSegments.length || !roleData) {
    return null;
  }

  return (
    <nav
      aria-label="Breadcrumb"
      className="hidden lg:flex items-center text-sm text-muted-foreground"
    >
      <ol className="flex items-center space-x-1">
        <li>
          <Link href={`/${userRole}`} className="flex items-center hover:text-foreground">
            <Home className="h-4 w-4" />
            <span className="sr-only">Home</span>
          </Link>
        </li>

        {pathSegments.map((segment, index) => {
          const href = `/${pathSegments.slice(0, index + 1).join('/')}`;
          const isLast = index === pathSegments.length - 1;
          const title = findMenuItem(segment, roleData, index, pathSegments);

          return (
            <li key={href} className="flex items-center">
              <ChevronRight className="h-4 w-4 mx-1" />
              {isLast ? (
                <span className="font-medium text-foreground">{title}</span>
              ) : (
                <Link href={href} className="hover:text-foreground">
                  {translateRole(title)}
                </Link>
              )}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}
