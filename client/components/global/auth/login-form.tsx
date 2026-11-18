"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

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
import { Eye, EyeOff } from "lucide-react";
import Link from "next/link";
import GoogleSignIn from "@/components/global/auth/google-sign-in";
import { signIn } from "next-auth/react";
import { useSimpleMutation } from "@/hooks/use-mutation-data";
import Loader from "@/components/global/loader";
import { useAuthRedirect } from "@/hooks/use-auth-redirect";

const formSchema = z.object({
  email: z.string().email({ message: "Vui lòng nhập địa chỉ email hợp lệ" }),
  password: z.string().min(8, { message: "Mật khẩu phải có ít nhất 8 ký tự" }),
});

export default function LoginForm() {
  const [showPassword, setShowPassword] = useState(false);
  const router = useRouter();
  const { callbackUrl } = useAuthRedirect();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const { mutate, isPending } = useSimpleMutation({
    mutationKey: ["login"],
    mutationFn: async (values: z.infer<typeof formSchema>) => {
      const result = await signIn("credentials", {
        email: values.email,
        password: values.password,
        redirect: false,
      });
      if (result?.error) {
        throw new Error(result.code as unknown as string);
      }
      return result;
    },
    onSuccess: async () => {
      const session = await fetch("/api/auth/session").then((res) =>
        res.json()
      );
      if (session?.user) {
        if (callbackUrl) {
          router.push(callbackUrl);
          return;
        } else {
          const roleName = session.user.roleName?.toLowerCase();
          if (roleName === "user") {
            router.push("/");
          } else if (roleName === "staff") {
            router.push(`/${roleName}/orders`);
          } else if (roleName) {
            router.push(`/${roleName}`);
          } else {
            router.push("/");
          }
        }
      } else {
        router.push("/");
      }
    },
    onError: (error) => {
      console.error(error);

      if (
        error.message ==
        "Tài khoản chưa được xác minh. Vui lòng kiểm tra email của bạn."
      ) {
        router.push(
          `/auth/verify-email?email=${encodeURIComponent(
            form.getValues().email
          )}`
        );
        return;
      }
    },
    successMessage: "Đăng nhập thành công!",
  });

  async function onSubmit(values: z.infer<typeof formSchema>) {
    mutate(values);
  }

  return (
    <>
      <CardHeader className="p-4 sm:p-6 md:p-8">
        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center">
          <h2 className="text-lg sm:text-xl font-semibold text-stone-200">
            Chào Mừng Trở Lại,
          </h2>
        </div>
        <p className="text-stone-400 text-sm sm:text-base">
          Vui lòng đăng nhập vào tài khoản của bạn
        </p>
      </CardHeader>
      <CardContent className="p-4 sm:p-6 md:p-8 pt-0 space-y-4">
        <Form {...form}>
          <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-stone-300">
                    Địa Chỉ Email
                  </FormLabel>
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
                <FormItem>
                  <FormLabel className="text-stone-300">Mật Khẩu</FormLabel>
                  <FormControl>
                    <div>
                      <div className="relative">
                        <Input
                          type={showPassword ? "text" : "password"}
                          placeholder="Mật khẩu"
                          className="bg-stone-700/50 border-stone-600/50 text-stone-200 focus:border-green-700 focus:ring-green-700 w-full"
                          {...field}
                        />
                        {showPassword ? (
                          <EyeOff
                            className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 w-4 h-4 cursor-pointer"
                            onClick={() => setShowPassword(false)}
                          />
                        ) : (
                          <Eye
                            className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 w-4 h-4 cursor-pointer"
                            onClick={() => setShowPassword(true)}
                          />
                        )}
                      </div>
                      <Link
                        href="/auth/forgot-password"
                        className="text-green-500 hover:text-green-400 text-sm mt-2 font-semibold"
                      >
                        Quên mật khẩu?
                      </Link>
                    </div>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <Button
              type="submit"
              className="w-full bg-linear-to-r from-green-800 to-green-700 text-stone-100 hover:from-green-700 hover:to-green-600 border-0 text-base font-semibold active:scale-95"
              disabled={isPending}
            >
              <Loader state={isPending} textLoading="Đang đăng nhập...">
                <span>Đăng Nhập</span>
              </Loader>
            </Button>
          </form>
        </Form>

        <div className="relative text-center">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-stone-700"></div>
          </div>
          <span className="relative px-2 text-sm text-stone-400 bg-linear-to-br from-stone-800 to-neutral-900">
            hoặc
          </span>
        </div>

        <GoogleSignIn callbackUrl={callbackUrl} />

        <p className="text-center text-sm text-stone-400">
          Mới sử dụng SCM?{" "}
          <Link href="/auth/register" passHref>
            <Button
              variant="link"
              className="text-green-500 hover:text-green-400 p-0 font-semibold"
            >
              Tạo Tài Khoản
            </Button>
          </Link>
        </p>
      </CardContent>
    </>
  );
}
