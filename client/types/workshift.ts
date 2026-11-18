export interface WorkShift {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  duration: string;
  gracePeriod: number;
}

export interface CreateNewWorkShiftDto {
  name: string;
  startTime: string;
  endTime: string;
  duration: string;
  gracePeriod: number;
}

export interface UpdateWorkShiftDto {
  id: string;
  startTime: string;
  endTime: string;
  gracePeriod: number;
}

export interface Shift {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  graceMinutes: number;
}

