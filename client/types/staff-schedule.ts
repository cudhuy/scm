export interface StaffScheduleResponse {
  scheduleDetail: ScheduleDetail[];
  branchId: string;
  workDate: string;
}

export interface ScheduleDetail {
  employeeId: string;
  employeeName: string;
  workShiftId: string;
  employeeShiftId: string;
  workDate: string;
  status: string;
  startAmount: number;
  endAmount: number;
  checkInDate?: string;
  checkOutDate?: string;
}

export interface CreateEmployeeShiftDto {
  workShiftId: string;
  employeeId: string;
  workDate: string;
  branchId: string;
}

export interface StaffResponse {
  id: string;
  fullName: string;
}
