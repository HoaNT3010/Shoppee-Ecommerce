import { CustomerDataTable } from "./customers/data-table"

export default function CustomersPage() {
  return (
    <div className="container mx-auto mt-4">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Customer Users</h1>
      </div>
      <CustomerDataTable />
    </div>
  )
}
