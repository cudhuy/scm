"use client";

import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { buttonVariants } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { CheckCircle } from "lucide-react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { cn } from "@/lib/utils";

export default function VerifyEmailSuccessPage() {
  const [countdown, setCountdown] = useState(5);
  const router = useRouter();

  useEffect(() => {
    const timer = setInterval(() => {
      setCountdown((prevCount) => prevCount - 1);
    }, 1000);

    const redirect = setTimeout(() => {
      router.push("/");
    }, 5000);

    return () => {
      clearInterval(timer);
      clearTimeout(redirect);
    };
  }, [router]);

  return (
    <Card className="w-full max-w-sm mx-auto">
      <CardHeader>
        <CardTitle className="text-3xl font-bold text-center">
          Email Verified!
        </CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col items-center p-8">
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ type: "spring", stiffness: 260, damping: 20 }}
        >
          <CheckCircle className="w-32 h-32 text-green-500 mb-6" />
        </motion.div>
        <p className="text-center text-xl mb-6">
          Your email has been successfully verified. Welcome aboard!
        </p>
        <p className="text-center text-lg text-slate-700/70">
          Redirecting to homepage in {countdown} seconds...
        </p>
      </CardContent>
      <CardFooter className="flex justify-center pb-8">
        <Link href={"/"} className={cn(buttonVariants())}>
          Go to Homepage Now
        </Link>
      </CardFooter>
    </Card>
  );
}
