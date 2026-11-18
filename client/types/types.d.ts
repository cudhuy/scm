/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
import { DefaultSession } from 'next-auth';
import { JWT } from 'next-auth/jwt';

declare module 'next-auth' {
  interface User {
    name?: string;
    avatar?: string;
    roleName?: string;
    branchId?: string;
    accessToken?: string;
    refreshToken?: string;
  }

  interface Session {
    user: User & DefaultSession['user'];
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    id?: string;
    name?: string;
    email?: string;
    avatar?: string;
    roleName?: string;
    branchId?: string;
    accessToken?: string;
    refreshToken?: string;
  }
}
