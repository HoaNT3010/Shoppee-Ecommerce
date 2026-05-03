import { useQuery } from "@tanstack/react-query"
import ProductService from "@/services/product-service"

export function useProductSummary(productId: string) {
  return useQuery({
    queryKey: ["product", productId],
    queryFn: () => ProductService.getById(productId),
    enabled: !!productId,
  })
}
