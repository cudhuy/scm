export interface UserType {
  id: string;
  name: string;
  email: string;
  avatar: string;
  accessToken: string;
  refreshToken: string;
  subId: string;
}

export type UserResponseType = {
  id: string;
  name: string;
  email: string;
  avatar: string;
  access_token: string;
  refresh_token: string;
  sub_id: string;
};

export interface UserProfile {
  name: string;
  email: string;
  roleName: string;
  avatar?: string;
}
