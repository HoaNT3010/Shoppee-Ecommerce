import type { ColumnDef } from "@tanstack/react-table"
import type { ShortCategoryResponse } from "@/types/category"
import { Badge } from "@/components/ui/badge"
import { format } from "date-fns"
import { Button } from "@/components/ui/button"
import { ArrowUpDown, MoreHorizontal } from "lucide-react"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

export type CategoryTableHandlers = {
  onView: (id: string) => void
  onEdit: (id: string) => void
  onSoftDelete: (id: string, isDeleted: boolean) => void
  onHardDelete: (id: string) => void
}

export const columns = (
  handlers: CategoryTableHandlers
): ColumnDef<ShortCategoryResponse>[] => [
  {
    accessorKey: "id",
    header: "ID",
  },
  // --- Name (sortable) ---
  {
    accessorKey: "name",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Name
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
  },
  // --- Description ---
  {
    accessorKey: "description",
    header: "Description",
    cell: ({ row }) => (
      <span className="block max-w-75 truncate">
        {row.getValue("description")}
      </span>
    ),
  },
  // --- Created Date (sortable) ---
  {
    accessorKey: "createdDate",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Created Date
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => {
      return (
        <span>
          {format(new Date(row.getValue("createdDate")), "dd/MM/yyyy")}
        </span>
      )
    },
  },
  // --- Status badge ---
  {
    accessorKey: "isDeleted",
    header: ({ column }) => (
      <Button
        variant="ghost"
        onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
      >
        Status
        <ArrowUpDown className="ml-2 h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => {
      const isDeleted = row.getValue("isDeleted") as boolean
      return (
        <Badge variant={isDeleted ? "destructive" : "default"}>
          {isDeleted ? "Deleted" : "Active"}
        </Badge>
      )
    },
  },
  // --- Row actions ---
  {
    id: "actions",
    cell: ({ row }) => {
      const category = row.original
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
              <DropdownMenuItem onClick={() => handlers.onView(category.id)}>
                View details
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handlers.onEdit(category.id)}>
                Edit
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={() =>
                  handlers.onSoftDelete(category.id, category.isDeleted)
                }
              >
                {category.isDeleted ? "Restore" : "Delete"}
              </DropdownMenuItem>
              <DropdownMenuItem
                className="text-destructive"
                onClick={() => handlers.onHardDelete(category.id)}
              >
                Hard delete
              </DropdownMenuItem>
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      )
    },
  },
]
