import { useState } from "react"
import { useNavigate, useParams } from "react-router"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { useProductSummary } from "./useProductSummary"
import ProductService from "@/services/product-service"
import type { ApiError } from "@/types/common"
import { StepLayout } from "@/components/product/step-layout"
import { PublishSummary } from "@/components/product/publish-summary"
import { PublishErrors } from "@/components/product/publish-errors"

export default function PublishStep() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [publishErrors, setPublishErrors] = useState<ApiError[]>([])

  const { data: product, isLoading } = useProductSummary(id!)

  const mutation = useMutation({
    mutationFn: () => ProductService.publish(id!),
    onSuccess: () => {
      // Invalidate product queries so the list and detail are fresh
      queryClient.invalidateQueries({ queryKey: ["products"] })
      queryClient.invalidateQueries({ queryKey: ["product", id] })

      toast.success("Product published successfully.")
      navigate(`/products`)
    },
    onError: (error: any) => {
      const errors: ApiError[] = error.response?.data ?? []

      if (errors.length > 0) {
        // Show validation errors inline — don't toast
        setPublishErrors(errors)
      } else {
        // Unexpected error — toast it
        toast.error("Failed to publish product. Please try again.")
      }
    },
  })

  function handlePublish() {
    setPublishErrors([]) // clear previous errors before retrying
    mutation.mutate()
  }

  return (
    <StepLayout
      currentStep={4}
      productId={id}
      title="Review & Publish"
      description="Review your product details before making it live."
    >
      <div className="space-y-6">
        {/* Product summary */}
        {isLoading ? (
          <LoadingSkeleton />
        ) : product ? (
          <PublishSummary product={product} />
        ) : null}

        {/* Publish validation errors */}
        <PublishErrors errors={publishErrors} />

        {/* Navigation */}
        <div className="flex justify-between">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate(`/products/new/${id}/main-image`)}
          >
            ← Back
          </Button>
          <Button
            onClick={handlePublish}
            disabled={mutation.isPending || isLoading}
            className="min-w-32"
          >
            {mutation.isPending ? "Publishing..." : "Publish Product"}
          </Button>
        </div>
      </div>
    </StepLayout>
  )
}

function LoadingSkeleton() {
  return (
    <div className="overflow-hidden rounded-lg border">
      <Skeleton className="h-40 w-full rounded-none" />
      <div className="space-y-4 p-4">
        <Skeleton className="h-5 w-48" />
        <Skeleton className="h-px w-full" />
        <div className="grid grid-cols-2 gap-3">
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="space-y-1">
              <Skeleton className="h-3 w-16" />
              <Skeleton className="h-4 w-24" />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
