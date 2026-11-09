'use client';

import { useState, useEffect } from 'react';
import { Sun, Moon } from 'lucide-react';

export function TimeDisplay() {
  // Initialize with null to avoid hydration mismatch
  const [currentTime, setCurrentTime] = useState<Date | null>(null);

  useEffect(() => {
    // Set initial time only on client-side
    setCurrentTime(new Date());

    // Update time every minute
    const interval = setInterval(() => {
      setCurrentTime(new Date());
    }, 60000);

    return () => clearInterval(interval);
  }, []);

  // If currentTime is null (during server rendering or initial client render)
  // return a placeholder with the same structure
  if (!currentTime) {
    return (
      <div className="flex items-center gap-2 bg-muted/30 px-3 py-1.5 rounded-full text-sm font-medium">
        <Moon className="h-4 w-4 text-indigo-400" />
        <span>--:--</span>
      </div>
    );
  }

  const hours = currentTime.getHours();
  const minutes = currentTime.getMinutes();
  const formattedTime = `${hours.toString().padStart(2, '0')}:${minutes
    .toString()
    .padStart(2, '0')}`;

  // Determine if it's day or night (day: 6AM-6PM, night: 6PM-6AM)
  const isDaytime = hours >= 6 && hours < 18;

  return (
    <div className="flex items-center gap-2 bg-muted/30 px-3 py-1.5 rounded-full text-sm font-medium">
      {isDaytime ? (
        <Sun className="h-4 w-4 text-amber-500" />
      ) : (
        <Moon className="h-4 w-4 text-indigo-400" />
      )}
      <span>{formattedTime}</span>
    </div>
  );
}
