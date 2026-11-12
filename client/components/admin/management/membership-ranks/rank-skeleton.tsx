import { Skeleton } from '@/components/ui/skeleton';

export default function MembershipRankSkeleton() {
  return (
    <>
      <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-5 gap-4">
        {[...Array(5)].map((_, index) => (
          <div key={index} className="bg-card rounded-xl p-6 shadow-xs">
            <div className="flex justify-between items-start">
              <div>
                <Skeleton className="h-5 w-24 mb-2" />
                <Skeleton className="h-8 w-12" />
              </div>
              <Skeleton className="h-10 w-10 rounded-lg" />
            </div>
          </div>
        ))}
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
        {[...Array(6)].map((_, index) => (
          <div
            key={index}
            className="rounded-2xl overflow-hidden bg-linear-to-br from-gray-100/50 to-gray-200/50 dark:from-gray-800/20 dark:to-gray-700/20 border border-gray-200/50 dark:border-gray-800/50 p-6"
          >
            <div className="relative">
              <Skeleton className="h-12 w-12 rounded-xl mb-4" />

              <div className="mb-6">
                <Skeleton className="h-7 w-32 mb-2" />
                <Skeleton className="h-4 w-full mb-1" />
                <Skeleton className="h-4 w-2/3" />
              </div>

              <div className="flex justify-end gap-2">
                <Skeleton className="h-9 w-20 rounded-md" />
                <Skeleton className="h-9 w-24 rounded-md" />
              </div>
            </div>
          </div>
        ))}
      </div>
    </>
  );
}
