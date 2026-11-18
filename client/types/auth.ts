export interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  roleName?: string;
  branchId?: string;
  accessToken?: string;
  refreshToken?: string;
}

export interface RegisterUser {
  fullName: string;
  email: string;
  password?: string;
}

export interface LoginGoogleUser {
  name: string;
  email: string;
}

export interface ResetPasswordDto {
  email: string;
  newPassword: string;
  confirmPassword: string;
}
