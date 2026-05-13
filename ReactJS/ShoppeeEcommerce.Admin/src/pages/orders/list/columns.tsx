import { type ColumnDef } from "@tanstack/react-table"
import { ArrowUpDown, MoreHorizontal } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import type { ListOrderResponse } from "@/types/order"
import { formatDateTime } from "@/lib/utils"

export type OrderTableHandlers = {
  onView: (order: ListOrderResponse) => void
}

// Status badge — each status gets a distinct color
function StatusBadge({ status }: { status: string }) {
  const styles: Record<string, string> = {
    Pending: "bg-yellow-500 hover:bg-yellow-500/90",
    AwaitingPayment: "bg-orange-500 hover:bg-orange-500/90",
    Paid: "bg-blue-500 hover:bg-blue-500/90",
    Completed: "bg-emerald-500 hover:bg-emerald-500/90",
    Cancelled: "bg-destructive hover:bg-destructive/90",
    Refunded: "bg-purple-500 hover:bg-purple-500/90",
  }

  return <Badge className={styles[status] ?? ""}>{status}</Badge>
}

export const createColumns = (
  handlers: OrderTableHandlers
): ColumnDef<ListOrderResponse>[] => [
  // Order ID — truncated since GUIDs are long
  {
    accessorKey: "id",
    header: "Order ID",
    cell: ({ row }) => (
      <span className="font-mono text-xs text-muted-foreground">
        #{row.original.id.slice(0, 8).toUpperCase()}
      </span>
    ),
  },

  // Status
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => <StatusBadge status={row.getValue("status")} />,
  },

  // Total price (sortable)
  {
    accessorKey: "totalPrice",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Total
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => (
      <span className="font-medium tabular-nums">
        {new Intl.NumberFormat("en-US", {
          style: "currency",
          currency: "USD",
        }).format(row.getValue("totalPrice"))}
      </span>
    ),
  },

  // Line items count
  {
    accessorKey: "lineItemsCount",
    header: "Items",
    cell: ({ row }) => {
      const count = row.getValue("lineItemsCount") as number
      return (
        <span className="text-sm text-muted-foreground">
          {count} {count === 1 ? "item" : "items"}
        </span>
      )
    },
  },

  // Created date (sortable)
  {
    accessorKey: "createdDate",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Created
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => formatDateTime(row.original.createdDate),
  },

  // Updated date
  {
    accessorKey: "updatedDate",
    header: "Last Updated",
    cell: ({ row }) => {
      const date = row.original.updatedDate
      return date ? (
        <span className="text-sm text-muted-foreground">
          {formatDateTime(date)}
        </span>
      ) : (
        <span className="text-sm text-muted-foreground">—</span>
      )
    },
  },

  // Actions
  {
    id: "actions",
    cell: ({ row }) => {
      const order = row.original
      return (
        <DropdownMenu>
          <DropdownMenuTrigger className="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-accent">
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuGroup>
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => handlers.onView(order)}>
                View details
              </DropdownMenuItem>
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      )
    },
  },
]
