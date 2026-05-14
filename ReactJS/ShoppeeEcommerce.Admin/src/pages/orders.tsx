import { useCallback } from "react"
import { OrderDataTable } from "./orders/list/data-table"
import { useNavigate } from "react-router"

export default function OrdersPage() {
  const navigate = useNavigate()
  const handleView = useCallback((id: string) => navigate(`/orders/${id}`), [])
  return (
    <div className="container mx-auto mt-4">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Orders</h1>
      </div>

      <OrderDataTable onView={handleView} />
    </div>
  )
}
