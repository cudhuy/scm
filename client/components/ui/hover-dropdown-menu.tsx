"use client";

import { ReactNode } from "react";

interface HoverDropdownProps {
  trigger: ReactNode;
  items: { item: ReactNode | string; custom?: boolean; onClick?: () => void }[];
  className?: string;
}

export default function HoverDropdown({
  trigger,
  items,
  className,
}: HoverDropdownProps) {
  return (
    <div className="group relative">
      {trigger}
      <div
        className={`absolute right-0 top-14 z-10 w-40 space-y-1 bg-[#1E1E1E] p-5 rounded-[10px] shadow-[0px_4px_10px_rgba(34,34,34,0.15), 5px_20px_40px_rgba(34,34,34,0.08)]
                  opacity-0 invisible translate-y-2 transition-all duration-300 ease-in-out 
                  group-hover:opacity-100 group-hover:visible group-hover:translate-y-0 ${className}`}
      >
        {items.map((item, index) =>
          item.custom ? (
            <div key={index}>{item.item}</div>
          ) : (
            <div
              key={index}
              className="block text-sm relative cursor-pointer text-[#E0E0E0] 
              after:absolute after:left-0 after:bottom-0 after:h-[4px] after:w-0 after:opacity-50 after:rounded-[50px]
            after:bg-[#be9c79] after:transition-all after:duration-300 hover:after:w-5/12"
              onClick={() => {
                item.onClick?.();
              }}
            >
              {item.item}
            </div>
          )
        )}
      </div>
    </div>
  );
}
