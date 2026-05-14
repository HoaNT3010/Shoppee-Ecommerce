import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import type { AdminOrderItemDetailResponse } from "@/types/order"

interface OrderItemsSectionProps {
  items: AdminOrderItemDetailResponse[]
  totalPrice: number
}

export function OrderItemsSection({
  items,
  totalPrice,
}: OrderItemsSectionProps) {
  const fmt = (n: number) =>
    new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(n)

  return (
    <Card>
      <CardHeader className="pb-3">
        <CardTitle className="text-base">
          Order Items
          <span className="ml-2 text-sm font-normal text-muted-foreground">
            ({items.length} {items.length === 1 ? "item" : "items"})
          </span>
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <div className="divide-y">
          {items.map((item) => (
            <div key={item.id} className="flex items-center gap-4 px-6 py-4">
              {/* Product image */}
              {item.productImgUrl ? (
                <img
                  src={item.productImgUrl}
                  alt={item.productName}
                  className="h-14 w-14 shrink-0 rounded-md border object-cover"
                />
              ) : (
                <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-md border bg-muted text-xs text-muted-foreground">
                  IMG
                </div>
              )}

              {/* Product info */}
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium">
                  {item.productName}
                </p>
                <p className="text-xs text-muted-foreground">
                  {item.productSKU}
                </p>
              </div>

              {/* Price × qty */}
              <div className="shrink-0 text-right">
                <p className="text-sm font-medium tabular-nums">
                  {fmt(item.price * item.quantity)}
                </p>
                <p className="text-xs text-muted-foreground">
                  {fmt(item.price)} × {item.quantity}
                </p>
              </div>
            </div>
          ))}
        </div>

        {/* Total row */}
        <div className="flex items-center justify-between border-t bg-muted/30 px-6 py-4">
          <p className="text-sm font-semibold">Total</p>
          <p className="text-sm font-semibold tabular-nums">
            {fmt(totalPrice)}
          </p>
        </div>
      </CardContent>
    </Card>
  )
}
