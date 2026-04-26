import type { ListCategoriesRequest } from "@/types/category"
import type { SortingState } from "@tanstack/react-table"
import { useReducer } from "react"

const DEFAULT_PARAMS: ListCategoriesRequest = {
  pageIndex: 1,
  pageSize: 5,
}

type Action =
  | { type: "SET_SEARCH"; payload: string }
  | { type: "SET_SORTING"; payload: SortingState }
  | { type: "SET_PAGE"; payload: number }
  | { type: "SET_PAGE_SIZE"; payload: number }
  | { type: "SET_INCLUDE_DELETED"; payload: boolean }
  | { type: "SET_DATE_RANGE"; payload: { fromDate?: string; toDate?: string } }
  | { type: "SET_PAGINATION"; payload: { pageIndex: number; pageSize: number } }

function reducer(
  state: ListCategoriesRequest,
  action: Action
): ListCategoriesRequest {
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

    case "SET_PAGE":
      return { ...state, pageIndex: action.payload }

    case "SET_PAGE_SIZE":
      return { ...state, pageSize: action.payload, pageIndex: 1 }

    case "SET_INCLUDE_DELETED":
      return { ...state, includeDeleted: action.payload, pageIndex: 1 }

    case "SET_DATE_RANGE":
      return { ...state, ...action.payload, pageIndex: 1 }

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

export function useCategoryTableState() {
  const [params, dispatch] = useReducer(reducer, DEFAULT_PARAMS)
  return { params, dispatch }
}
