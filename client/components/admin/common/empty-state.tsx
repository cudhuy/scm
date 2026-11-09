'use client';

import { motion } from 'framer-motion';
import type { ReactNode } from 'react';
import { Button } from '@/components/ui/button';
import Image from 'next/image';
import { cn } from '@/lib/utils';

interface EmptyStateProps {
  title: string;
  description: string;
  icon?: ReactNode;
  action?: {
    label: string;
    onClick: () => void;
  };
  className?: string;
  variant?: 'default' | 'bordered' | 'subtle';
}

export function EmptyState({
  title,
  description,
  icon,
  action,
  className = '',
  variant = 'default',
}: EmptyStateProps) {
  // Animation variants
  const containerVariants = {
    hidden: { opacity: 0, y: 20 },
    visible: {
      opacity: 1,
      y: 0,
      transition: {
        duration: 0.5,
        when: 'beforeChildren',
        staggerChildren: 0.1,
      },
    },
  };

  const itemVariants = {
    hidden: { opacity: 0, y: 10 },
    visible: { opacity: 1, y: 0, transition: { duration: 0.4 } },
  };

  // Determine container styles based on variant
  const containerStyles = cn(
    'flex flex-col items-center justify-center w-full mx-auto p-4 rounded-xl transition-all duration-300',
    {
      'bg-white dark:bg-gray-800 shadow-lg border border-gray-100 dark:border-gray-700':
        variant === 'default',
      'border-2 border-dashed border-gray-200 dark:border-gray-700 bg-gray-50/50 dark:bg-gray-800/50':
        variant === 'bordered',
      'bg-gray-50/80 dark:bg-gray-800/30': variant === 'subtle',
    },
    className,
  );

  return (
    <motion.div
      className="w-full px-4 py-2"
      initial="hidden"
      animate="visible"
      variants={containerVariants}
    >
      <motion.div className={containerStyles}>
        <motion.div className="relative" variants={itemVariants}>
          {icon ? (
            <div className="text-gray-400 dark:text-gray-500">{icon}</div>
          ) : (
            <div className="relative">
              <motion.div
                className="absolute inset-0 rounded-full bg-gray-100 dark:bg-gray-400 opacity-50 blur-lg"
                initial={{ scale: 0.8, opacity: 0 }}
                animate={{ scale: 1, opacity: 0.5 }}
                transition={{
                  duration: 1,
                  repeat: Number.POSITIVE_INFINITY,
                  repeatType: 'reverse',
                }}
              />
              <Image
                src="/empty-state.png"
                alt="Empty state"
                width={80}
                height={80}
                className="relative z-10"
              />
            </div>
          )}
        </motion.div>

        <motion.h3
          className="mt-4 text-xl font-semibold text-gray-800 dark:text-gray-100"
          variants={itemVariants}
        >
          {title}
        </motion.h3>

        <motion.p
          className="mt-3 text-sm text-center text-gray-500 dark:text-gray-400 leading-relaxed max-w-lg"
          variants={itemVariants}
        >
          {description}
        </motion.p>

        {action && (
          <motion.div
            className="mt-6"
            variants={itemVariants}
            whileHover={{ scale: 1.03 }}
            whileTap={{ scale: 0.97 }}
          >
            <Button onClick={action.onClick} className="px-6 font-medium" size="lg">
              {action.label}
            </Button>
          </motion.div>
        )}
      </motion.div>
    </motion.div>
  );
}
