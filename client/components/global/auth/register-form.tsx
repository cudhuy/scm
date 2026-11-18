"use client";

import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useRouter } from "next/navigation";
import { motion } from "framer-motion";
import Link from "next/link";
import zxcvbn from "zxcvbn";
import { Check, Eye, EyeOff } from "lucide-react";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { CardContent, CardHeader } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import Loader from "@/components/global/loader";
import { userService } from "@/services/userService";
import { useSimpleMutation } from "@/hooks/use-mutation-data";
import { cn } from "@/lib/utils";
import { registerSchema } from "@/schema";
import GoogleSignIn from "./google-sign-in";

const passwordRequirements = [
  { label: "Ít nhất 8 ký tự", regex: /.{8,}/ },
  { label: "Chứa chữ cái viết hoa", regex: /[A-Z]/ },
  { label: "Chứa chữ cái viết thường", regex: /[a-z]/ },
  { label: "Chứa số", regex: /\d/ },
  { label: "Chứa ký tự đặc biệt", regex: /[^A-Za-z0-9]/ },
];

export default function RegisterForm() {
  const [showPassword, setShowPassword] = useState(false);
  const router = useRouter();

  const form = useForm<z.infer<typeof registerSchema>>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      fullName: "",
      email: "",
      password: "",
    },
    mode: "onChange",
  });

  const { mutate, isPending } = useSimpleMutation({
    mutationKey: ["register"],
    mutationFn: async (values: z.infer<typeof registerSchema>) => {
      const result = await userService.register(values);
      return result;
    },
    successMessage: "Đăng ký thành công",
    onSuccess: () => {
      router.push(
        `/auth/verify-email?email=${encodeURIComponent(form.getValues().email)}`
      );
    },
    onError: (error) => console.error(error),
  });

  const password = form.watch("password");
  const passwordStrength = zxcvbn(password || "");
  const strengthLabels = ["Rất yếu", "Yếu", "Khá", "Mạnh", "Rất mạnh"];
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

  async function onSubmit(values: z.infer<typeof registerSchema>) {
    mutate(values);
  }

  return (
    <>
      <CardHeader className="p-4 sm:p-6 md:p-8">
        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center">
          <h2 className="text-lg sm:text-xl font-semibold text-stone-200">
            Tạo tài khoản
          </h2>
        </div>
        <p className="text-stone-400 text-sm sm:text-base">
          Vui lòng điền vào biểu mẫu để đăng ký
        </p>
      </CardHeader>
      <CardContent className="p-4 sm:p-6 md:p-8 pt-0 space-y-4">
        <Form {...form}>
          <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
            <FormField
              control={form.control}
              name="fullName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-stone-300">Họ và tên</FormLabel>
                  <FormControl>
                    <Input
                      type="text"
                      placeholder="Nguyễn Văn A"
                      className="bg-stone-700/50 border-stone-600/50 text-stone-200 placeholder:text-stone-500 focus:border-green-700 focus:ring-green-700 w-full"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-stone-300">Email</FormLabel>
                  <FormControl>
                    <Input
                      type="email"
                      placeholder="your@email.com"
                      className="bg-stone-700/50 border-stone-600/50 text-stone-200 placeholder:text-stone-500 focus:border-green-700 focus:ring-green-700 w-full"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem className="space-y-3">
                  <FormLabel className="text-stone-300">Mật khẩu</FormLabel>
                  <FormControl>
                    <div className="relative">
                      <Input
                        type={showPassword ? "text" : "password"}
                        placeholder="Mật khẩu"
                        className="bg-stone-700/50 border-stone-600/50 text-stone-200 focus:border-green-700 focus:ring-green-700 w-full"
                        {...field}
                      />
                      {showPassword ? (
                        <Eye
                          className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 w-4 h-4 cursor-pointer"
                          onClick={() => setShowPassword(false)}
                        />
                      ) : (
                        <EyeOff
                          className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 w-4 h-4 cursor-pointer"
                          onClick={() => setShowPassword(true)}
                        />
                      )}
                    </div>
                  </FormControl>
                  <FormMessage />

                  {/* Password Strength Indicator */}
                  {password && (
                    <motion.div
                      initial={{ opacity: 0, y: -10 }}
                      animate={{ opacity: 1, y: 0 }}
                      className="space-y-3 rounded-lg bg-stone-800/50 p-3 border border-stone-700/50"
                    >
                      <div className="space-y-2">
                        <div className="flex justify-between text-sm">
                          <span className="text-stone-400">Độ mạnh:</span>
                          <span
                            className={cn(
                              "font-medium",
                              passwordStrength.score === 4 &&
                                "text-emerald-500",
                              passwordStrength.score === 3 && "text-green-500",
                              passwordStrength.score === 2 && "text-yellow-500",
                              passwordStrength.score === 1 && "text-orange-500",
                              passwordStrength.score === 0 && "text-red-500"
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
                        {passwordRequirements.map((requirement, index) => (
                          <div
                            key={index}
                            className="flex items-center gap-2 text-xs sm:text-sm"
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
                                "text-xs sm:text-sm",
                                checkRequirement(requirement)
                                  ? "text-stone-300"
                                  : "text-stone-500"
                              )}
                            >
                              {requirement.label}
                            </span>
                          </div>
                        ))}
                      </div>
                    </motion.div>
                  )}
                </FormItem>
              )}
            />

            <Button
              type="submit"
              className="w-full bg-linear-to-r from-green-800 to-green-700 text-stone-100 hover:from-green-700 hover:to-green-600 border-0 text-base font-semibold active:scale-95"
              disabled={isPending}
            >
              <Loader state={isPending} textLoading="Đang đăng ký...">
                <span>Đăng ký</span>
              </Loader>
            </Button>

            <div className="relative text-center">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-stone-700"></div>
              </div>
              <span className="relative px-2 text-sm text-stone-400 bg-linear-to-br from-stone-800 to-neutral-900">
                hoặc
              </span>
            </div>
          </form>

          <GoogleSignIn />

          <p className="text-center text-sm text-stone-400">
            Bạn đã có tài khoản?{" "}
            <Link href="/auth/login" passHref>
              <Button
                variant="link"
                className="text-green-500 hover:text-green-400 p-0 font-semibold"
              >
                Đăng nhập
              </Button>
            </Link>
          </p>
        </Form>
      </CardContent>
    </>
  );
}
