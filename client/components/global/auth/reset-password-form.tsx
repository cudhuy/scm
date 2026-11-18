"use client";

import { useState } from "react";
import { motion, type Variants } from "framer-motion";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import {
  ArrowLeft,
  Lock,
  Eye,
  EyeOff,
  Loader2,
  CheckCircle2,
  Check,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import zxcvbn from "zxcvbn";
import { cn } from "@/lib/utils";
import { Progress } from "@/components/ui/progress";
import { useSimpleMutation } from "@/hooks/use-mutation-data";
import { userService } from "@/services/userService";
import { ResetPasswordDto } from "@/types/auth";

const formSchema = z
  .object({
    password: z
      .string()
      .min(8, { message: "Mật khẩu phải có ít nhất 8 ký tự" }),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Mật khẩu không khớp",
    path: ["confirmPassword"],
  });

const passwordRequirements = [
  { label: "Ít nhất 8 ký tự", regex: /.{8,}/ },
  { label: "Chứa chữ cái viết hoa", regex: /[A-Z]/ },
  { label: "Chứa chữ cái viết thường", regex: /[a-z]/ },
  { label: "Chứa số", regex: /\d/ },
  { label: "Chứa ký tự đặc biệt", regex: /[^A-Za-z0-9]/ },
];

export default function ResetPasswordForm() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const router = useRouter();
  const searchParams = useSearchParams();
  const email = searchParams?.get("email") || "";

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      password: "",
      confirmPassword: "",
    },
  });

  const password = form.watch("password");
  const passwordStrength = zxcvbn(password || "");
  const strengthLabels = ["Rất yếu", "Yếu", "Trung bình", "Mạnh", "Rất mạnh"];
  const strengthColors = [
    "bg-red-500",
    "bg-orange-500",
    "bg-yellow-500",
    "bg-green-500",
    "bg-emerald-500",
  ];

  const checkRequirement = (requirement: { regex: RegExp }) => {
    return requirement.regex.test(password || "");
  };

  const { mutateAsync: resetPassword } = useSimpleMutation<
    ResetPasswordDto,
    void
  >({
    mutationKey: ["reset-password"],
    mutationFn: async (data: ResetPasswordDto) => {
      await userService.resetPassword(data);
        },
        successMessage: "Đặt lại mật khẩu thành công",
        onSuccess: () => {
      router.push("/auth/login");
    },
  });

  async function onSubmit(values: z.infer<typeof formSchema>) {
    setIsSubmitting(true);

    // Simulate API call
    await resetPassword({
      email,
      newPassword: values.password,
      confirmPassword: values.confirmPassword,
    });

    setIsSubmitting(false);
    setIsSuccess(true);
  }

  const containerVariants: Variants = {
    hidden: { opacity: 0 },
    visible: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1,
      },
    },
  };

  const itemVariants: Variants = {
    hidden: { opacity: 0, y: 20 },
    visible: {
      opacity: 1,
      y: 0,
      transition: {
        type: "spring",
        stiffness: 100,
        damping: 15,
      },
    },
  };

  return (
    <div>
      <motion.div
        initial="hidden"
        animate="visible"
        variants={containerVariants}
        className="w-full max-w-md"
      >
        <div className="rounded-lg shadow-lg p-8 border border-border text-white">
          {isSuccess ? (
            <motion.div
              initial={{ scale: 0.8, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              transition={{ type: "spring", stiffness: 200, damping: 15 }}
              className="text-center py-8"
            >
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-green-500/10 mb-4">
                <CheckCircle2 className="h-8 w-8 text-green-500" />
              </div>
                <h1 className="text-2xl font-bold mb-2">
                Đặt lại mật khẩu thành công!
                </h1>
                <p className="text-stone-400 mb-6">
                Mật khẩu của bạn đã được đặt lại thành công. Bạn sẽ được chuyển hướng đến trang đăng nhập.
                </p>
              <div className="flex justify-center">
                <div className="w-16 h-1 bg-muted overflow-hidden rounded-full">
                  <motion.div
                    className="h-full bg-primary"
                    initial={{ width: 0 }}
                    animate={{ width: "100%" }}
                    transition={{ duration: 2, ease: "linear" }}
                  />
                </div>
              </div>
            </motion.div>
          ) : (
            <>
              <motion.div variants={itemVariants} className="mb-6">
                <Link
                  href="/auth/verify-otp"
                  className="text-stone-400 hover:text-stone-500 inline-flex items-center"
                >
                  <ArrowLeft className="mr-2 h-4 w-4" />
                  Quay lại
                </Link>
              </motion.div>

              <motion.div variants={itemVariants} className="text-center mb-8">
                <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-stone-400 mb-4">
                  <Lock stroke="#fff" className="h-8 w-8 text-primary" />
                </div>
                <h1 className="text-2xl font-bold mb-2">Đặt lại mật khẩu</h1>
                <p className="text-stone-400">
                  Tạo mật khẩu mới cho tài khoản của bạn
                </p>
              </motion.div>

              <Form {...form}>
                <form
                  onSubmit={form.handleSubmit(onSubmit)}
                  className="space-y-6"
                >
                  <motion.div variants={itemVariants}>
                    <FormField
                      control={form.control}
                      name="password"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Mật khẩu mới</FormLabel>
                          <FormControl>
                            <div className="relative">
                              <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-stone-400" />
                              <Input
                                type={showPassword ? "text" : "password"}
                                placeholder="Nhập mật khẩu mới"
                                className="pl-10 pr-10 bg-stone-700/50 border-stone-600/50 text-stone-200 placeholder:text-stone-500 focus:border-green-700 focus:ring-green-700"
                                {...field}
                              />
                              <div
                                className="absolute right-0 top-1/2 transform -translate-y-1/2 w-8 p-0"
                                onClick={() => setShowPassword(!showPassword)}
                              >
                                {showPassword ? (
                                  <EyeOff className="h-4 w-4 text-stone-400" />
                                ) : (
                                  <Eye className="h-4 w-4 text-stone-400" />
                                )}
                              </div>
                            </div>
                          </FormControl>
                          <FormMessage />
                          {password && (
                            <motion.div
                              initial={{ opacity: 0, y: -10 }}
                              animate={{ opacity: 1, y: 0 }}
                              className="space-y-3 rounded-lg bg-stone-800/50 p-3 border border-stone-700/50"
                            >
                              <div className="space-y-2">
                                <div className="flex justify-between text-sm">
                                    <span className="text-stone-400">
                                    Độ mạnh:
                                    </span>
                                  <span
                                    className={cn(
                                      "font-medium",
                                      passwordStrength.score === 4 &&
                                        "text-emerald-500",
                                      passwordStrength.score === 3 &&
                                        "text-green-500",
                                      passwordStrength.score === 2 &&
                                        "text-yellow-500",
                                      passwordStrength.score === 1 &&
                                        "text-orange-500",
                                      passwordStrength.score === 0 &&
                                        "text-red-500"
                                    )}
                                  >
                                    {strengthLabels[passwordStrength.score]}
                                  </span>
                                </div>
                                <Progress
                                  value={(passwordStrength.score + 1) * 20}
                                  className={cn(
                                    "h-1",
                                    strengthColors[passwordStrength.score]
                                  )}
                                />
                              </div>

                              <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                                {passwordRequirements.map(
                                  (requirement, index) => (
                                    <div
                                      key={index}
                                      className="flex items-center gap-2 text-sm"
                                    >
                                      <div
                                        className={cn(
                                          "size-4 rounded-full flex items-center justify-center",
                                          checkRequirement(requirement)
                                            ? "bg-green-500/20 text-green-500"
                                            : "bg-stone-700/50 text-stone-500"
                                        )}
                                      >
                                        <Check className="h-3 w-3" />
                                      </div>
                                      <span
                                        className={cn(
                                          "text-sm",
                                          checkRequirement(requirement)
                                            ? "text-stone-300"
                                            : "text-stone-500"
                                        )}
                                      >
                                        {requirement.label}
                                      </span>
                                    </div>
                                  )
                                )}
                              </div>
                            </motion.div>
                          )}
                        </FormItem>
                      )}
                    />
                  </motion.div>

                  <motion.div variants={itemVariants}>
                    <FormField
                      control={form.control}
                      name="confirmPassword"
                      render={({ field }) => (
                        <FormItem>
                            <FormLabel>Xác nhận mật khẩu</FormLabel>
                          <FormControl>
                            <div className="relative">
                              <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-stone-400" />
                              <Input
                                type={showConfirmPassword ? "text" : "password"}
                                placeholder="Xác nhận mật khẩu mới"
                                className="pl-10 pr-10 bg-stone-700/50 border-stone-600/50 text-stone-200 placeholder:text-stone-500 focus:border-green-700 focus:ring-green-700"
                                {...field}
                              />
                              <div
                                className="absolute right-0 top-1/2 transform -translate-y-1/2 w-8 p-0"
                                onClick={() =>
                                  setShowConfirmPassword(!showConfirmPassword)
                                }
                              >
                                {showConfirmPassword ? (
                                  <EyeOff className="h-4 w-4 text-stone-400" />
                                ) : (
                                  <Eye className="h-4 w-4 text-stone-400" />
                                )}
                              </div>
                            </div>
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </motion.div>

                  <motion.div variants={itemVariants}>
                    <Button
                      type="submit"
                      className="w-full bg-linear-to-r from-green-800 to-green-700 text-stone-100 hover:from-green-700 hover:to-green-600 border-0 text-base font-semibold active:scale-95"
                      disabled={isSubmitting}
                    >
                      {isSubmitting ? (
                        <>
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          Đang đặt lại...
                        </>
                        ) : (
                        <>Đặt lại mật khẩu</>
                      )}
                    </Button>
                  </motion.div>
                </form>
              </Form>
            </>
          )}
        </div>
      </motion.div>
    </div>
  );
}
