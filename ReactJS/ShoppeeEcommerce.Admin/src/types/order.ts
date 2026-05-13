import type { PagedList } from "./paged-list"

export interface AdminListOrdersRequest {
  minPrice?: number
  maxPrice?: number
  status?:
    | "Pending"
    | "AwaitingPayment"
    | "Paid"
    | "Completed"
    | "Cancelled"
    | "Refunded"
    | null
  fromCreatedDate?: string
  toCreatedDate?: string
  // currently allowed sort values: createdDate, totalPrice
  sortBy?: string
  sortDesc?: boolean
  pageIndex?: number
  pageSize?: number
}

export interface ListOrderResponse {
  id: string
  status: string
  totalPrice: number
  createdDate: string
  updatedDate?: string
  userId: string
  lineItemsCount: number
}

export interface AdminListOrdersResponse extends PagedList<ListOrderResponse> {}
