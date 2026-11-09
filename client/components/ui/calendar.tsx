"use client"

import * as React from "react"
import { ChevronLeft, ChevronRight } from "lucide-react"
import { DayPicker, getDefaultClassNames } from "react-day-picker"

import { cn } from "@/lib/utils"
import { buttonVariants } from "@/components/ui/button"

export type CalendarProps = React.ComponentProps<typeof DayPicker>

function Calendar({
  className,
  classNames,
  showOutsideDays = true,
  ...props
}: CalendarProps) {
  const defaultClassNames = getDefaultClassNames()
  return (
    <DayPicker
      showOutsideDays={showOutsideDays}
      className={cn("p-3", className)}
      classNames={{
        // NOTE: migrated to react-day-picker v10 keys.
        months: cn("flex flex-col sm:flex-row space-y-4 sm:space-x-4 sm:space-y-0", defaultClassNames.months),
        month: cn("flex w-full flex-col space-y-4", defaultClassNames.month),
        nav: cn("flex items-center gap-1", defaultClassNames.nav),
        button_previous: cn(
          buttonVariants({ variant: "outline" }),
          "size-7 bg-transparent p-0 opacity-50 hover:opacity-100 absolute left-1",
          defaultClassNames.button_previous
        ),
        button_next: cn(
          buttonVariants({ variant: "outline" }),
          "size-7 bg-transparent p-0 opacity-50 hover:opacity-100 absolute right-1",
          defaultClassNames.button_next
        ),
        month_caption: cn(
          "flex items-center justify-center h-10 pt-1 relative",
          defaultClassNames.month_caption
        ),
        caption_label: cn("text-sm font-medium select-none", defaultClassNames.caption_label),
        month_grid: cn("w-full border-collapse space-y-1", defaultClassNames.month_grid),
        weekdays: cn("flex", defaultClassNames.weekdays),
        weekday: cn(
          "text-muted-foreground rounded-md w-8 font-normal text-[0.8rem]",
          defaultClassNames.weekday
        ),
        week: cn("flex w-full mt-2", defaultClassNames.week),
        day: cn(
          "relative p-0 text-center text-sm focus-within:relative focus-within:z-20 [&:has([aria-selected])]:bg-accent [&:has([aria-selected].day-outside)]:bg-accent/50",
          props.mode === "range"
            ? "[&:has(>.range-end)]:rounded-r-md [&:has(>.range-start)]:rounded-l-md first:[&:has([aria-selected])]:rounded-l-md last:[&:has([aria-selected])]:rounded-r-md"
            : "[&:has([aria-selected])]:rounded-md",
          defaultClassNames.day
        ),
        day_button: cn(
          buttonVariants({ variant: "ghost" }),
          "size-8 p-0 font-normal aria-selected:opacity-100"
        ),
        range_start: cn("range-start rounded-l-md", defaultClassNames.range_start),
        range_end: cn("range-end rounded-r-md", defaultClassNames.range_end),
        range_middle: cn(
          "aria-selected:bg-accent aria-selected:text-accent-foreground rounded-none",
          defaultClassNames.range_middle
        ),
        selected: cn(
          "bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground focus:bg-primary focus:text-primary-foreground",
          defaultClassNames.selected
        ),
        today: cn("bg-accent text-accent-foreground", defaultClassNames.today),
        outside: cn(
          "text-muted-foreground opacity-50",
          defaultClassNames.outside
        ),
        disabled: cn("text-muted-foreground opacity-50", defaultClassNames.disabled),
        hidden: cn("invisible", defaultClassNames.hidden),
        ...classNames,
      }}
      components={{
        Chevron: ({ orientation, className, ...rest }) => {
          if (orientation === "left") {
            return <ChevronLeft className={cn("size-4", className)} {...rest} />
          }
          if (orientation === "right") {
            return <ChevronRight className={cn("size-4", className)} {...rest} />
          }
          // Down/up orientation used by dropdowns; render a neutral chevron so return type stays Element.
          return <ChevronRight className={cn("size-4 rotate-90", className)} {...rest} />
        },
      }}
      {...props}
    />
  )
}

export { Calendar }
