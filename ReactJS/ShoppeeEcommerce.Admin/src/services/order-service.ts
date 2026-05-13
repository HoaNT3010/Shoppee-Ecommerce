import api from "@/lib/api"
import type {
  AdminListOrdersRequest,
  AdminListOrdersResponse,
} from "@/types/order"

const OrderService = {
  list: async (params: AdminListOrdersRequest) => {
    const { data } = await api.get<AdminListOrdersResponse>("/admin/orders", {
      params,
    })
    return data
  },
}
export default OrderService
