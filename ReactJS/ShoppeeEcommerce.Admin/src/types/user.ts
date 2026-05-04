import type { PagedRequest } from "./filter-request"
import type { PagedList } from "./paged-list"

export interface ListCustomersRequest extends PagedRequest {
  searchTerm?: string
}

export interface BaseCustomerResponse {
  id: string
  userName: string
  email: string
  firstName?: string
  lastName?: string
}

export interface ListCustomersResponse extends PagedList<BaseCustomerResponse> {}
