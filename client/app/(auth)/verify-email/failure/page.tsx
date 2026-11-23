"use client";

import { useState } from "react";
import { motion } from "framer-motion";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { AlertCircle } from "lucide-react";
import { toast } from "sonner";

export default function VerifyEmailFailurePage() {
  const [email, setEmail] = useState("");

  const handleResend = (e: React.FormEvent) => {
    e.preventDefault();

    toast.message("Verification Email Sent", {
      description: "Please check your inbox for the new verification link.",
    });
  };

  return (
    <Card className="w-full max-w-sm mx-auto">
      <CardHeader>
        <CardTitle className="text-3xl font-bold text-center">
          Verification Failed
        </CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col items-center p-8">
        <motion.div
          initial={{ rotate: 0 }}
          animate={{ rotate: 360 }}
          transition={{ duration: 0.5 }}
        >
          <AlertCircle className="w-32 h-32 text-red-500 mb-6" />
        </motion.div>
        <p className="text-center text-xl mb-6">
          Oops! We couldn&apos;t verify your email. Please try again.
        </p>
        <form onSubmit={handleResend} className="w-full max-w-md space-y-6">
          <Input
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="text-lg p-6"
          />
          <Button type="submit" size="lg" className="w-full">
            Resend Verification Email
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
