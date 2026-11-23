import VerifyEmailForm from "@/components/global/auth/verify-email-form";
import { Shield } from "lucide-react";
import React, { Suspense } from "react";

export default function VerifyEmailPage() {
  const fallback = (
    <div className="min-h-screen flex items-center justify-center p-4">
      <div className="bg-card rounded-lg shadow-lg p-8 border border-border">
        <div className="text-center">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 mb-4">
            <Shield className="h-8 w-8 text-primary animate-pulse" />
          </div>
          <h1 className="text-2xl font-bold mb-2">Loading...</h1>
        </div>
      </div>
    </div>
  );

  return (
    <Suspense fallback={fallback}>
      <VerifyEmailForm />
    </Suspense>
  );
}
