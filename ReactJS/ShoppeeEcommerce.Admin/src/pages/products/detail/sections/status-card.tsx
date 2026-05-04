import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { ConfirmDialog } from "@/components/category/confirm-dialog"
import ProductService from "@/services/product-service"
import { useState } from "react"
import type { DetailedProductResponse } from "@/types/product"
import type { ApiError } from "@/types/common"

interface StatusCardProps {
  product: DetailedProductResponse
}

export function StatusCard({ product }: StatusCardProps) {
  const queryClient = useQueryClient()
  const [publishErrors, setPublishErrors] = useState<ApiError[]>([])
  const [confirmOpen, setConfirmOpen] = useState(false)

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ["product", product.id] })
    queryClient.invalidateQueries({ queryKey: ["products"] })
  }

  const publishMutation = useMutation({
    mutationFn: () => ProductService.publish(product.id),
    onSuccess: () => {
      toast.success("Product published.")
      setPublishErrors([])
      invalidate()
    },
    onError: (error: any) => {
      const errors: ApiError[] = error.response?.data ?? []
      setPublishErrors(errors)
    },
  })

  const isDeleted = product.isDeleted

  return (
    <Card>
      <CardHeader className="pb-3">
        <CardTitle className="text-base">Status</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Current status badges */}
        <div className="flex flex-wrap gap-2">
          <Badge variant={isDeleted ? "destructive" : "outline"}>
            {isDeleted ? "Deleted" : "Active"}
          </Badge>
          <Badge
            className={
              product.status === "Published"
                ? "bg-emerald-500 hover:bg-emerald-500/90"
                : ""
            }
            variant={product.status === "Published" ? "default" : "secondary"}
          >
            {product.status}
          </Badge>
        </div>

        {/* Publish errors */}
        {publishErrors.length > 0 && (
          <div className="space-y-1 rounded-md border border-destructive/50 bg-destructive/10 p-3">
            {publishErrors.map((err) => (
              <p key={err.code} className="text-xs text-destructive">
                {err.description}
              </p>
            ))}
          </div>
        )}

        <div className="space-y-2">
          {/* Publish — only show when Draft and not deleted */}
          {product.status === "Draft" && !isDeleted && (
            <Button
              className="w-full"
              onClick={() => publishMutation.mutate()}
              disabled={publishMutation.isPending}
            >
              {publishMutation.isPending ? "Publishing..." : "Publish Product"}
            </Button>
          )}

          {/* Soft delete / restore */}
          <Button
            variant="outline"
            className={`w-full ${!isDeleted ? "text-destructive hover:text-destructive" : ""}`}
            onClick={() => setConfirmOpen(true)}
          >
            {isDeleted ? "Restore Product" : "Delete Product"}
          </Button>
        </div>
      </CardContent>

      <ConfirmDialog
        open={confirmOpen}
        title={isDeleted ? "Restore Product" : "Delete Product"}
        description={
          isDeleted
            ? "This will restore the product and make it active again."
            : "This will soft-delete the product. It can be restored later."
        }
        confirmLabel={isDeleted ? "Restore" : "Delete"}
        variant={isDeleted ? "default" : "destructive"}
        invalidateKey={["products"]}
        onConfirm={() =>
          isDeleted
            ? ProductService.restore(product.id)
            : ProductService.softDelete(product.id)
        }
        onClose={() => {
          setConfirmOpen(false)
          invalidate()
        }}
        successMsg={
          isDeleted
            ? "Product restore successfully."
            : "Product soft-delete successfully."
        }
      />
    </Card>
  )
}
