export interface PagedRequest {
  pageIndex?: number
  pageSize?: number
}

export interface SortedPagedRequest extends PagedRequest {
  sortBy?: string
  sortDesc?: boolean
}

export interface SortedPagedIncludeDeletedRequest extends SortedPagedRequest {
  includeDeleted?: boolean
}

export interface DateRangesSortedPagedIncludeDeletedRequest extends SortedPagedIncludeDeletedRequest {
  fromCreatedDate?: string
  toCreatedDate?: string
}
