export interface CategoryWithDeleteStatus {
  categoryId: string;
  categoryName: string;
  description: string;
  isDeleted: boolean;
  createdAt: string;
  createdBy: string;
}

export interface Category {
  categoryId: string;
  categoryName: string;
  description: string;
  createdAt: string;
  createdBy: string;
}

export interface CreateCategoryDto {
  name: string;
  description: string;
}

export interface UpdateCategoryDto {
  id: string;
  name?: string;
  description?: string;
}
