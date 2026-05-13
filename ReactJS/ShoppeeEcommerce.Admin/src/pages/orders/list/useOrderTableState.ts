import type { AdminListOrdersRequest } from "@/types/order"
import type { SortingState } from "@tanstack/react-table"
import { useReducer } from "react"

const DEFAULT_PARAMS: AdminListOrdersRequest = {
  pageIndex: 1,
  pageSize: 10,
}

type OrderStatus =
  | "Pending"
  | "AwaitingPayment"
  | "Paid"
  | "Completed"
  | "Cancelled"
  | "Refunded"
  | null

type Action =
  | { type: "SET_SORTING"; payload: SortingState }
  | { type: "SET_PAGINATION"; payload: { pageIndex: number; pageSize: number } }
  | { type: "SET_STATUS"; payload: OrderStatus }
  | {
      type: "SET_PRICE_RANGE"
      payload: { minPrice?: number; maxPrice?: number }
    }
  | {
      type: "SET_DATE_RANGE"
      payload: { fromCreatedDate?: string; toCreatedDate?: string }
    }

function reducer(
  state: AdminListOrdersRequest,
  action: Action
): AdminListOrdersRequest {
  switch (action.type) {
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
      return {
        ...state,
        fromCreatedDate: action.payload.fromCreatedDate,
        toCreatedDate: action.payload.toCreatedDate,
        pageIndex: 1,
      }
    default:
      return state
  }
}

export function useOrderTableState() {
  const [params, dispatch] = useReducer(reducer, DEFAULT_PARAMS)
  return { params, dispatch }
}
