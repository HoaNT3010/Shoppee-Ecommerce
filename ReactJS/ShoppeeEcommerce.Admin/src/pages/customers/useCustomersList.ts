import UserService from "@/services/user-service"
import type { ListCustomersRequest } from "@/types/user"
import { useQuery } from "@tanstack/react-query"

export function useCustomersList(params: ListCustomersRequest) {
  return useQuery({
    queryKey: ["customers", params],
    queryFn: () => UserService.list(params),
    placeholderData: (prev) => prev,
  })
}
