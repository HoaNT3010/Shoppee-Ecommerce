import {
  flexRender,
  functionalUpdate,
  getCoreRowModel,
  type PaginationState,
} from "@tanstack/react-table"
import { columns } from "./columns"
import { Input } from "@/components/ui/input"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import { useReactTable } from "@tanstack/react-table"
import { useMemo } from "react"
import { useCustomerTableState } from "./useCustomerTableState"
import { useCustomersList } from "./useCustomersList"

export function CustomerDataTable() {
  const { params, dispatch } = useCustomerTableState()
  const { data, isFetching } = useCustomersList(params)

  const tableColumns = useMemo(() => columns(), [])

  const table = useReactTable({
    data: data?.items ?? [],
    columns: tableColumns,
    pageCount: data?.totalPages ?? -1, // tell TanStack the total pages

    // Manual mode — table doesn't process data itself
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,

    state: {
      pagination: {
        pageIndex: params.pageIndex! - 1,
        pageSize: params.pageSize!,
      },
    },

    onPaginationChange: (updater) => {
      const prev: PaginationState = {
        pageIndex: params.pageIndex! - 1,
        pageSize: params.pageSize!,
      }
      const next = functionalUpdate(updater, prev)
      dispatch({
        type: "SET_PAGINATION",
        payload: {
          pageIndex: next.pageIndex + 1,
          pageSize: next.pageSize,
        },
      })
    },

    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex items-center gap-2">
        <Input
          placeholder="Search by username, email, name"
          onChange={(e) =>
            dispatch({ type: "SET_SEARCH", payload: e.target.value })
          }
          className="max-w-sm"
        />
      </div>

      <div
        className={`rounded-md border transition-opacity ${isFetching ? "opacity-50" : ""}`}
      >
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead key={header.id}>
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow key={row.id}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={table.getAllColumns().length}
                  className="h-24 text-center"
                >
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-end gap-2">
        <span className="text-sm text-muted-foreground">
          Page {params.pageIndex} of {data?.totalPages ?? "—"}
        </span>
        <Button
          onClick={() =>
            dispatch({
              type: "SET_PAGINATION",
              payload: {
                pageIndex: params.pageIndex! - 1,
                pageSize: params.pageSize!,
              },
            })
          }
          disabled={params.pageIndex! <= 1}
        >
          Previous
        </Button>
        <Button
          onClick={() => {
            dispatch({
              type: "SET_PAGINATION",
              payload: {
                pageIndex: params.pageIndex! + 1,
                pageSize: params.pageSize!,
              },
            })
          }}
          disabled={params.pageIndex! >= (data?.totalPages ?? 1)}
        >
          Next
        </Button>
      </div>
    </div>
  )
}
