import api from "@/lib/api"
import type {
  ListCategoriesResponse,
  ListCategoriesRequest,
  DetailedCategoryResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  BaseCategoryResponse,
} from "@/types/category"

const CategoryService = {
  list: async (params: ListCategoriesRequest) => {
    const { data } = await api.get<ListCategoriesResponse>(
      "/admin/categories",
      { params }
    )
    return data
  },
  getById: async (id: string) => {
    const { data } = await api.get<DetailedCategoryResponse>(
      `/admin/categories/${id}`
    )
    return data
  },
  create: async (request: CreateCategoryRequest): Promise<void> => {
    await api.post("/admin/categories", request)
  },

  // Patch update (Name and Description are optional)
  update: async (request: UpdateCategoryRequest): Promise<void> => {
    await api.patch("/admin/categories", request)
  },

  // Soft Delete (Moves to Archive)
  softDelete: async (id: string): Promise<void> => {
    await api.delete(`/admin/categories/${id}`)
  },

  // Restore Soft Deleted
  restore: async (id: string): Promise<void> => {
    await api.post(`/admin/categories/${id}/restore`)
  },

  // Hard Delete (Permanent)
  hardDelete: async (id: string): Promise<void> => {
    await api.delete(`/admin/categories/${id}/hard`)
  },
  generalList: async () => {
    const { data } = await api.get<BaseCategoryResponse[]>("/categories")
    return data
  },
}
export default CategoryService
