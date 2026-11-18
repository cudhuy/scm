export interface SalaryResponse {
  id: string;
  employeeId: string;
  employeeName: string;
  month: number;
  year: number;
  totalHours: number;
  bonus: number;
  penalty: number;
  netSalary: number;
  totalNumberOfShifts: number;
  totalLateShifts: number;
  totalAbsentShifts: number;
  totalNotCompleteShifts: number;
  salaryBase: number;
  isConfirmed: boolean;
  branchId: string;
}

export interface SalaryCalculationParams {
  branchId: string;
  month: number;
  year: number;
  token?: string;
}

export interface ConfirmSalaryRequest {
  month: number;
  year: number;
  employeeId: string;
  employeeName: string;
  totalHour: number;
  bonus: number;
  penalty: number;
  netSalary: number;
  totalNumberOfShifts: number;
  totalLateShifts: number;
  totalAbsentShifts: number;
  totalNotCompleteShifts: number;
  salaryBase: number;
  isConfirmed?: boolean;
  branchId: string;
}

export interface SendSalarySlipRequest {
  salarySlipIds: string[];
}
