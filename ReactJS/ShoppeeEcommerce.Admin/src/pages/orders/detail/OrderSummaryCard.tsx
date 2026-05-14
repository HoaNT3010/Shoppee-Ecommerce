import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { formatDateTime } from "@/lib/utils"
import type { AdminViewOrderDetailResponse } from "@/types/order"

const STATUS_STYLES: Record<string, string> = {
  Pending: "bg-yellow-500 hover:bg-yellow-500/90",
  AwaitingPayment: "bg-orange-500 hover:bg-orange-500/90",
  Paid: "bg-blue-500 hover:bg-blue-500/90",
  Completed: "bg-emerald-500 hover:bg-emerald-500/90",
  Cancelled: "bg-destructive hover:bg-destructive/90",
  Refunded: "bg-purple-500 hover:bg-purple-500/90",
}

interface OrderSummaryCardProps {
  order: AdminViewOrderDetailResponse
}

export function OrderSummaryCard({ order }: OrderSummaryCardProps) {
  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between gap-2">
          <CardTitle className="text-base">Summary</CardTitle>
          <Badge className={STATUS_STYLES[order.status] ?? ""}>
            {order.status}
          </Badge>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        <Field
          label="Total"
          value={new Intl.NumberFormat("en-US", {
            style: "currency",
            currency: "USD",
          }).format(order.totalPrice)}
        />

        <Separator />

        <Field label="Created" value={formatDateTime(order.createdDate)} />
        <Field
          label="Last Updated"
          value={order.updatedDate ? formatDateTime(order.updatedDate) : "—"}
        />

        <Separator />

        <div className="space-y-0.5">
          <p className="text-xs text-muted-foreground">Order ID</p>
          <p className="font-mono text-xs break-all">{order.id}</p>
        </div>
        <div className="space-y-0.5">
          <p className="text-xs text-muted-foreground">User ID</p>
          <p className="font-mono text-xs break-all">{order.userId}</p>
        </div>
      </CardContent>
    </Card>
  )
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="text-sm font-medium tabular-nums">{value}</p>
    </div>
  )
}
