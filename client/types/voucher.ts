import { MembershipRank } from "@/types/membership-rank";

export interface Voucher {
  voucherId: string;
  code: string;
  noOfVoucher: string;
  description: string;
  discountValue: string;
  discountType: number;
  usageCount: number;
  minOrder: string;
  startDate: string;
  expireDate: string;
  membershipRanks: MembershipRank[];
}

export interface CreateNewVoucherDto {
  code: string;
  noOfVoucher: number;
  description: string;
  discountValue: number;
  discountType: number;
  minOrder: number;
  startDate: string;
  expireDate: string;
  membershipRankIds: string[];
}

export interface UpdateVoucherDto {
  voucherId: string;
  noOfVoucher: number;
  description: string;
  discountValue: number;
  discountType: number;
  minOrder: number;
  startDate: string;
  expireDate: string;
  membershipRankIds: string[];
}

export interface CustomerVoucher {
  id: string;
  code: string;
  description: string;
  discountValue: number;
  discountType: number;
  expireDate: string;
  minOrder: number;
}