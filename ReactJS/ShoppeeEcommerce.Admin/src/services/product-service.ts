import api from "@/lib/api"
import type {
  AdminListProductsRequest,
  AdminListProductsResponse,
} from "@/types/product"

const ProductService = {
  list: async (params: AdminListProductsRequest) => {
    const { data } = await api.get<AdminListProductsResponse>(
      "/admin/products",
      { params }
    )
    console.log(data)
    return data
  },
  // Soft Delete (Moves to Archive)
  softDelete: async (id: string): Promise<void> => {
    await api.delete(`/admin/products/${id}`)
  },

  // Restore Soft Deleted
  restore: async (id: string): Promise<void> => {
    await api.post(`/admin/products/${id}/restore`)
  },

  // Hard Delete (Permanent)
  hardDelete: async (id: string): Promise<void> => {
    await api.delete(`/admin/products/${id}/hard`)
  },
}

export default ProductService
