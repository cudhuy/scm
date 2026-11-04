import { z } from 'zod';
import { SALARY_TYPES } from './constants/salary-types';
import { ACCEPTED_IMAGE_TYPES, MAX_FILE_SIZE } from './constants/image';

export const voucherSchema = z.object({
  code: z.string().nonempty({ message: 'Mã voucher là bắt buộc' }),
  description: z.string().nonempty({ message: 'Mô tả là bắt buộc' }),
  discountType: z.enum(['1', '2']),
  discountValue: z.string().nonempty({ message: 'Giá trị giảm giá là bắt buộc' }),
  noOfVoucher: z.string().nonempty({ message: 'Số lượng voucher là bắt buộc' }),
  minOrder: z.string().nonempty({ message: 'Đơn hàng tối thiểu là bắt buộc' }),
  startDate: z.date({ required_error: 'Ngày bắt đầu là bắt buộc' }),
  expireDate: z.date({ required_error: 'Ngày hết hạn là bắt buộc' }),
  membershipRankIds: z
    .array(z.string())
    .nonempty({ message: 'Phải chọn ít nhất một cấp bậc thành viên' }),
});

export const addStaffSchema = z.object({
  fullName: z.string().min(1, 'Họ và tên là bắt buộc'),
  email: z.string().email('Địa chỉ email không hợp lệ'),
  phone: z
    .string()
    .min(1, 'Số điện thoại là bắt buộc')
    .regex(/(0[3-9][0-9]{8})\b/, 'Số điện thoại không hợp lệ. Ví dụ: 0982641707'),
  isStaff: z.boolean(),
  branchId: z.string().min(1, 'Chi nhánh là bắt buộc'),
  title: z.string().min(1, 'Chức vụ là bắt buộc'),
  type: z.enum([SALARY_TYPES.HOURLY, SALARY_TYPES.FIXED], {
    required_error: 'Hình thức lương là bắt buộc',
    invalid_type_error: 'Phải là "Lương Theo Giờ" hoặc "Lương Cứng"',
  }),
  salaryBase: z
    .number()
    .min(0, 'Lương cơ bản phải là số dương')
    .refine((val) => !isNaN(val), { message: 'Phải là số hợp lệ' }),
});

export const editStaffSchema = z.object({
  title: z.string().min(1, 'Vui lòng nhập chức vụ'),
  type: z.enum(['Lương Theo Giờ', 'Lương Cứng'], {
    required_error: 'Vui lòng chọn hình thức lương',
    invalid_type_error: 'Hình thức lương phải là "Lương Theo Giờ" hoặc "Lương Cứng"',
  }),
  salaryBase: z
    .number()
    .min(0, 'Lương phải là số dương')
    .refine((val) => !isNaN(val), { message: 'Phải là số hợp lệ' }),
  branchId: z.string().min(1, 'Vui lòng chọn chi nhánh'),
});

export const roleSchema = z.object({
  roleName: z.string().nonempty({ message: 'Tên vai trò là bắt buộc' }),
  description: z.string().nonempty({ message: 'Mô tả là bắt buộc' }),
});

export const membershipRankSchema = z.object({
  rankName: z.string().nonempty({ message: 'Tên cấp bậc là bắt buộc' }),
  description: z.string().nonempty({ message: 'Mô tả là bắt buộc' }),
});

export const workShiftSchema = z.object({
  name: z.string().optional(),
  startTime: z.string().nonempty({ message: 'Giờ bắt đầu là bắt buộc' }),
  endTime: z.string().nonempty({ message: 'Giờ kết thúc là bắt buộc' }),
  gracePeriod: z.string().nonempty({ message: 'Thời gian ân hạn là bắt buộc' }),
});

export const registerSchema = z.object({
  fullName: z.string().min(1, { message: 'Họ và tên là bắt buộc' }),
  email: z.string().email({ message: 'Vui lòng nhập địa chỉ email hợp lệ' }),
  password: z
    .string()
    .min(8, { message: 'Mật khẩu phải có ít nhất 8 ký tự' })
    .regex(/[A-Z]/, 'Mật khẩu phải chứa ít nhất một chữ cái in hoa')
    .regex(/[a-z]/, 'Mật khẩu phải chứa ít nhất một chữ cái thường')
    .regex(/[0-9]/, 'Mật khẩu phải chứa ít nhất một số')
    .regex(/[^A-Za-z0-9]/, 'Mật khẩu phải chứa ít nhất một ký tự đặc biệt'),
});

export const notificationSchema = z.object({
  title: z.string().min(1, 'Tiêu đề thông báo là bắt buộc'),
  content: z.string().min(1, 'Nội dung thông báo là bắt buộc'),
  ranks: z.array(z.string()).nonempty({ message: 'Phải chọn ít nhất một cấp bậc' }),
  priority: z.enum(['0', '1', '2']),
});

export const profileAdminSchema = z.object({
  fullName: z.string().min(1, 'Họ và tên là bắt buộc'),
  email: z.string().email('Địa chỉ email không hợp lệ'),
  phone: z
    .string()
    .optional()
    .refine((val) => !val || /(0[3-9][0-9]{8})\b/.test(val), {
      message: 'Số điện thoại không hợp lệ. Ví dụ: 0982641707',
    }),
  dob: z.string().optional(),
  address: z.string().optional(),
  image: z
    .instanceof(File)
    .optional()
    .refine((file) => !file || file.size <= MAX_FILE_SIZE, 'Kích thước tệp phải nhỏ hơn 5MB')
    .refine(
      (file) => !file || ACCEPTED_IMAGE_TYPES.includes(file.type),
      'Chỉ hỗ trợ định dạng .jpg, .jpeg, .png và .gif',
    ),
});
