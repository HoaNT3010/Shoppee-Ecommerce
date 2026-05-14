import { ChevronLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { useOrderDetail } from "./useOrderDetail"
import { OrderItemsSection } from "./OrderItemsSection"
import { OrderSummaryCard } from "./OrderSummaryCard"
import { PaymentCard } from "./PaymentCard"
import { useNavigate, useParams } from "react-router"

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: order, isLoading } = useOrderDetail(id!)

  if (isLoading) return <LoadingSkeleton />

  if (!order)
    return (
      <div className="container mx-auto py-10 text-center">
        <p className="text-muted-foreground">Order not found.</p>
        <Button variant="link" onClick={() => navigate("/orders")}>
          Back to Orders
        </Button>
      </div>
    )

  return (
    <div className="container mx-auto max-w-6xl py-10">
      {/* Header */}
      <div className="mb-6 flex items-center gap-3">
        <Button
          variant="ghost"
          size="sm"
          className="gap-2 text-muted-foreground"
          onClick={() => navigate("/orders")}
        >
          <ChevronLeft className="h-4 w-4" />
          Orders
        </Button>
        <span className="text-muted-foreground">/</span>
        <h1 className="font-mono text-lg font-semibold">
          #{order.id.slice(0, 8).toUpperCase()}
        </h1>
      </div>

      {/* Two-column layout */}
      <div className="grid grid-cols-3 gap-6">
        {/* Left — items (2/3 width) */}
        <div className="col-span-2">
          <OrderItemsSection
            items={order.items}
            totalPrice={order.totalPrice}
          />
        </div>

        {/* Right — sidebar (1/3 width) */}
        <div className="space-y-4">
          <OrderSummaryCard order={order} />
          <PaymentCard payment={order.payment} />
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
        <div className="col-span-2">
          <Skeleton className="h-96 w-full rounded-lg" />
        </div>
        <div className="space-y-4">
          <Skeleton className="h-56 w-full rounded-lg" />
          <Skeleton className="h-64 w-full rounded-lg" />
        </div>
      </div>
    </div>
  )
}
