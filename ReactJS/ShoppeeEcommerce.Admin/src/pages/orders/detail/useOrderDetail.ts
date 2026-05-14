import { useQuery } from "@tanstack/react-query"
import OrderService from "@/services/order-service"

export function useOrderDetail(id: string) {
  return useQuery({
    queryKey: ["order", id],
    queryFn: () => OrderService.getById(id),
    enabled: !!id,
  })
}
