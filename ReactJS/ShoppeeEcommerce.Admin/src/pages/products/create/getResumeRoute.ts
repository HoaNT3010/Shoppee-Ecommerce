import type { DetailedProductResponse } from "@/types/product"

export function getResumeRoute(product: DetailedProductResponse): string {
  if (!product.images?.length) return `/products/new/${product.id}/images`
  if (!product.images.some((i) => i.isMain))
    return `/products/new/${product.id}/main-image`
  return `/products/new/${product.id}/publish`
}
