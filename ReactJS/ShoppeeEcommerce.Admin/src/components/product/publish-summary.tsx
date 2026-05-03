import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import type { DetailedProductResponse } from "@/types/product"

interface PublishSummaryProps {
  product: DetailedProductResponse
}

export function PublishSummary({ product }: PublishSummaryProps) {
  const mainImage = product.images?.find((img) => img.isMain)

  return (
    <div className="overflow-hidden rounded-lg border bg-muted/30">
      {/* Main image banner */}
      {mainImage ? (
        <div className="h-40 w-full overflow-hidden">
          <img
            src={mainImage.url}
            alt={product.name}
            className="h-full w-full object-cover"
          />
        </div>
      ) : (
        <div className="flex h-40 w-full items-center justify-center bg-muted">
          <p className="text-sm text-muted-foreground">No main image set</p>
        </div>
      )}

      <div className="space-y-4 p-4">
        {/* Name + status */}
        <div className="flex items-start justify-between gap-3">
          <div>
            <p className="font-semibold">{product.name}</p>
            <p className="mt-0.5 text-xs text-muted-foreground">
              {product.sku}
            </p>
          </div>
          <Badge variant="secondary">Draft</Badge>
        </div>

        <Separator />

        {/* Key details grid */}
        <div className="grid grid-cols-2 gap-3 text-sm">
          <SummaryField
            label="Price"
            value={new Intl.NumberFormat("en-US", {
              style: "currency",
              currency: "USD",
            }).format(product.price)}
          />
          <SummaryField
            label="Images"
            value={`${product.images?.length ?? 0} uploaded`}
          />
          <SummaryField
            label="Categories"
            value={`${product.categories?.length ?? 0} selected`}
          />
          <SummaryField
            label="Created"
            value={new Date(product.createdDate).toLocaleDateString()}
          />
        </div>

        {/* Categories list */}
        {product.categories?.length > 0 && (
          <div className="flex flex-wrap gap-1.5">
            {product.categories.map((cat) => (
              <Badge key={cat.id} variant="outline" className="text-xs">
                {cat.name}
              </Badge>
            ))}
          </div>
        )}

        {/* Description */}
        <div className="space-y-1">
          <p className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
            Description
          </p>
          <p className="line-clamp-3 text-sm leading-relaxed">
            {product.description}
          </p>
        </div>
      </div>
    </div>
  )
}

function SummaryField({ label, value }: { label: string; value: string }) {
  return (
    <div className="space-y-0.5">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="text-sm font-medium">{value}</p>
    </div>
  )
}
