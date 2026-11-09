'use client';

import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Check, X } from 'lucide-react';
import zxcvbn from 'zxcvbn';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { cn } from '@/lib/utils';

const passwordRequirements = [
  { label: 'Dài ít nhất 8 ký tự', regex: /.{8,}/ },
  { label: 'Chứa ít nhất một chữ cái in hoa', regex: /[A-Z]/ },
  { label: 'Chứa ít nhất một chữ cái thường', regex: /[a-z]/ },
  { label: 'Chứa ít nhất một số', regex: /\d/ },
  {
    label: 'Chứa ít nhất một ký tự đặc biệt',
    regex: /[!@#$%^&*(),.?":{}|<>]/,
  },
];

interface PasswordStrengthProps {
  password: string;
}

export default function PasswordStrength({ password }: PasswordStrengthProps) {
  const [score, setScore] = useState(0);
  const [animateProgress, setAnimateProgress] = useState(0);

  const strengthLabels = ['Rất yếu', 'Yếu', 'Khá', 'Mạnh', 'Rất mạnh'];
  const strengthColors = [
    'bg-red-500',
    'bg-orange-500',
    'bg-yellow-500',
    'bg-green-500',
    'bg-emerald-500',
  ];

  useEffect(() => {
    if (password) {
      const result = zxcvbn(password);
      setScore(result.score);

      setAnimateProgress(0);
      const timer = setTimeout(() => {
        setAnimateProgress((result.score + 1) * 20);
      }, 100);

      return () => clearTimeout(timer);
    } else {
      setScore(0);
      setAnimateProgress(0);
    }
  }, [password]);

  const checkRequirement = (requirement: { regex: RegExp }) => {
    return password ? requirement.regex.test(password) : false;
  };

  return (
    <Card className="border-none bg-muted/50 overflow-hidden">
      <div className="absolute top-0 left-0 w-full" />

      <CardHeader className="space-y-1">
        <CardTitle className="text-2xl">Độ mạnh mật khẩu</CardTitle>
        <CardDescription>Độ mạnh mật khẩu của bạn được hiển thị bên dưới</CardDescription>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="space-y-2">
          <div className="flex justify-between text-sm">
            <span>Độ mạnh:</span>
            <motion.span
              className={cn(
                'font-medium',
                score === 4 && 'text-emerald-500',
                score === 3 && 'text-green-500',
                score === 2 && 'text-yellow-500',
                score === 1 && 'text-orange-500',
                score === 0 && 'text-red-500',
              )}
              initial={{ opacity: 0, y: -10 }}
              animate={{ opacity: 1, y: 0 }}
              key={score}
            >
              {password ? strengthLabels[score] : 'Chưa đặt'}
            </motion.span>
          </div>

          <div className="h-2 w-full bg-muted rounded-full overflow-hidden">
            <motion.div
              className={cn('h-full rounded-full', strengthColors[score])}
              initial={{ width: '0%' }}
              animate={{ width: `${animateProgress}%` }}
              transition={{ type: 'spring', stiffness: 300, damping: 30 }}
            />
          </div>
        </div>

        <div className="space-y-2">
          {passwordRequirements.map((requirement, index) => {
            const isValid = checkRequirement(requirement);
            return (
              <motion.div
                key={index}
                className="flex items-center gap-2 text-sm"
                initial={{ opacity: 0, x: -10 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: index * 0.1 }}
              >
                <motion.div
                  className={cn(
                    'size-5 rounded-full flex items-center justify-center',
                    isValid ? 'bg-green-500/20 text-green-600' : 'bg-muted text-muted-foreground',
                  )}
                  animate={{
                    scale: isValid ? [1, 1.2, 1] : 1,
                    backgroundColor: isValid
                      ? 'rgba(34, 197, 94, 0.2)'
                      : 'rgba(217, 217, 217, 0.2)',
                  }}
                  transition={{ duration: 0.3 }}
                >
                  {isValid ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                </motion.div>
                <span className={cn(isValid && 'text-green-600')}>{requirement.label}</span>
              </motion.div>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
}
