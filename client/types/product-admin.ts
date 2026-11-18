export interface Product {
  id: string;
  name: string;
  categoryName: string;
  description?: string;
  price: number;
  image: string;
  isDeleted: boolean;
}

export interface IngredientOfProduct {
  ingredientId: string;
  name?: string;
  quantityRequired: number;
  unit: string;
}

export interface IngredientOfProductNew {
  IngredientId: string;
  name?: string;
  QuantityRequired: number;
  unit: string;
}

export interface ProductDetail {
  id: string;
  name: string;
  description: string;
  price: number;
  status?: number;
  createdAt?: string;
  categoryName?: string;
  image: string;
  ingredients?: ProductIngredientDTO[];
}

export interface ProductIngredientDTO {
  ingredientId: string;
  ingredientName?: string;
  quantityRequired: number;
  unit?: string;
}

export interface CreateProductDto {
  categoryId: string;
  name: string;
  description?: string;
  price: number;
  image: string;
  ingredients: ProductIngredientDTO[];
}

export interface UpdateProductDto {
  id: string;
  categoryId: string;
  name: string;
  description?: string;
  price: number;
  image: string;
  ingredients: ProductIngredientDTO[];
}

export interface GetProductsResponse {
  items: Product[];
  totalItems: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}
