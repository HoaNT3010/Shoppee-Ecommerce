import type { ListOrderResponse } from "@/types/order"
import { useCallback, useState } from "react"
import { OrderDataTable } from "./orders/list/data-table"
import { OrderDetailSheet } from "./orders/list/OrderDetailSheet"

export default function OrdersPage() {
  const [selectedOrder, setSelectedOrder] = useState<ListOrderResponse | null>(
    null
  )
  const handleView = useCallback(
    (order: ListOrderResponse) => setSelectedOrder(order),
    []
  )
  return (
    <div className="container mx-auto mt-4">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Orders</h1>
      </div>

      <OrderDataTable onView={handleView} />

      <OrderDetailSheet
        order={selectedOrder}
        open={!!selectedOrder}
        onClose={() => setSelectedOrder(null)}
      />
    </div>
  )
}
