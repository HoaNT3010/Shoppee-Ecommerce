import api from "@/lib/api"
import type {
  AdminListOrdersRequest,
  AdminListOrdersResponse,
  AdminViewOrderDetailResponse,
} from "@/types/order"

const OrderService = {
  list: async (params: AdminListOrdersRequest) => {
    const { data } = await api.get<AdminListOrdersResponse>("/admin/orders", {
      params,
    })
    return data
  },
  getById: async (id: string): Promise<AdminViewOrderDetailResponse> => {
    const { data } = await api.get(`/admin/orders/${id}`)
    return data
  },
}
export default OrderService
