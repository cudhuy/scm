/**
 * Inventory Statistics Data Model
 */
export interface InventoryStatistics {
  totalIngredients: number;
  totalWarnings: number;
  totalInventories: number;
}

/**
 * Interface for paginated inventory items response
 */
export interface InventoryItemsResponse {
  items: InventoryItemDto[];
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Interface for inventory item DTO
 */
export interface InventoryItemDto {
  ingredientId: string;
  ingredientName: string;
  ingredientQuantity: string;
  price: string;
  branchName: string;
}

/**
 * Interface for inventory receipt DTO
 */
export interface InventoryReceiptDto {
  createAt: string;
  ingredientName: string;
  ingredientQuantity: string;
  branchName: string;
  unitPrice: string;
  totalPrice: string;
  inventoryReceiver: string;
}

/**
 * Interface for paginated inventory receipts response
 */
export interface InventoryReceiptsResponse {
  items: InventoryReceiptDto[];
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Interface for low stock alert DTO
 */
export interface LowStockAlertDto {
  ingredientId: string;
  ingredientName?: string;
  branchName?: string;
  ingredientQuantity?: string;
  importDate?: string;
  expiryDate?: string;
  alertMessage: string;
}

/**
 * Interface for paginated low stock alerts response
 */
export interface LowStockAlertsResponse {
  items: LowStockAlertDto[];
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

export interface IngredientDetailDto {
  ingredientId: string;
  name: string;
  branchName: string;
  quantity: string;
  lowStockThreshold: string;
  batchDetails: BatchDetailDto[];
  supplier: string;
  avgPrice: string;
  totalPrice: string;
}

export interface BatchDetailDto {
  quantity: string;
  importDate: string;
  expiryDate: string;
  unitPrice: string;
  status: string;
}

export interface RestockInventoryDto {
  branchId: string;
  ingredientId: string;
  ingredientName: string;
  ingredientUnit: string;
  quantityReceived: number;
  lowStockThreshold: number;
  unitPrice: number;
  expiryDate: Date;
  supplierName: string;
}
