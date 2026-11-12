'use client';

import type React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Eye } from 'lucide-react';
import { formatCurrency, formatDateLocale } from '@/lib/utils';
import Image from 'next/image';
import type { ProductDetail, ProductIngredientDTO } from '@/types/product-admin';
import { Skeleton } from '@/components/ui/skeleton';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { memo, useCallback } from 'react';

interface ProductDetailViewProps {
  productId: string;
  productDetails: ProductDetail | null;
  isDetailDialogOpen: boolean;
  setIsDetailDialogOpen: (open: boolean) => void;
  isLoadingProductDetails: boolean;
  detailsErrorProduct: Error | null;
  onViewProductDetail: (productId: string) => void;
}

export const ProductDetailView = memo(function ProductDetailView({
  productId,
  productDetails,
  isDetailDialogOpen,
  setIsDetailDialogOpen,
  isLoadingProductDetails,
  detailsErrorProduct,
  onViewProductDetail,
}: ProductDetailViewProps) {
  const handleOpenDialog = useCallback(() => {
    if (!isLoadingProductDetails) {
      onViewProductDetail(productId);
      setIsDetailDialogOpen(true);
    }
  }, [productId, isLoadingProductDetails, onViewProductDetail, setIsDetailDialogOpen]);

  return (
    <Dialog open={isDetailDialogOpen} onOpenChange={setIsDetailDialogOpen}>
      <DialogContent className="max-w-3xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-xl">
            {isLoadingProductDetails ? (
              <Skeleton className="h-7 w-48" />
            ) : (
              `Chi tiết sản phẩm - ${productDetails?.name || 'Không tìm thấy'}`
            )}
          </DialogTitle>
        </DialogHeader>

        {isLoadingProductDetails ? (
          <ProductDetailSkeleton />
        ) : detailsErrorProduct ? (
          <ErrorDisplay error={detailsErrorProduct} onRetry={handleOpenDialog} />
        ) : productDetails ? (
          <ProductDetailContent productDetails={productDetails} />
        ) : (
          <EmptyState />
        )}
      </DialogContent>
    </Dialog>
  );
});

interface ProductDetailContentProps {
  productDetails: ProductDetail;
}

const ProductDetailContent = memo(function ProductDetailContent({
  productDetails,
}: ProductDetailContentProps) {
  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row gap-6">
        <div className="md:w-1/3">
          <div className="relative h-64 w-full rounded-md overflow-hidden border bg-background">
            {productDetails.image ? (
              <Image
                src={productDetails.image}
                alt={productDetails.name}
                fill
                sizes="(max-width: 768px) 100vw, 33vw"
                className="object-contain"
                priority
              />
            ) : (
              <div className="w-full h-full flex items-center justify-center text-muted-foreground">
                Không có hình ảnh
              </div>
            )}
          </div>
        </div>

        <div className="md:w-2/3 space-y-4">
          <div className="flex justify-between items-start">
            <h2 className="text-xl font-semibold">{productDetails.name}</h2>
            <StatusBadge status={productDetails.status} />
          </div>

          <div className="text-2xl font-bold text-primary">
            {formatCurrency(productDetails.price)}
          </div>

          <div className="grid grid-cols-2 gap-x-4 gap-y-2">
            <InfoItem label="Danh mục" value={productDetails.categoryName || 'N/A'} />
            {productDetails.createdAt && (
              <InfoItem label="Ngày tạo" value={formatDateLocale(productDetails.createdAt)} />
            )}
          </div>

          <div>
            <h4 className="text-sm font-medium text-muted-foreground mb-1">Mô tả</h4>
            <div className="overflow-auto max-h-[150px] md:max-h-[200px]">
              <p className="text-sm bg-muted/30 p-3 rounded-md whitespace-pre-wrap wrap-break-word">
                {productDetails.description || 'Không có mô tả'}
              </p>
            </div>
          </div>
        </div>
      </div>

      <Separator />

      {productDetails.ingredients && productDetails.ingredients.length > 0 && (
        <IngredientsSection ingredients={productDetails.ingredients} />
      )}
    </div>
  );
});

function ProductDetailSkeleton() {
  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row gap-6">
        <div className="md:w-1/3">
          <Skeleton className="h-64 w-full rounded-md" />
        </div>
        <div className="md:w-2/3 space-y-4">
          <div className="flex justify-between">
            <Skeleton className="h-7 w-48" />
            <Skeleton className="h-6 w-20" />
          </div>
          <Skeleton className="h-8 w-32" />
          <div className="grid grid-cols-2 gap-4">
            <Skeleton className="h-5 w-full" />
            <Skeleton className="h-5 w-full" />
          </div>
          <div className="space-y-2">
            <Skeleton className="h-5 w-24" />
            <Skeleton className="h-20 w-full" />
          </div>
        </div>
      </div>
      <Skeleton className="h-px w-full" />
      <div className="space-y-2">
        <Skeleton className="h-6 w-32" />
        <Skeleton className="h-32 w-full rounded-md" />
      </div>
    </div>
  );
}

interface ErrorDisplayProps {
  error: Error;
  onRetry: () => void;
}

function ErrorDisplay({ error, onRetry }: ErrorDisplayProps) {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-red-100">
        <span className="text-red-600 text-xl">!</span>
      </div>
      <h3 className="text-lg font-medium text-destructive">Không thể tải chi tiết sản phẩm</h3>
      <p className="text-sm text-muted-foreground">
        {error?.message || 'Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.'}
      </p>
      <Button variant="outline" onClick={onRetry}>
        Thử lại
      </Button>
    </div>
  );
}

function EmptyState() {
  return (
    <div className="py-8 text-center space-y-4">
      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted">
        <Eye className="h-6 w-6 text-muted-foreground" />
      </div>
      <h3 className="text-lg font-medium">Không có chi tiết sản phẩm</h3>
      <p className="text-sm text-muted-foreground">
        Không tìm thấy thông tin sản phẩm được yêu cầu.
      </p>
    </div>
  );
}

function StatusBadge({ status }: { status?: number }) {
  if (status === undefined) return <Badge variant="outline">N/A</Badge>;

  switch (status) {
    case 1:
      return <Badge variant="default">Đang hoạt động</Badge>;
    case 0:
      return <Badge variant="secondary">Ngừng hoạt động</Badge>;
    default:
      return <Badge variant="outline">Không xác định</Badge>;
  }
}

interface IngredientsSectionProps {
  ingredients: ProductIngredientDTO[];
}

const IngredientsSection = memo(function IngredientsSection({
  ingredients,
}: IngredientsSectionProps) {
  return (
    <div>
      <h3 className="text-lg font-semibold mb-3">Thành phần</h3>
      <div className="border rounded-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="bg-muted/50">
                <th className="p-3 text-left font-medium">Tên</th>
                <th className="p-3 text-right font-medium">Số lượng</th>
                <th className="p-3 text-left font-medium">Đơn vị</th>
              </tr>
            </thead>
            <tbody>
              {ingredients.map((ingredient, index) => (
                <tr key={index} className="border-t hover:bg-muted/30 transition-colors">
                  <td className="p-3">{ingredient.ingredientName || 'N/A'}</td>
                  <td className="p-3 text-right">{ingredient.quantityRequired}</td>
                  <td className="p-3">{ingredient.unit}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
});

interface InfoItemProps {
  label: string;
  value: React.ReactNode;
  valueClassName?: string;
}

const InfoItem = memo(function InfoItem({ label, value, valueClassName = '' }: InfoItemProps) {
  return (
    <div className="flex items-start gap-2">
      <span className="text-sm font-medium text-muted-foreground">{label}:</span>
      <span className={`text-sm ${valueClassName}`}>{value}</span>
    </div>
  );
});
