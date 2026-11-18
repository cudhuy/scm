"use client";

import { useState, useRef, useEffect } from "react";
import { motion, AnimatePresence, type Variants } from "framer-motion";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { ArrowLeft, Shield, Loader2, CheckCircle2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { useSimpleMutation } from "@/hooks/use-mutation-data";
import { userService } from "@/services/userService";

const formSchema = z.object({
  otp: z
    .string()
    .length(6, { message: "Mã OTP phải có 6 chữ số" })
    .regex(/^\d+$/, { message: "Mã OTP chỉ được chứa chữ số" }),
});

export default function VerifyOTPForm() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isVerified, setIsVerified] = useState(false);
  const [resendDisabled, setResendDisabled] = useState(true);
  const [countdown, setCountdown] = useState(30);
  const router = useRouter();
  const searchParams = useSearchParams();
  const email = searchParams?.get("email") || "";
  const inputRefs = useRef<(HTMLInputElement | null)[]>([
    null,
    null,
    null,
    null,
    null,
    null,
  ]);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      otp: "",
    },
  });

  const { mutateAsync: verifyOtp } = useSimpleMutation({
    mutationKey: ["verify-otp"],
    mutationFn: ({ email, otp }: { email: string; otp: string }) =>
      userService.verifyOtp(email, otp),
    successMessage: "Mã OTP đã được xác minh thành công",
    onSuccess: () => {
      router.push(`/auth/reset-password?email=${encodeURIComponent(email)}`);
    },
  });

  useEffect(() => {
    if (resendDisabled && countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    } else if (countdown === 0) {
      setResendDisabled(false);
    }
  }, [countdown, resendDisabled]);

  async function onSubmit(values: z.infer<typeof formSchema>) {
    setIsSubmitting(true);
    await verifyOtp({ email, otp: values.otp });

    setIsSubmitting(false);
    setIsVerified(true);
  }

  const handleResendOTP = async () => {
    setResendDisabled(true);
    setCountdown(30);

    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1000));
  };

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

  const successVariants: Variants = {
    hidden: { scale: 0.8, opacity: 0 },
    visible: {
      scale: 1,
      opacity: 1,
      transition: {
        type: "spring",
        stiffness: 200,
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
        <div className="shadow-lg p-8 text-white">
          <motion.div variants={itemVariants} className="mb-6">
            <Link
              href="/auth/forgot-password"
              className="text-stone-400 hover:text-stone-500 inline-flex items-center"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Quay lại
            </Link>
          </motion.div>

          <AnimatePresence mode="wait">
            {isVerified ? (
              <motion.div
                key="success"
                initial="hidden"
                animate="visible"
                variants={successVariants}
                className="text-center py-8"
              >
                <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-green-500/10 mb-4">
                  <CheckCircle2 className="h-8 w-8 text-green-500" />
                </div>
                <h1 className="text-2xl font-bold mb-2">
                  Mã OTP đã được xác minh!
                </h1>
                <p className="text-stone-400 mb-4">
                  Đang chuyển hướng bạn đến trang đổi mật khẩu...
                </p>
                <div className="flex justify-center">
                  <div className="w-16 h-1 bg-muted overflow-hidden rounded-full">
                    <motion.div
                      className="h-full bg-primary"
                      initial={{ width: 0 }}
                      animate={{ width: "100%" }}
                      transition={{ duration: 1.5, ease: "linear" }}
                    />
                  </div>
                </div>
              </motion.div>
            ) : (
              <motion.div key="form">
                <motion.div
                  variants={itemVariants}
                  className="text-center mb-8"
                >
                  <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-stone-400 mb-4">
                    <Shield stroke="#fff" className="h-8 w-8 text-primary" />
                  </div>
                  <h1 className="text-2xl font-bold mb-2">Xác minh OTP</h1>
                  <p className="text-stone-400">
                    Chúng tôi đã gửi mã OTP gồm 6 chữ số đến&nbsp;
                    <span className="font-medium text-white">{email}</span>
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
                        name="otp"
                        render={({ field }) => (
                          <FormItem>
                            <FormControl>
                              <div className="flex justify-between gap-2">
                                {Array(6)
                                  .fill(0)
                                  .map((_, index) => (
                                    <Input
                                      key={index}
                                      ref={(el) => {
                                        inputRefs.current[index] = el;
                                      }}
                                      type="text"
                                      inputMode="numeric"
                                      maxLength={1}
                                      className="w-12 h-12 text-center text-lg font-bold"
                                      onChange={(e) => {
                                        const value = e.target.value;
                                        if (value && /^\d$/.test(value)) {
                                          // Update the form value
                                          const newOtp = field.value.split("");
                                          newOtp[index] = value;
                                          field.onChange(newOtp.join(""));

                                          // Move to next input
                                          if (
                                            index < 5 &&
                                            inputRefs.current[index + 1]
                                          ) {
                                            inputRefs.current[
                                              index + 1
                                            ]?.focus();
                                          }
                                        }
                                      }}
                                      onKeyDown={(e) => {
                                        // Handle backspace
                                        if (e.key === "Backspace") {
                                          if (
                                            !e.currentTarget.value &&
                                            index > 0 &&
                                            inputRefs.current[index - 1]
                                          ) {
                                            inputRefs.current[
                                              index - 1
                                            ]?.focus();
                                          }

                                          // Update the form value
                                          const newOtp = field.value.split("");
                                          newOtp[index] = "";
                                          field.onChange(newOtp.join(""));
                                        }
                                      }}
                                      onPaste={(e) => {
                                        e.preventDefault();
                                        const pastedData = e.clipboardData
                                          .getData("text/plain")
                                          .trim();
                                        if (
                                          /^\d+$/.test(pastedData) &&
                                          pastedData.length <= 6
                                        ) {
                                          // Fill inputs with pasted data
                                          const otpChars = pastedData
                                            .split("")
                                            .slice(0, 6);
                                          otpChars.forEach((char, i) => {
                                            if (inputRefs.current[i]) {
                                              inputRefs.current[i]!.value =
                                                char;
                                            }
                                          });

                                          // Update form value
                                          field.onChange(
                                            otpChars.join("") +
                                              "0".repeat(
                                                Math.max(0, 6 - otpChars.length)
                                              )
                                          );

                                          // Focus the next empty input or the last one
                                          const nextEmptyIndex =
                                            otpChars.length < 6
                                              ? otpChars.length
                                              : 5;
                                          inputRefs.current[
                                            nextEmptyIndex
                                          ]?.focus();
                                        }
                                      }}
                                      value={field.value[index] || ""}
                                      disabled={isSubmitting}
                                    />
                                  ))}
                              </div>
                            </FormControl>
                            <FormMessage className="text-center mt-2" />
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
                            Đang xác minh...
                          </>
                        ) : (
                          <>Xác minh</>
                        )}
                      </Button>
                    </motion.div>
                  </form>
                </Form>

                <motion.div
                  variants={itemVariants}
                  className="mt-6 text-center"
                >
                  <p className="text-sm text-stone-400 mb-2">
                    Bạn không nhận được mã OTP?
                  </p>
                  <Button
                    variant="link"
                    onClick={handleResendOTP}
                    disabled={resendDisabled}
                    className="text-green-500 hover:text-green-400"
                  >
                    {resendDisabled ? (
                      <>Gửi lại mã OTP sau {countdown}s</>
                    ) : (
                      <>Gửi lại mã OTP</>
                    )}
                  </Button>
                </motion.div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </motion.div>
    </div>
  );
}
