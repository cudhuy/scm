export interface Branch {
  id: string;
  name: string;
  address: string;
  phone: string;
  email: string;
  type: 'Main' | 'Sub';
}

export interface CreateNewBranchDto {
  name: string;
  address: string;
  phone: string;
  email: string;
}

export interface UpdateBranchDto {
  id: string;
  name?: string;
  address?: string;
  phone?: string;
  email?: string;
}
