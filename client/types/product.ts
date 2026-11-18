export interface Product {
  id: string;
  categoryName: string;
  name: string;
  price: number;
  description: string;
  image: string;
  isWishlist:boolean
}

export interface Category {
  categoryId: string;
  categoryName: string;
}

export interface ProductList {
  items: Product[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
  pageSize: number;
}
