import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { z } from "zod"
import { toast } from "sonner"
import { Pencil, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import ProductService from "@/services/product-service"
import type { DetailedProductResponse } from "@/types/product"
import { parseUpdateErrors } from "@/lib/error-parser"

const updateSchema = z.object({
  name: z.string().max(200, "Name cannot exceed 200 characters.").optional(),
  description: z
    .string()
    .max(1000, "Description cannot exceed 1000 characters.")
    .optional(),
  price: z
    .number()
    .gt(0, "Price must be greater than 0.")
    .multipleOf(0.01, "Price must have at most 2 decimal places.")
    .optional(),
  sku: z
    .string()
    .max(128, "SKU must not exceed 128 characters.")
    .regex(
      /^[a-zA-Z0-9-_]+$/,
      "SKU must only contain letters, numbers, hyphens, and underscores."
    )
    .optional(),
})

type UpdateFormValues = z.infer<typeof updateSchema>

interface BasicInfoSectionProps {
  product: DetailedProductResponse
}

export function BasicInfoSection({ product }: BasicInfoSectionProps) {
  const [isEditing, setIsEditing] = useState(false)
  const queryClient = useQueryClient()

  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors },
  } = useForm<UpdateFormValues>({
    resolver: zodResolver(updateSchema),
    defaultValues: {
      name: product.name,
      description: product.description,
      price: product.price,
      sku: product.sku,
    },
  })

  function handleCancel() {
    reset({
      name: product.name,
      description: product.description,
      price: product.price,
      sku: product.sku,
    })
    setIsEditing(false)
  }

  const mutation = useMutation({
    mutationFn: (values: UpdateFormValues) =>
      ProductService.updateInfo(product.id, values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["product", product.id] })
      queryClient.invalidateQueries({ queryKey: ["products"] })
      toast.success("Product info updated.")
      setIsEditing(false)
    },
    onError: (error: any) => {
      const { fieldErrors, domainErrors } = parseUpdateErrors(error)

      // Map field errors onto form fields
      Object.entries(fieldErrors).forEach(([field, message]) => {
        setError(field as keyof UpdateFormValues, { message })
      })

      // Toast domain errors (e.g. SKU conflict)
      domainErrors.forEach((err) => {
        if (err.code === "Product.ProductSKUExisted") {
          setError("sku", { message: err.description })
        } else {
          toast.error(err.description)
        }
      })
    },
  })

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between pb-3">
        <CardTitle className="text-base">Basic Information</CardTitle>
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
              <Label htmlFor="name">Name</Label>
              <Input id="name" {...register("name")} />
              {errors.name && (
                <p className="text-sm text-destructive">
                  {errors.name.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                rows={4}
                {...register("description")}
              />
              {errors.description && (
                <p className="text-sm text-destructive">
                  {errors.description.message}
                </p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="price">Price</Label>
                <div className="relative">
                  <span className="absolute top-1/2 left-3 -translate-y-1/2 text-sm text-muted-foreground">
                    $
                  </span>
                  <Input
                    id="price"
                    type="number"
                    step="0.01"
                    className="pl-7"
                    {...register("price", { valueAsNumber: true })}
                  />
                </div>
                {errors.price && (
                  <p className="text-sm text-destructive">
                    {errors.price.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="sku">SKU</Label>
                <Input id="sku" {...register("sku")} />
                {errors.sku && (
                  <p className="text-sm text-destructive">
                    {errors.sku.message}
                  </p>
                )}
              </div>
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
          <div className="space-y-4">
            <Field label="Name" value={product.name} />
            <Field label="Description" value={product.description} />
            <div className="grid grid-cols-2 gap-4">
              <Field
                label="Price"
                value={new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                }).format(product.price)}
              />
              <Field label="SKU" value={product.sku} />
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  )
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <div className="space-y-1">
      <p className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
        {label}
      </p>
      <p className="text-sm">{value}</p>
    </div>
  )
}
