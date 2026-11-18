"use client";

import React, { useState } from "react";
import { motion, type Variants } from "framer-motion";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { ArrowLeft, Mail, KeyRound, Loader2 } from "lucide-react";
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
import { useSimpleMutation } from "@/hooks/use-mutation-data";
import { userService } from "@/services/userService";

const formSchema = z.object({
  email: z.string().email({ message: "Vui lòng nhập địa chỉ email hợp lệ" }),
});

const ChangePasswordForm = () => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const router = useRouter();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
    },
  });

  const { mutateAsync: sendOtp } = useSimpleMutation({
    mutationKey: ["send-otp"],
    mutationFn: (email: string) => userService.resendOtp(email),
    successMessage: "Mã OTP đã được gửi đến email của bạn",
    onSuccess: () => {
      router.push(
        `/auth/verify-otp?email=${encodeURIComponent(form.getValues().email)}`
      );
    },
  });

  async function onSubmit(values: z.infer<typeof formSchema>) {
    setIsSubmitting(true);
    await sendOtp(values.email);

    setIsSubmitting(false);
    setIsSubmitted(true);
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
    <>
      <motion.div
        initial="hidden"
        animate="visible"
        variants={containerVariants}
        className="w-full max-w-md"
      >
        <div className="text-white shadow-lg p-8">
          <motion.div variants={itemVariants} className="mb-6">
            <Link
              href="/auth/login"
              className="text-stone-400 hover:text-stone-500 inline-flex items-center"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Quay lại đăng nhập
            </Link>
          </motion.div>

          <motion.div variants={itemVariants} className="text-center mb-8">
            <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-stone-400 mb-4">
              <KeyRound stroke="#fff" className="h-8 w-8 text-primary" />
            </div>
            <h1 className="text-2xl font-bold mb-2">Quên mật khẩu</h1>
            <p className="text-stone-400">
              Nhập địa chỉ email của bạn và chúng tôi sẽ gửi mã OTP để bạn đặt lại mật khẩu.
            </p>
          </motion.div>

          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <motion.div variants={itemVariants}>
                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Email</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-stone-500" />
                          <Input
                            placeholder="Enter your email address"
                            className="pl-10 bg-stone-700/50 border-stone-600/50 text-stone-200 placeholder:text-stone-500 focus:border-green-700 focus:ring-green-700"
                            {...field}
                            disabled={isSubmitting || isSubmitted}
                          />
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
                  disabled={isSubmitting || isSubmitted}
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Đang gửi...
                    </>
                  ) : isSubmitted ? (
                    <>Đã gửi OTP!</>
                    ) : (
                    <>Gửi mã OTP</>
                  )}
                </Button>
              </motion.div>
            </form>
          </Form>

          <motion.div variants={itemVariants} className="mt-6 text-center">
            <p className="text-sm text-stone-400">
              Bạn nhớ mật khẩu?&nbsp;
              <Link href="/auth/login" passHref>
              <Button
                variant="link"
                className="text-green-500 hover:text-green-400 p-0 font-semibold"
              >
                Đăng nhập
              </Button>
              </Link>
            </p>
          </motion.div>
        </div>
      </motion.div>
    </>
  );
};

export default ChangePasswordForm;
