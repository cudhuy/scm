export interface Role {
  id: string;
  roleName: string;
  description: string;
}

export interface CreateNewRoleDto {
  roleName: string;
  description: string;
}

export interface UpdateRoleDto {
  id: string;
  roleName: string;
  description: string;
}

export interface RoleCount {
  roleName: string;
  userCount: number;
}