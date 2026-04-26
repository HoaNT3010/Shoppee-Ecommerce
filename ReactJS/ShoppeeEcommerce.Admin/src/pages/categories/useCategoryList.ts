import { useQuery } from "@tanstack/react-query"
import CategoryService from "@/services/category-service"
import type { ListCategoriesRequest } from "@/types/category"

export function useCategoryList(params: ListCategoriesRequest) {
  return useQuery({
    queryKey: ["categories", params], // re-fetches automatically when params change
    queryFn: () => CategoryService.list(params),
    placeholderData: (prev) => prev, // keeps old data visible while loading
  })
}
