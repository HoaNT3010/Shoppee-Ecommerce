import type { BaseCreatorResponse } from "./creator"
import type { DateRangesSortedPagedIncludeDeletedRequest } from "./filter-request"
import type { PagedList } from "./paged-list"

export interface CreateCategoryRequest {
  name: string
  description: string
}

export interface UpdateCategoryRequest {
  categoryId: string
  name?: string
  description?: string
}

export interface ShortCategoryResponse {
  id: string
  name: string
  description: string
  createdDate: string
  isDeleted: boolean
}

export interface ListCategoriesRequest extends DateRangesSortedPagedIncludeDeletedRequest {
  searchTerm?: string
}

export interface ListCategoriesResponse extends PagedList<ShortCategoryResponse> {}

export interface DetailedCategoryResponse {
  id: string
  name: string
  description: string
  createdDate: string
  updatedDate?: string
  isDeleted: boolean
  deletedDate?: string
  creator?: BaseCreatorResponse
}
