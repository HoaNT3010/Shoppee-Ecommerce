import ProductService from "@/services/product-service"
import type { AdminListProductsRequest } from "@/types/product"
import { useQuery } from "@tanstack/react-query"

export function useProductList(params: AdminListProductsRequest) {
  return useQuery({
    queryKey: ["products", params],
    queryFn: () => ProductService.list(params),
    placeholderData: (prev) => prev,
  })
}
