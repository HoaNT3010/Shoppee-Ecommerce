// pages/products/components/PublishErrors.tsx
import { AlertCircle } from "lucide-react"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import type { ApiError } from "@/types/common"

// Maps error codes to actionable guidance
const ERROR_HINTS: Record<string, string> = {
  "Product.MissingImages": "Go back to step 2 to upload images.",
  "Product.MissingMainImage": "Go back to step 3 to set a main image.",
  "Product.MissingCategories": "Go back to step 1 to add categories.",
  "Product.InvalidPrice": "Go back to step 1 to fix the price.",
}

interface PublishErrorsProps {
  errors: ApiError[]
}

export function PublishErrors({ errors }: PublishErrorsProps) {
  if (errors.length === 0) return null

  return (
    <Alert variant="destructive">
      <AlertCircle className="h-4 w-4" />
      <AlertTitle>Product cannot be published yet</AlertTitle>
      <AlertDescription>
        <ul className="mt-2 space-y-2">
          {errors.map((err) => (
            <li key={err.code} className="space-y-0.5">
              <p className="font-medium">{err.description}</p>
              {ERROR_HINTS[err.code] && (
                <p className="text-xs opacity-80">{ERROR_HINTS[err.code]}</p>
              )}
            </li>
          ))}
        </ul>
      </AlertDescription>
    </Alert>
  )
}
