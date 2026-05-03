import { useState } from "react"
import { useNavigate, useParams } from "react-router"
import { useMutation } from "@tanstack/react-query"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"

import type { CreateProductImageResponse } from "@/types/product"
import ProductService from "@/services/product-service"
import { StepLayout } from "@/components/product/step-layout"
import { ImageDropzone } from "@/components/product/image-dropzone"

const ACCEPTED_FORMATS = ["image/jpeg", "image/png", "image/webp"]

export default function ImagesStep() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [files, setFiles] = useState<File[]>([])
  const [validationError, setValidationError] = useState<string>()

  const mutation = useMutation({
    mutationFn: () => ProductService.uploadImages(id!, files),
    onSuccess: (images: CreateProductImageResponse[]) => {
      toast.success("Images uploaded successfully.")
      // Pass images to next step via navigation state
      // so MainImageStep doesn't need an extra fetch
      navigate(`/products/new/${id}/main-image`, { state: { images } })
    },
    onError: () => {
      toast.error("Failed to upload images. Please try again.")
    },
  })

  function handleSubmit() {
    // Frontend validation
    if (files.length === 0) {
      setValidationError("Please select at least one image.")
      return
    }

    const invalidFiles = files.filter((f) => !ACCEPTED_FORMATS.includes(f.type))
    if (invalidFiles.length > 0) {
      setValidationError(
        "Some files have unsupported formats. Use JPG, PNG, or WEBP."
      )
      return
    }

    setValidationError(undefined)
    mutation.mutate()
  }

  function handleFilesChange(newFiles: File[]) {
    setFiles(newFiles)
    if (newFiles.length > 0) setValidationError(undefined)
  }

  return (
    <StepLayout
      currentStep={2}
      productId={id}
      title="Product Images"
      description="Upload at least one image for your product. You can add up to 10 images."
    >
      <div className="space-y-6">
        <ImageDropzone
          files={files}
          onChange={handleFilesChange}
          error={validationError}
        />

        <div className="flex justify-between">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate(`/products/${id}/setup/images`)}
          >
            ← Back
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={mutation.isPending || files.length === 0}
            className="min-w-32"
          >
            {mutation.isPending ? "Uploading..." : "Next: Set Main Image →"}
          </Button>
        </div>
      </div>
    </StepLayout>
  )
}
