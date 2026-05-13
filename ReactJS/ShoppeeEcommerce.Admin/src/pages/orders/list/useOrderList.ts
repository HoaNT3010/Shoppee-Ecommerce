import OrderService from "@/services/order-service"
import type { AdminListOrdersRequest } from "@/types/order"
import { useQuery } from "@tanstack/react-query"

export function useOrderList(params: AdminListOrdersRequest) {
  return useQuery({
    queryKey: ["orders", params],
    queryFn: () => OrderService.list(params),
    placeholderData: (prev) => prev,
  })
}
