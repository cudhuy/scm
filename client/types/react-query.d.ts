import { CustomError } from './type';

// types.ts
declare module '@tanstack/react-query' {
  interface Register {
    defaultError: CustomError;
  }
}
