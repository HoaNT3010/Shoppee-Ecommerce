import { useState } from "react"
import { useNavigate, useParams } from "react-router"
import { useMutation } from "@tanstack/react-query"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"

import { useProductImages } from "./useProductImages"
import ProductService from "@/services/product-service"
import { StepLayout } from "@/components/product/step-layout"
import { ImageSelector } from "@/components/product/image-selector"

export default function MainImageStep() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { images, isLoading } = useProductImages(id!)

  // Pre-select the current main image if one already exists (draft pickup)
  const existingMain = images.find((img) => img.isMain)
  const [selectedId, setSelectedId] = useState<number | undefined>(
    existingMain?.id
  )

  // Keep selectedId in sync once images load (for draft pickup where
  // images load async and aren't available on first render)
  useState(() => {
    if (existingMain && !selectedId) {
      setSelectedId(existingMain.id)
    }
  })

  const mutation = useMutation({
    mutationFn: () => ProductService.setMainImage(id!, selectedId!),
    onSuccess: () => {
      toast.success("Main image set successfully.")
      navigate(`/products/new/${id}/publish`)
    },
    onError: () => {
      toast.error("Failed to set main image. Please try again.")
    },
  })

  return (
    <StepLayout
      currentStep={3}
      productId={id}
      title="Set Main Image"
      description="Choose which image will be displayed as the product's primary image."
    >
      <div className="space-y-6">
        {/* Image grid */}
        {isLoading ? (
          <LoadingSkeleton />
        ) : images.length === 0 ? (
          // Shouldn't normally happen, but handle it gracefully
          <div className="rounded-lg border border-dashed p-10 text-center">
            <p className="text-sm text-muted-foreground">
              No images found. Please go back and upload images first.
            </p>
          </div>
        ) : (
          <ImageSelector
            images={images}
            selectedId={selectedId}
            onSelect={setSelectedId}
          />
        )}

        {/* Selection hint */}
        {!isLoading && images.length > 0 && (
          <p className="text-sm text-muted-foreground">
            {selectedId
              ? "Image selected as main. Click another to change."
              : "Click an image to set it as the main product image."}
          </p>
        )}

        {/* Navigation */}
        <div className="flex justify-between">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate(`/products/new/${id}/images`)}
          >
            ← Back
          </Button>
          <Button
            onClick={() => mutation.mutate()}
            disabled={!selectedId || mutation.isPending || isLoading}
            className="min-w-32"
          >
            {mutation.isPending ? "Saving..." : "Next: Publish →"}
          </Button>
        </div>
      </div>
    </StepLayout>
  )
}

function LoadingSkeleton() {
  return (
    <div className="grid grid-cols-3 gap-3 sm:grid-cols-4">
      {Array.from({ length: 4 }).map((_, i) => (
        <Skeleton key={i} className="aspect-square rounded-lg" />
      ))}
    </div>
  )
}
