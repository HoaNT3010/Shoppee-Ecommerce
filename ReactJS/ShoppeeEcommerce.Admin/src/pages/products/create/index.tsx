import { useState } from "react"
import { useForm, Controller } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation } from "@tanstack/react-query"
import { useNavigate } from "react-router"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { basicInfoSchema, type BasicInfoFormValues } from "./basicInfoSchema"
import ProductService from "@/services/product-service"
import type { ApiError } from "@/types/common"
import { StepLayout } from "@/components/product/step-layout"
import { CategoryMultiSelect } from "@/components/category/category-multiselect"

export default function BasicInfoStep() {
  const navigate = useNavigate()
  const [serverErrors, setServerErrors] = useState<ApiError[]>([])

  const {
    register,
    handleSubmit,
    control,
    setError,
    formState: { errors },
  } = useForm<BasicInfoFormValues>({
    resolver: zodResolver(basicInfoSchema),
    defaultValues: {
      name: "",
      description: "",
      price: undefined,
      sku: "",
      categoryIds: [],
    },
  })

  const mutation = useMutation({
    mutationFn: (values: BasicInfoFormValues) => ProductService.create(values),
    onSuccess: (data) => {
      navigate(`/products/new/${data.id}/images`)
    },
    onError: async (error: any) => {
      const errors: ApiError[] = error.response?.data ?? []

      // Map known field-level errors to their form fields
      errors.forEach((err) => {
        if (err.code === "Product.ProductSKUExisted") {
          setError("sku", { message: err.description })
        } else if (err.code === "Product.ProductNameExisted") {
          setError("name", { message: err.description })
        }
      })

      // Keep unmapped errors for the general error display
      const unmapped = errors.filter(
        (err) =>
          err.code !== "Product.ProductSKUExisted" &&
          err.code !== "Product.ProductNameExisted"
      )
      setServerErrors(unmapped)
    },
  })

  return (
    <StepLayout
      currentStep={1}
      title="Basic Information"
      description="Start by filling in the core details of your product."
    >
      <form
        onSubmit={handleSubmit((v) => mutation.mutate(v))}
        className="space-y-5"
      >
        {/* Name */}
        <div className="space-y-2">
          <Label htmlFor="name">
            Name <span className="text-destructive">*</span>
          </Label>
          <Input
            id="name"
            placeholder="e.g. Wireless Headphones Pro"
            {...register("name")}
          />
          {errors.name && (
            <p className="text-sm text-destructive">{errors.name.message}</p>
          )}
        </div>

        {/* Description */}
        <div className="space-y-2">
          <Label htmlFor="description">
            Description <span className="text-destructive">*</span>
          </Label>
          <Textarea
            id="description"
            placeholder="Describe your product..."
            rows={4}
            {...register("description")}
          />
          <div className="flex items-start justify-between">
            {errors.description ? (
              <p className="text-sm text-destructive">
                {errors.description.message}
              </p>
            ) : (
              <span />
            )}
          </div>
        </div>

        {/* Price */}
        <div className="space-y-2">
          <Label htmlFor="price">
            Price <span className="text-destructive">*</span>
          </Label>
          <div className="relative">
            <span className="absolute top-1/2 left-3 -translate-y-1/2 text-sm text-muted-foreground">
              $
            </span>
            <Input
              id="price"
              type="number"
              step="0.01"
              min="0.01"
              placeholder="0.00"
              className="pl-7"
              {...register("price", { valueAsNumber: true })}
            />
          </div>
          {errors.price && (
            <p className="text-sm text-destructive">{errors.price.message}</p>
          )}
        </div>

        {/* SKU */}
        <div className="space-y-2">
          <Label htmlFor="sku">
            SKU <span className="text-destructive">*</span>
          </Label>
          <Input id="sku" placeholder="e.g. WH-PRO-001" {...register("sku")} />
          <p className="text-xs text-muted-foreground">
            Letters, numbers, hyphens and underscores only.
          </p>
          {errors.sku && (
            <p className="text-sm text-destructive">{errors.sku.message}</p>
          )}
        </div>

        {/* Categories */}
        <div className="space-y-2">
          <Label>
            Categories <span className="text-destructive">*</span>
          </Label>
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

        {/* Unmapped server errors */}
        {serverErrors.length > 0 && (
          <div className="space-y-1 rounded-md border border-destructive/50 bg-destructive/10 p-3">
            {serverErrors.map((err) => (
              <p key={err.code} className="text-sm text-destructive">
                {err.description}
              </p>
            ))}
          </div>
        )}

        {/* Submit */}
        <div className="flex justify-end pt-2">
          <Button
            type="submit"
            disabled={mutation.isPending}
            className="min-w-32"
          >
            {mutation.isPending ? "Creating..." : "Next: Add Images →"}
          </Button>
        </div>
      </form>
    </StepLayout>
  )
}
