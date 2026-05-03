import { useLocation } from "react-router"
import { useQuery } from "@tanstack/react-query"
import ProductService from "@/services/product-service"

export function useProductImages(productId: string) {
  const location = useLocation()
  const stateImages = location.state?.images

  // Guard against the state being a non-array (e.g. wrapped response object)
  const validStateImages = Array.isArray(stateImages) ? stateImages : undefined

  const { data: fetchedImages, isLoading } = useQuery({
    queryKey: ["product-images", productId],
    queryFn: () => ProductService.getImages(productId),
    enabled: !validStateImages,
  })

  return {
    images: validStateImages ?? fetchedImages ?? [],
    isLoading: !validStateImages && isLoading,
  }
}
