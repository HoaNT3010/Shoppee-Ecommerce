import type { ColumnDef } from "@tanstack/react-table"
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
import { type ListProductsResponse } from "@/types/product"
import { useNavigate } from "react-router"

export type ProductTableHandlers = {
  onView: (id: string) => void
  onSoftDelete: (id: string, isDeleted: boolean) => void
}

// Status badge — Draft is muted, Published is green
function StatusBadge({ status }: { status: string }) {
  if (status === "Published")
    return (
      <Badge className="bg-emerald-500 hover:bg-emerald-500/90">
        Published
      </Badge>
    )
  return <Badge variant="secondary">Draft</Badge>
}

// Image thumbnail with fallback
function ProductImage({ imgUrl, name }: { imgUrl?: string; name: string }) {
  console.log(imgUrl)
  if (imgUrl)
    return (
      <img
        src={imgUrl}
        alt={name}
        className="h-9 w-9 rounded-md border object-cover"
      />
    )
  return (
    <div className="flex h-9 w-9 items-center justify-center rounded-md border bg-muted text-[10px] font-medium text-muted-foreground">
      IMG
    </div>
  )
}

export const createColumns = (
  handlers: ProductTableHandlers
): ColumnDef<ListProductsResponse>[] => [
  // Image + Name combined
  {
    accessorKey: "name",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Product
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => (
      <div className="flex items-center gap-3">
        <ProductImage imgUrl={row.original.imgUrl} name={row.original.name} />
        <div className="min-w-0">
          <p className="truncate text-sm font-medium">{row.original.name}</p>
          <p className="truncate text-xs text-muted-foreground">
            {row.original.sku}
          </p>
        </div>
      </div>
    ),
  },

  // Price (sortable)
  {
    accessorKey: "price",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Price
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => {
      const price = row.getValue("price") as number
      return (
        <span className="font-medium tabular-nums">
          {new Intl.NumberFormat("en-US", {
            style: "currency",
            currency: "USD",
          }).format(price)}
        </span>
      )
    },
  },

  // Status badge
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => <StatusBadge status={row.getValue("status")} />,
  },

  // isDeleted badge
  {
    accessorKey: "isDeleted",
    header: "Visibility",
    cell: ({ row }) => {
      const isDeleted = row.getValue("isDeleted") as boolean
      return (
        <Badge variant={isDeleted ? "destructive" : "outline"}>
          {isDeleted ? "Deleted" : "Active"}
        </Badge>
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
    cell: ({ row }) =>
      new Date(row.getValue("createdDate")).toLocaleDateString(),
  },

  // Row actions — View + Delete/Restore only
  {
    id: "actions",
    cell: ({ row }) => {
      const product = row.original
      const navigate = useNavigate()

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
              <DropdownMenuItem
                onClick={() => navigate(`/products/${product.id}`)}
              >
                View details
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                className={product.isDeleted ? "" : "text-destructive"}
                onClick={() =>
                  handlers.onSoftDelete(product.id, product.isDeleted)
                }
              >
                {product.isDeleted ? "Restore" : "Delete"}
              </DropdownMenuItem>
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      )
    },
  },
]
