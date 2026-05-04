import { useParams, useNavigate } from "react-router"
import { ChevronLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { useProductDetail } from "./useProductDetail"
import { BasicInfoSection } from "./sections/basic-info-section"
import { CategoriesSection } from "./sections/categories-section"
import { StatusCard } from "./sections/status-card"
import { MetadataCard } from "./sections/metadata-card"
import { ActionsCard } from "./sections/actions-card"
import { getResumeRoute } from "../create/getResumeRoute"
import { ImagesSection } from "./sections/images-section"

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: product, isLoading } = useProductDetail(id!)

  if (isLoading) return <LoadingSkeleton />

  if (!product)
    return (
      <div className="container mx-auto py-10 text-center">
        <p className="text-muted-foreground">Product not found.</p>
        <Button variant="link" onClick={() => navigate("/products")}>
          Back to Products
        </Button>
      </div>
    )

  return (
    <div className="container mx-auto max-w-6xl py-10">
      {/* Header */}
      <div className="mb-6 flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button
            variant="ghost"
            size="sm"
            className="gap-2 text-muted-foreground"
            onClick={() => navigate("/products")}
          >
            <ChevronLeft className="h-4 w-4" />
            Products
          </Button>
          <span className="text-muted-foreground">/</span>
          <h1 className="text-lg font-semibold">{product.name}</h1>
        </div>

        {/* Continue Setup — only for drafts */}
        {product.status === "Draft" && !product.isDeleted && (
          <Button
            variant="outline"
            onClick={() => navigate(getResumeRoute(product))}
          >
            Continue Setup
          </Button>
        )}
      </div>

      {/* Two-column layout */}
      <div className="grid grid-cols-3 gap-6">
        {/* Left — main content (2/3 width) */}
        <div className="col-span-2 space-y-6">
          <ImagesSection product={product} />
          <BasicInfoSection product={product} />
          <CategoriesSection product={product} />
        </div>

        {/* Right — sidebar (1/3 width) */}
        <div className="space-y-4">
          <StatusCard product={product} />
          <MetadataCard product={product} />
          <ActionsCard productId={product.id} />
        </div>
      </div>
    </div>
  )
}

function LoadingSkeleton() {
  return (
    <div className="container mx-auto max-w-6xl py-10">
      <Skeleton className="mb-6 h-8 w-48" />
      <div className="grid grid-cols-3 gap-6">
        <div className="col-span-2 space-y-6">
          <Skeleton className="h-64 w-full rounded-lg" />
          <Skeleton className="h-48 w-full rounded-lg" />
          <Skeleton className="h-32 w-full rounded-lg" />
        </div>
        <div className="space-y-4">
          <Skeleton className="h-40 w-full rounded-lg" />
          <Skeleton className="h-48 w-full rounded-lg" />
          <Skeleton className="h-24 w-full rounded-lg" />
        </div>
      </div>
    </div>
  )
}
