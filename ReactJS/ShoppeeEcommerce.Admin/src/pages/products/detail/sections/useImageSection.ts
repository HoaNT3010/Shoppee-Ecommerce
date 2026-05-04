import { useEffect, useState } from "react"
import { arrayMove } from "@dnd-kit/sortable"
import type { DragEndEvent } from "@dnd-kit/core"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import ProductService from "@/services/product-service"
import type { ProductImageResponse } from "@/types/product"

export function useImageSection(
  productId: string,
  initialImages: ProductImageResponse[]
) {
  const queryClient = useQueryClient()
  const [images, setImages] = useState(initialImages)
  const [isDirty, setIsDirty] = useState(false)

  useEffect(() => {
    setImages(initialImages)
  }, [initialImages])

  function invalidate() {
    queryClient.invalidateQueries({ queryKey: ["product", productId] })
    queryClient.invalidateQueries({ queryKey: ["products"] })
  }

  // ── Reorder ──────────────────────────────────────────────
  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over || active.id === over.id) return

    setImages((prev) => {
      const oldIndex = prev.findIndex((img) => img.id === active.id)
      const newIndex = prev.findIndex((img) => img.id === over.id)
      const reordered = arrayMove(prev, oldIndex, newIndex)
      // Reassign displayOrder based on new position
      return reordered.map((img, i) => ({ ...img, displayOrder: i + 1 }))
    })
    setIsDirty(true)
  }

  const reorderMutation = useMutation({
    mutationFn: () =>
      ProductService.reorderImages(productId, {
        orders: images.map((img) => ({
          imageId: img.id,
          displayOrder: img.displayOrder,
        })),
      }),
    onSuccess: () => {
      toast.success("Image order saved.")
      setIsDirty(false)
      invalidate()
    },
    onError: () => {
      toast.error("Failed to save image order.")
    },
  })

  // ── Set main image ────────────────────────────────────────
  const setMainMutation = useMutation({
    mutationFn: (imageId: number) =>
      ProductService.setMainImage(productId, imageId),
    onSuccess: () => {
      toast.success("Main image updated.")
      invalidate()
    },
    onError: () => {
      toast.error("Failed to set main image.")
    },
  })

  // ── Upload more images ────────────────────────────────────
  const uploadMutation = useMutation({
    mutationFn: (files: File[]) =>
      ProductService.uploadImages(productId, files),
    onSuccess: (newImages) => {
      toast.success(`${newImages.length} image(s) uploaded.`)
      invalidate()
    },
    onError: () => {
      toast.error("Failed to upload images.")
    },
  })

  return {
    images,
    isDirty,
    handleDragEnd,
    reorderMutation,
    setMainMutation,
    uploadMutation,
  }
}
