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
