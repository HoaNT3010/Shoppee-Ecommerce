import type { AdminListProductsRequest } from "@/types/product"
import type { SortingState } from "@tanstack/react-table"
import { useReducer } from "react"

const DEFAULT_PARAMS: AdminListProductsRequest = {
  pageIndex: 1,
  pageSize: 5,
}

type Action =
  | { type: "SET_SEARCH"; payload: string }
  | { type: "SET_SORTING"; payload: SortingState }
  | { type: "SET_INCLUDE_DELETED"; payload: boolean }
  | { type: "SET_DATE_RANGE"; payload: { fromDate?: string; toDate?: string } }
  | { type: "SET_PAGINATION"; payload: { pageIndex: number; pageSize: number } }
  | { type: "SET_STATUS"; payload: "Draft" | "Published" | null }
  | {
      type: "SET_PRICE_RANGE"
      payload: { minPrice?: number; maxPrice?: number }
    }

function reducer(
  state: AdminListProductsRequest,
  action: Action
): AdminListProductsRequest {
  switch (action.type) {
    case "SET_SEARCH":
      return { ...state, searchTerm: action.payload, pageIndex: 1 } // reset page
    case "SET_SORTING": {
      const sort = action.payload[0]
      return {
        ...state,
        sortBy: sort?.id,
        sortDesc: sort?.desc,
        pageIndex: 1,
      }
    }
    case "SET_PAGINATION":
      return {
        ...state,
        pageIndex: action.payload.pageIndex,
        pageSize: action.payload.pageSize,
      }
    case "SET_INCLUDE_DELETED":
      return { ...state, includeDeleted: action.payload, pageIndex: 1 }
    case "SET_STATUS":
      return { ...state, status: action.payload, pageIndex: 1 }
    case "SET_PRICE_RANGE":
      return {
        ...state,
        minPrice: action.payload.minPrice,
        maxPrice: action.payload.maxPrice,
        pageIndex: 1,
      }
    case "SET_DATE_RANGE":
      return { ...state, ...action.payload, pageIndex: 1 }
    default:
      return state
  }
}

export function useProductTableState() {
  const [params, dispatch] = useReducer(reducer, DEFAULT_PARAMS)
  return { params, dispatch }
}
