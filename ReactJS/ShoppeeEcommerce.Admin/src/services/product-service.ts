import api from "@/lib/api"
import type {
  AdminListProductsRequest,
  AdminListProductsResponse,
  CreateProductImageResponse,
  CreateProductRequest,
  CreateProductResponse,
  DetailedProductResponse,
} from "@/types/product"

const ProductService = {
  list: async (params: AdminListProductsRequest) => {
    const { data } = await api.get<AdminListProductsResponse>(
      "/admin/products",
      { params }
    )
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

  create: async (
    request: CreateProductRequest
  ): Promise<CreateProductResponse> => {
    const { data } = await api.post("/admin/products", request)
    return data
  },

  uploadImages: async (
    productId: string,
    files: File[]
  ): Promise<CreateProductImageResponse[]> => {
    const formData = new FormData()
    files.forEach((file) => formData.append("Images", file))
    const { data } = await api.post(
      `/admin/products/${productId}/images`,
      formData,
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    )
    return data
  },

  setMainImage: async (productId: string, imageId: number): Promise<void> => {
    await api.patch(`/admin/products/${productId}/images/${imageId}/main`)
  },

  publish: async (productId: string): Promise<void> => {
    await api.patch(`/admin/products/${productId}/publish`)
  },

  getImages: async (id: string): Promise<CreateProductImageResponse[]> => {
    const { data } = await api.get<CreateProductImageResponse[]>(
      `/admin/products/${id}/images`
    )
    return data
  },

  getById: async (id: string): Promise<DetailedProductResponse> => {
    const { data } = await api.get<DetailedProductResponse>(
      `/admin/products/${id}`
    )
    return data
  },
}

export default ProductService
