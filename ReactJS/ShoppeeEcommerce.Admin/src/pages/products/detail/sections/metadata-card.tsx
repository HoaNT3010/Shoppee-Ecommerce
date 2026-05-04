import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { User } from "lucide-react"
import type { DetailedProductResponse } from "@/types/product"

interface MetadataCardProps {
  product: DetailedProductResponse
}

export function MetadataCard({ product }: MetadataCardProps) {
  return (
    <Card>
      <CardHeader className="pb-3">
        <CardTitle className="text-base">Metadata</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <MetaField
          label="Created"
          value={new Date(product.createdDate).toLocaleString()}
        />
        <MetaField
          label="Last Updated"
          value={
            product.updatedDate
              ? new Date(product.updatedDate).toLocaleString()
              : "—"
          }
        />
        {product.isDeleted && product.deletedDate && (
          <MetaField
            label="Deleted On"
            value={new Date(product.deletedDate).toLocaleString()}
          />
        )}

        {product.creator && (
          <div className="space-y-1 border-t pt-1">
            <p className="pt-2 text-xs font-medium tracking-wide text-muted-foreground uppercase">
              Created By
            </p>
            <div className="flex items-center gap-2.5 pt-1">
              <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-muted">
                <User className="h-4 w-4 text-muted-foreground" />
              </div>
              <div className="min-w-0">
                <p className="truncate text-sm font-medium">
                  {formatName(product.creator)}
                </p>
                <p className="truncate text-xs text-muted-foreground">
                  {product.creator.email}
                </p>
              </div>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  )
}

function MetaField({ label, value }: { label: string; value: string }) {
  return (
    <div className="space-y-0.5">
      <p className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
        {label}
      </p>
      <p className="text-sm">{value}</p>
    </div>
  )
}

function formatName(creator: DetailedProductResponse["creator"]) {
  if (!creator) return "—"
  const full = [creator.firstName, creator.lastName].filter(Boolean).join(" ")
  return full || creator.userName
}
