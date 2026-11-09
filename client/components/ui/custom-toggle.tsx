'use client';

import { cn } from '@/lib/utils';

interface CustomToggleProps {
  checked?: boolean;
  onCheckedChange?: (checked: boolean) => void;
  disabled?: boolean;
  className?: string;
  id?: string;
}

export function CustomToggle({
  checked = false,
  onCheckedChange,
  disabled = false,
  className = '',
  id,
}: CustomToggleProps) {
  const handleChange = () => {
    if (disabled) return;
    onCheckedChange?.(!checked);
  };

  return (
    <label
      className={cn(
        'relative inline-flex items-center',
        disabled ? 'cursor-not-allowed' : 'cursor-pointer',
        className,
      )}
    >
      <input
        type="checkbox"
        className="sr-only peer"
        checked={checked}
        onChange={handleChange}
        disabled={disabled}
        id={id}
      />
      <div
        className={cn(
          'group peer ring-0 rounded-full outline-hidden duration-300 after:duration-300 w-16 h-8 shadow-md',
          "after:content-[''] after:rounded-full after:absolute after:bg-gray-50 dark:after:bg-gray-700 after:outline-hidden after:h-6 after:w-6 after:top-1 after:left-1",
          'after:flex after:justify-center after:items-center peer-checked:after:translate-x-8 peer-hover:after:scale-95',
          checked ? 'bg-emerald-500 dark:bg-teal-600' : 'bg-rose-400 dark:bg-red-600/70',
          disabled ? 'opacity-50' : '',
        )}
      >
        {/* Unlocked icon - visible when checked (active) */}
        <svg
          className="absolute top-1 left-8 stroke-gray-900 dark:stroke-gray-100 w-6 h-6"
          height="100"
          preserveAspectRatio="xMidYMid meet"
          viewBox="0 0 100 100"
          width="100"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            className="svg-fill-primary fill-gray-900 dark:fill-gray-100"
            d="M50,18A19.9,19.9,0,0,0,30,38v8a8,8,0,0,0-8,8V74a8,8,0,0,0,8,8H70a8,8,0,0,0,8-8V54a8,8,0,0,0-8-8H38V38a12,12,0,0,1,23.6-3,4,4,0,1,0,7.8-2A20.1,20.1,0,0,0,50,18Z"
          />
        </svg>
        {/* Locked icon - visible when unchecked (inactive) */}
        <svg
          className="absolute top-1 left-1 stroke-gray-900 dark:stroke-gray-100 w-6 h-6"
          height="100"
          preserveAspectRatio="xMidYMid meet"
          viewBox="0 0 100 100"
          width="100"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M30,46V38a20,20,0,0,1,40,0v8a8,8,0,0,1,8,8V74a8,8,0,0,1-8,8H30a8,8,0,0,1-8-8V54A8,8,0,0,1,30,46Zm32-8v8H38V38a12,12,0,0,1,24,0Z"
            fillRule="evenodd"
            className="fill-gray-900 dark:fill-gray-100"
          />
        </svg>
      </div>
    </label>
  );
}
