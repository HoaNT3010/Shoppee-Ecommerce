import * as React from "react"
import {
  flexRender,
  functionalUpdate,
  getCoreRowModel,
  useReactTable,
  type PaginationState,
  type SortingState,
} from "@tanstack/react-table"
import { useOrderTableState } from "./useOrderTableState"
import { useOrderList } from "./useOrderList"
import { createColumns, type OrderTableHandlers } from "./columns"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"

const ORDER_STATUSES = [
  "Pending",
  "AwaitingPayment",
  "Paid",
  "Completed",
  "Cancelled",
  "Refunded",
] as const

export function OrderDataTable({ onView }: OrderTableHandlers) {
  const { params, dispatch } = useOrderTableState()
  const { data, isFetching } = useOrderList(params)

  const sorting: SortingState = params.sortBy
    ? [{ id: params.sortBy, desc: params.sortDesc === true }]
    : []

  const columns = React.useMemo(() => createColumns({ onView }), [onView])

  const table = useReactTable({
    data: data?.items ?? [],
    columns,
    pageCount: data?.totalPages ?? -1,
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
    state: {
      sorting,
      pagination: {
        pageIndex: (params.pageIndex ?? 1) - 1,
        pageSize: params.pageSize ?? 10,
      },
    },
    onSortingChange: (updater) => {
      const next = functionalUpdate(updater, sorting)
      dispatch({ type: "SET_SORTING", payload: next })
    },
    onPaginationChange: (updater) => {
      const prev: PaginationState = {
        pageIndex: (params.pageIndex ?? 1) - 1,
        pageSize: params.pageSize ?? 10,
      }
      const next = functionalUpdate(updater, prev)
      dispatch({
        type: "SET_PAGINATION",
        payload: { pageIndex: next.pageIndex + 1, pageSize: next.pageSize },
      })
    },
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex flex-wrap items-end gap-3">
        {/* Status filter */}
        <div className="space-y-1">
          <p className="text-xs text-muted-foreground">Status</p>
          <Select
            onValueChange={(v) =>
              dispatch({
                type: "SET_STATUS",
                payload:
                  v === "All" ? null : (v as (typeof ORDER_STATUSES)[number]),
              })
            }
            defaultValue="All"
          >
            <SelectTrigger className="w-44">
              <SelectValue placeholder="All statuses" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All statuses</SelectItem>
              {ORDER_STATUSES.map((status) => (
                <SelectItem key={status} value={status}>
                  {status}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {/* Price range */}
        <div className="space-y-1">
          <p className="text-xs text-muted-foreground">Price range</p>
          <div className="flex items-center gap-2">
            <Input
              type="number"
              placeholder="Min"
              className="w-24"
              onChange={(e) =>
                dispatch({
                  type: "SET_PRICE_RANGE",
                  payload: {
                    minPrice: e.target.value
                      ? Number(e.target.value)
                      : undefined,
                    maxPrice: params.maxPrice,
                  },
                })
              }
            />
            <span className="text-sm text-muted-foreground">—</span>
            <Input
              type="number"
              placeholder="Max"
              className="w-24"
              onChange={(e) =>
                dispatch({
                  type: "SET_PRICE_RANGE",
                  payload: {
                    minPrice: params.minPrice,
                    maxPrice: e.target.value
                      ? Number(e.target.value)
                      : undefined,
                  },
                })
              }
            />
          </div>
        </div>

        {/* Date range */}
        <div className="space-y-1">
          <p className="text-xs text-muted-foreground">Date range</p>
          <div className="flex items-center gap-2">
            <Input
              type="date"
              className="w-36"
              onChange={(e) =>
                dispatch({
                  type: "SET_DATE_RANGE",
                  payload: {
                    fromCreatedDate: e.target.value || undefined,
                    toCreatedDate: params.toCreatedDate,
                  },
                })
              }
            />
            <span className="text-sm text-muted-foreground">—</span>
            <Input
              type="date"
              className="w-36"
              onChange={(e) =>
                dispatch({
                  type: "SET_DATE_RANGE",
                  payload: {
                    fromCreatedDate: params.fromCreatedDate,
                    toCreatedDate: e.target.value || undefined,
                  },
                })
              }
            />
          </div>
        </div>
      </div>

      {/* Table */}
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
                  No orders found.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-between">
        <p className="text-sm text-muted-foreground">
          {data?.totalCount ?? 0} total orders
        </p>
        <div className="flex items-center gap-2">
          <span className="text-sm text-muted-foreground">
            Page {params.pageIndex} of {data?.totalPages ?? "—"}
          </span>
          <Button
            variant="outline"
            size="sm"
            onClick={() =>
              dispatch({
                type: "SET_PAGINATION",
                payload: {
                  pageIndex: (params.pageIndex ?? 1) - 1,
                  pageSize: params.pageSize ?? 10,
                },
              })
            }
            disabled={(params.pageIndex ?? 1) <= 1}
          >
            Previous
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() =>
              dispatch({
                type: "SET_PAGINATION",
                payload: {
                  pageIndex: (params.pageIndex ?? 1) + 1,
                  pageSize: params.pageSize ?? 10,
                },
              })
            }
            disabled={(params.pageIndex ?? 1) >= (data?.totalPages ?? 1)}
          >
            Next
          </Button>
        </div>
      </div>
    </div>
  )
}
