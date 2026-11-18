/* eslint-disable @typescript-eslint/no-explicit-any */
export interface CustomError extends Error {
  status?: number;
  data?: any;
}
