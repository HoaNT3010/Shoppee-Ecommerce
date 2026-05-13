import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet"
import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import type { ListOrderResponse } from "@/types/order"
import { formatDateTime } from "@/lib/utils"

interface OrderDetailSheetProps {
  order: ListOrderResponse | null
  open: boolean
  onClose: () => void
}

const STATUS_STYLES: Record<string, string> = {
  Pending: "bg-yellow-500 hover:bg-yellow-500/90",
  AwaitingPayment: "bg-orange-500 hover:bg-orange-500/90",
  Paid: "bg-blue-500 hover:bg-blue-500/90",
  Completed: "bg-emerald-500 hover:bg-emerald-500/90",
  Cancelled: "bg-destructive hover:bg-destructive/90",
  Refunded: "bg-purple-500 hover:bg-purple-500/90",
}

export function OrderDetailSheet({
  order,
  open,
  onClose,
}: OrderDetailSheetProps) {
  return (
    <Sheet open={open} onOpenChange={(v) => !v && onClose()}>
      <SheetContent className="flex flex-col gap-0 overflow-hidden p-0 sm:max-w-md">
        {/* Header */}
        <SheetHeader className="border-b px-6 py-5">
          <div className="flex items-center justify-between gap-4">
            <div className="space-y-1">
              <p className="text-xs font-medium tracking-widest text-muted-foreground uppercase">
                Order
              </p>
              <SheetTitle className="font-mono text-lg">
                #{order?.id.slice(0, 8).toUpperCase()}
              </SheetTitle>
            </div>
            {order && (
              <Badge className={STATUS_STYLES[order.status] ?? ""}>
                {order.status}
              </Badge>
            )}
          </div>
        </SheetHeader>

        {/* Body */}
        {order && (
          <div className="flex-1 space-y-5 overflow-y-auto px-6 py-6">
            {/* Summary */}
            <Section title="Summary">
              <Field
                label="Total"
                value={new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                }).format(order.totalPrice)}
              />
              <Field
                label="Items"
                value={`${order.lineItemsCount} ${order.lineItemsCount === 1 ? "item" : "items"}`}
              />
            </Section>

            <Separator />

            {/* Dates */}
            <Section title="Timeline">
              <Field
                label="Created"
                value={formatDateTime(order.createdDate)}
              />
              <Field
                label="Last Updated"
                value={
                  order.updatedDate ? formatDateTime(order.updatedDate) : "—"
                }
              />
            </Section>

            <Separator />

            {/* Full ID */}
            <Section title="Reference">
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Order ID</p>
                <p className="font-mono text-xs break-all text-foreground">
                  {order.id}
                </p>
              </div>
            </Section>
          </div>
        )}
      </SheetContent>
    </Sheet>
  )
}

function Section({
  title,
  children,
}: {
  title: string
  children: React.ReactNode
}) {
  return (
    <div className="space-y-3">
      <p className="text-[11px] font-semibold tracking-widest text-muted-foreground uppercase">
        {title}
      </p>
      <div className="space-y-3">{children}</div>
    </div>
  )
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4">
      <p className="text-sm text-muted-foreground">{label}</p>
      <p className="text-sm font-medium">{value}</p>
    </div>
  )
}
