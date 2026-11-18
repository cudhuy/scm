export interface MembershipRank {
  id: string;
  name: string;
  description: string;
}

export interface CreateMembershipRankDto {
  name: string;
  description: string;
}

export interface UpdateMembershipRankDto {
  id: string;
  name: string;
  description: string;
}

export interface MembershipRankCount {
  totalCustomer: string;
  totalCustomerRankRegular: string;
  totalCustomerVip: string;
  totalCustomerStar: string;
}
