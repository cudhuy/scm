import { Skeleton } from '@/components/ui/skeleton';

export function SkeletonStats() {
  const skeletonItems = Array(4).fill(0);

  return (
    <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4 md:gap-6">
      {skeletonItems.map((_, index) => (
        <div key={`skeleton-${index}`} className="relative overflow-hidden rounded-xl h-full">
          <div className="bg-muted/10 absolute inset-0"></div>

          <div className="relative p-4 sm:p-6 h-full">
            <div className="flex items-start justify-between gap-3">
              <div className="space-y-2 w-full">
                <Skeleton className="h-4 w-24" />

                <div className="flex items-baseline gap-2">
                  <Skeleton className="h-8 w-16" />
                  <Skeleton className="h-4 w-12 rounded-full" />
                </div>
              </div>

              <Skeleton className="h-9 w-9 rounded-lg" />
            </div>

            <Skeleton className="h-3 w-32 mt-2" />

            <div className="absolute bottom-0 right-0 transform translate-x-1/4 translate-y-1/4">
              <Skeleton className="w-16 h-16 rounded-full opacity-20 blur-2xl" />
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
