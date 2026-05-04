import { useState } from "react"
import { useForm, Controller } from "react-hook-form"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Pencil, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { CategoryMultiSelect } from "@/components/category/category-multiselect"
import ProductService from "@/services/product-service"
import type { DetailedProductResponse } from "@/types/product"

interface CategoriesSectionProps {
  product: DetailedProductResponse
}

export function CategoriesSection({ product }: CategoriesSectionProps) {
  const [isEditing, setIsEditing] = useState(false)
  const queryClient = useQueryClient()

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: {
      categoryIds: product.categories.map((c) => c.id),
    },
  })

  function handleCancel() {
    reset({ categoryIds: product.categories.map((c) => c.id) })
    setIsEditing(false)
  }

  const mutation = useMutation({
    mutationFn: (values: { categoryIds: string[] }) =>
      ProductService.updateCategories(product.id, values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["product", product.id] })
      queryClient.invalidateQueries({ queryKey: ["products"] })
      toast.success("Categories updated.")
      setIsEditing(false)
    },
    onError: () => {
      toast.error("Failed to update categories. Please try again.")
    },
  })

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between pb-3">
        <CardTitle className="text-base">Categories</CardTitle>
        {!isEditing ? (
          <Button
            variant="ghost"
            size="sm"
            className="gap-2 text-muted-foreground"
            onClick={() => setIsEditing(true)}
          >
            <Pencil className="h-3.5 w-3.5" />
            Edit
          </Button>
        ) : (
          <Button
            variant="ghost"
            size="sm"
            className="gap-2 text-muted-foreground"
            onClick={handleCancel}
          >
            <X className="h-3.5 w-3.5" />
            Cancel
          </Button>
        )}
      </CardHeader>

      <CardContent>
        {isEditing ? (
          <form
            onSubmit={handleSubmit((v) => mutation.mutate(v))}
            className="space-y-4"
          >
            <div className="space-y-2">
              <Label>Categories</Label>
              <Controller
                control={control}
                name="categoryIds"
                render={({ field }) => (
                  <CategoryMultiSelect
                    value={field.value}
                    onChange={field.onChange}
                    error={
                      errors.categoryIds?.root?.message ??
                      errors.categoryIds?.message
                    }
                  />
                )}
              />
            </div>
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" onClick={handleCancel}>
                Cancel
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending ? "Saving..." : "Save Changes"}
              </Button>
            </div>
          </form>
        ) : (
          <div className="flex flex-wrap gap-1.5">
            {product.categories.length > 0 ? (
              product.categories.map((cat) => (
                <Badge key={cat.id} variant="secondary">
                  {cat.name}
                </Badge>
              ))
            ) : (
              <p className="text-sm text-muted-foreground">
                No categories assigned.
              </p>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  )
}
