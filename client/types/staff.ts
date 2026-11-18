export interface Staff {
  userId: string;
  fullName: string;
  email: string;
  phone: string;
  dob?: string;
  avatar?: string;
  roleName: string;
  description?: string;
  status: string;
  salaryBase: number;
  title: string;
  type: 'Lương Theo Giờ' | 'Lương Cứng';
  branchName: string;
  branchAddress: string;
  typeBranch: string;
  isDeleted: boolean;
}

export interface CreateNewStaffDto {
  fullname: string;
  email: string;
  phone: string;
  isStaff: boolean;
  description?: string;
  title?: string;
  type?: string;
  salaryBase: number;
  branchId: string;
}

export interface UpdateStaffDto {
  userId: string;
  title?: string;
  type?: string;
  status?: string;
  salaryBase: number;
  branchId: string;
}

export interface GetStaffsResponse {
  items: Staff[];
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}
