// pages/products/create/basicInfoSchema.ts
import { z } from "zod"

export const basicInfoSchema = z.object({
  name: z
    .string()
    .min(1, "Product name is required.")
    .max(200, "Product name cannot exceed 200 characters."),

  description: z
    .string()
    .min(1, "Product description is required.")
    .max(1000, "Product description cannot exceed 1000 characters."),

  price: z
    .number({
      error: (issue) =>
        issue.code === "invalid_type" && issue.input === undefined
          ? "Product price is required."
          : "Product price must be a number.",
    })
    .gt(0, "Product price must be greater than 0.")
    .multipleOf(0.01, "Product price must have at most 2 decimal places."),

  sku: z
    .string()
    .min(1, "Product SKU is required.")
    .max(128, "SKU must not exceed 128 characters.")
    .regex(
      /^[a-zA-Z0-9-_]+$/,
      "SKU must only contain letters, numbers, hyphens, and underscores."
    ),

  categoryIds: z
    .array(z.string().min(1))
    .min(1, "At least one category is required.")
    .max(10, "A product cannot belong to more than 10 categories.")
    .refine(
      (ids) => new Set(ids).size === ids.length,
      "Duplicate categories are not allowed."
    ),
})

export type BasicInfoFormValues = z.infer<typeof basicInfoSchema>
