import type { ListCustomersRequest } from "@/types/user"
import { useReducer } from "react"

const DEFAULT_PARAMS: ListCustomersRequest = {
  pageIndex: 1,
  pageSize: 5,
}

type Action =
  | { type: "SET_SEARCH"; payload: string }
  | { type: "SET_PAGINATION"; payload: { pageIndex: number; pageSize: number } }

function reducer(
  state: ListCustomersRequest,
  action: Action
): ListCustomersRequest {
  switch (action.type) {
    case "SET_SEARCH":
      return { ...state, searchTerm: action.payload, pageIndex: 1 } // reset page
    case "SET_PAGINATION":
      return {
        ...state,
        pageIndex: action.payload.pageIndex,
        pageSize: action.payload.pageSize,
      }
    default:
      return state
  }
}

export function useCustomerTableState() {
  const [params, dispatch] = useReducer(reducer, DEFAULT_PARAMS)
  return { params, dispatch }
}
