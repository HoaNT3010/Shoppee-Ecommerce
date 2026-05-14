import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { formatDateTime } from "@/lib/utils"
import type { AdminOrderPaymentResponse } from "@/types/order"

interface PaymentCardProps {
  payment: AdminOrderPaymentResponse | null
}

export function PaymentCard({ payment }: PaymentCardProps) {
  const fmt = (n: number) =>
    new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(n)

  const fmtDate = (d?: string) => (d ? formatDateTime(d) : "—")

  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between gap-2">
          <CardTitle className="text-base">Payment</CardTitle>
          {payment && <Badge variant="outline">{payment.method}</Badge>}
        </div>
      </CardHeader>
      <CardContent>
        {/* No payment yet — pending orders */}
        {!payment ? (
          <div className="flex flex-col items-center justify-center gap-2 py-6">
            <p className="text-sm text-muted-foreground">
              No payment information yet.
            </p>
            <p className="text-xs text-muted-foreground">
              Payment details will appear once the order is processed.
            </p>
          </div>
        ) : (
          <div className="space-y-3">
            <Field label="Amount" value={fmt(payment.amount)} />
            <Field label="Status" value={payment.status} />
            <Field label="Method" value={payment.method} />

            <Separator />

            {/* Timeline — only show relevant dates */}
            <Field label="Initiated" value={fmtDate(payment.createdDate)} />
            {payment.paidDate && (
              <Field label="Paid On" value={fmtDate(payment.paidDate)} />
            )}
            {payment.refundedDate && (
              <Field
                label="Refunded On"
                value={fmtDate(payment.refundedDate)}
              />
            )}
            {payment.failedDate && (
              <Field label="Failed On" value={fmtDate(payment.failedDate)} />
            )}

            {/* Refund info */}
            {payment.refundedAmount != null && (
              <>
                <Separator />
                <Field
                  label="Refunded Amount"
                  value={fmt(payment.refundedAmount)}
                />
              </>
            )}

            {/* Failure reason */}
            {payment.failureReason && (
              <>
                <Separator />
                <div className="space-y-1">
                  <p className="text-xs text-muted-foreground">
                    Failure Reason
                  </p>
                  <p className="text-sm text-destructive">
                    {payment.failureReason}
                  </p>
                </div>
              </>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  )
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="text-sm font-medium">{value}</p>
    </div>
  )
}
