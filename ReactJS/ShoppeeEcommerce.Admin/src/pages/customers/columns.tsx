import type { ColumnDef } from "@tanstack/react-table"
import type { BaseCustomerResponse } from "@/types/user"

export const columns = (): ColumnDef<BaseCustomerResponse>[] => [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "userName",
    header: "User Name",
  },
  {
    accessorKey: "email",
    header: "Email",
  },
  {
    accessorKey: "firstName",
    header: "First Name",
    cell: ({ row }) => {
      const value = row.getValue("firstName")
      return value ? value : "N/A"
    },
  },
  {
    accessorKey: "lastName",
    header: "Last Name",
    cell: ({ row }) => {
      const value = row.getValue("lastName")
      return value ? value : "N/A"
    },
  },
]
