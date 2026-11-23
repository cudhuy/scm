"use client";

import type React from "react";

import { useState, useRef, useEffect } from "react";
import { motion } from "framer-motion";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { CheckCircle2, Timer } from "lucide-react";
import { toast } from "sonner";
import Loader from "@/components/global/loader";

export default function VerifyOTPPage() {
  const [otp, setOtp] = useState(["", "", "", "", "", ""]);
  const [isVerifying, setIsVerifying] = useState(false);
  const [isVerified, setIsVerified] = useState(false);
  const [countdown, setCountdown] = useState(30);
  const [canResend, setCanResend] = useState(false);
  const inputs = useRef<(HTMLInputElement | null)[]>([]);

  useEffect(() => {
    // Auto focus first input on mount
    inputs.current[0]?.focus();

    // Countdown timer for resend
    let timer: NodeJS.Timeout;
    if (countdown > 0 && !canResend) {
      timer = setInterval(() => {
        setCountdown((prev) => prev - 1);
      }, 1000);
    } else {
      setCanResend(true);
    }

    return () => clearInterval(timer);
  }, [countdown, canResend]);

  const handleChange = (element: HTMLInputElement, index: number) => {
    if (isVerifying) return;

    const value = element.value;
    // Allow only numbers
    if (value && !/^\d+$/.test(value)) return;

    setOtp((prevOtp) => {
      const newOtp = [...prevOtp];
      newOtp[index] = value.slice(-1);
      return newOtp;
    });

    // Auto advance to next input
    if (element.value && index < 5) {
      inputs.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (
    e: React.KeyboardEvent<HTMLInputElement>,
    index: number
  ) => {
    if (isVerifying) return;

    // Handle backspace
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      inputs.current[index - 1]?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent) => {
    e.preventDefault();
    if (isVerifying) return;

    const pastedData = e.clipboardData.getData("text").slice(0, 6);
    if (!/^\d+$/.test(pastedData)) return;

    const otpArray = pastedData.split("").slice(0, 6);
    setOtp((prevOtp) => {
      const newOtp = [...prevOtp];
      otpArray.forEach((value, index) => {
        newOtp[index] = value;
      });
      return newOtp;
    });

    // Focus last filled input or first empty input
    const lastFilledIndex = otpArray.length - 1;
    if (lastFilledIndex >= 0) {
      inputs.current[Math.min(lastFilledIndex, 5)]?.focus();
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isVerifying || isVerified) return;

    const otpString = otp.join("");
    if (otpString.length !== 6) {
      toast.error("Please enter all 6 digits");
      return;
    }

    setIsVerifying(true);

    try {
      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 1500));

      setIsVerified(true);
      toast.success("Email verified successfully!");
    } catch (error) {
      toast.error("Failed to verify OTP. Please try again." + error);
      setIsVerifying(false);
    }
  };

  const handleResend = async () => {
    if (!canResend) return;

    try {
      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 1000));

      setCountdown(30);
      setCanResend(false);
      toast.success("New OTP sent successfully!");
    } catch (error) {
      toast.error("Failed to resend OTP. Please try again." + error);
    }
  };

  return (
    <Card className="w-full max-w-xl mx-auto">
      <CardHeader>
        <CardTitle className="text-3xl font-bold text-center">
          Verify Your Email
        </CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col items-center p-8">
        {isVerified ? (
          <motion.div
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            transition={{ type: "spring", duration: 0.5 }}
            className="flex flex-col items-center"
          >
            <CheckCircle2 className="w-32 h-32 text-green-500 mb-6" />
            <p className="text-center text-xl">Email verified successfully!</p>
          </motion.div>
        ) : (
          <>
            <p className="text-center text-lg mb-8">
              We&apos;ve sent a 6-digit code to your email. Enter it below to
              verify your account.
            </p>
            <form onSubmit={handleSubmit} className="w-full max-w-md space-y-6">
              <div className="flex justify-center gap-2 sm:gap-4">
                {otp.map((digit, index) => (
                  <Input
                    key={index}
                    type="text"
                    inputMode="numeric"
                    maxLength={1}
                    value={digit}
                    onChange={(e) => handleChange(e.target, index)}
                    onKeyDown={(e) => handleKeyDown(e, index)}
                    onPaste={handlePaste}
                    ref={(el) => {
                      inputs.current[index] = el;
                    }}
                    className="w-10 h-10 sm:w-12 sm:h-12 text-center text-2xl font-bold p-0 border border-gray-400"
                    disabled={isVerifying}
                  />
                ))}
              </div>
              <div className="flex flex-col items-center gap-4">
                <Button
                  type="submit"
                  size="lg"
                  className="w-full max-w-xs"
                  disabled={isVerifying || otp.join("").length !== 6}
                >
                  <Loader state={isVerifying} textLoading="Verifying...">
                    <span>Verify Email</span>
                  </Loader>
                </Button>
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Timer className="w-4 h-4" />
                  {canResend ? (
                    <Button
                      variant="link"
                      className="p-0 h-auto font-normal"
                      onClick={handleResend}
                    >
                      Resend OTP
                    </Button>
                  ) : (
                    <span>Resend OTP in {countdown}s</span>
                  )}
                </div>
              </div>
            </form>
          </>
        )}
      </CardContent>
    </Card>
  );
}
