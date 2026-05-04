import api from "@/lib/api"
import type { ListCustomersRequest, ListCustomersResponse } from "@/types/user"

const UserService = {
  list: async (params: ListCustomersRequest) => {
    const { data } = await api.get<ListCustomersResponse>(
      "/admin/users/customers",
      { params }
    )
    return data
  },
}

export default UserService
