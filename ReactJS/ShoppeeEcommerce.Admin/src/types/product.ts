import type { BaseCreatorResponse } from "./creator"
import type { DateRangesSortedPagedIncludeDeletedRequest } from "./filter-request"
import type { PagedList } from "./paged-list"

export interface AdminListProductsRequest extends DateRangesSortedPagedIncludeDeletedRequest {
  searchTerm?: string
  status?: "Draft" | "Published" | null
  minPrice?: number
  maxPrice?: number
}

export interface ListProductsResponse {
  id: string
  name: string
  description: string
  price: number
  sku: string
  createdDate: string
  isDeleted: boolean
  status: "Draft" | "Published"
  imgUrl?: string
}

export interface AdminListProductsResponse extends PagedList<ListProductsResponse> {}

export interface CreateProductRequest {
  name: string
  description: string
  price: number
  sku: string
  categoryIds: string[]
}

export interface CreateProductResponse {
  id: string
  name: string
  description: string
  price: number
  sku: string
  status: string
  categories: string[]
  createdAt: string
}

export interface CreateProductImageResponse {
  id: number
  url: string
  displayOrder: number
  isMain: boolean
  altText?: string
}

export interface DetailedProductResponse {
  id: string
  name: string
  description: string
  price: number
  sku: string
  status: string
  createdDate: string
  updatedDate?: string
  isDeleted: boolean
  deletedDate?: string
  creator?: BaseCreatorResponse
  images: ProductImageResponse[]
  categories: ProductCategoryResponse[]
}

export interface ProductImageResponse {
  id: number
  url: string
  isMain: boolean
  displayOrder: number
  altText?: string
}

export interface ProductCategoryResponse {
  id: string
  name: string
}
