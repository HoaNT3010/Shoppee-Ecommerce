import { Button } from "@/components/ui/button"
import { createColumns, type ProductTableHandlers } from "./columns"
import { useProductTableState } from "./useProductTableState"
import { useProductList } from "./useProductList"
import {
  flexRender,
  functionalUpdate,
  getCoreRowModel,
  useReactTable,
  type PaginationState,
  type SortingState,
} from "@tanstack/react-table"
import React from "react"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

export function ProductDataTable({
  onView,
  onSoftDelete,
}: ProductTableHandlers) {
  const { params, dispatch } = useProductTableState()
  const { data, isFetching } = useProductList(params)

  const sorting: SortingState = params.sortBy
    ? [{ id: params.sortBy, desc: params.sortDesc === true }]
    : []

  const columns = React.useMemo(
    () => createColumns({ onView, onSoftDelete }),
    [onView, onSoftDelete]
  )

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
        pageIndex: params.pageIndex! - 1,
        pageSize: params.pageSize!,
      },
    },
    onSortingChange: (updater) => {
      const next = functionalUpdate(updater, sorting)
      dispatch({ type: "SET_SORTING", payload: next })
    },
    onPaginationChange: (updater) => {
      const prev: PaginationState = {
        pageIndex: params.pageIndex! - 1,
        pageSize: params.pageSize!,
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
      <div className="flex flex-wrap items-center gap-3">
        <Input
          placeholder="Search products..."
          onChange={(e) =>
            dispatch({ type: "SET_SEARCH", payload: e.target.value })
          }
          className="max-w-xs"
        />
        <Select
          onValueChange={(v) =>
            dispatch({
              type: "SET_STATUS",
              payload: v === "all" ? null : (v as "Draft" | "Published"),
            })
          }
          defaultValue="all"
        >
          <SelectTrigger className="w-36">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="Draft">Draft</SelectItem>
            <SelectItem value="Published">Published</SelectItem>
          </SelectContent>
        </Select>
        <div className="flex items-center gap-2">
          <Input
            type="number"
            placeholder="Min price"
            className="w-28"
            onChange={(e) =>
              dispatch({
                type: "SET_PRICE_RANGE",
                payload: {
                  minPrice: e.target.value ? Number(e.target.value) : undefined,
                  maxPrice: params.maxPrice,
                },
              })
            }
          />
          <span className="text-sm text-muted-foreground">—</span>
          <Input
            type="number"
            placeholder="Max price"
            className="w-28"
            onChange={(e) =>
              dispatch({
                type: "SET_PRICE_RANGE",
                payload: {
                  minPrice: params.minPrice,
                  maxPrice: e.target.value ? Number(e.target.value) : undefined,
                },
              })
            }
          />
        </div>
        <label className="ml-auto flex items-center gap-2 text-sm">
          <Checkbox
            checked={params.includeDeleted ?? false}
            onCheckedChange={(v) =>
              dispatch({ type: "SET_INCLUDE_DELETED", payload: !!v })
            }
          />
          Include deleted
        </label>
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
          variant="outline"
          size="sm"
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
          variant="outline"
          size="sm"
          onClick={() =>
            dispatch({
              type: "SET_PAGINATION",
              payload: {
                pageIndex: params.pageIndex! + 1,
                pageSize: params.pageSize!,
              },
            })
          }
          disabled={params.pageIndex! >= (data?.totalPages ?? 1)}
        >
          Next
        </Button>
      </div>
    </div>
  )
}
