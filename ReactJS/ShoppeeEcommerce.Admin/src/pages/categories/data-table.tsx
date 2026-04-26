"use client"
import {
  flexRender,
  functionalUpdate,
  getCoreRowModel,
  type PaginationState,
  type SortingState,
} from "@tanstack/react-table"
import { useCategoryTableState } from "./useCategoryTableState"
import { useCategoryList } from "./useCategoryList"
import { columns, type CategoryTableHandlers } from "./columns"
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
import { Checkbox } from "@/components/ui/checkbox"
import { useReactTable } from "@tanstack/react-table"
import { useMemo } from "react"

interface CategoryDataTableProps extends CategoryTableHandlers {}

export function CategoryDataTable({
  onView,
  onEdit,
  onSoftDelete,
  onHardDelete,
}: CategoryDataTableProps) {
  const { params, dispatch } = useCategoryTableState()
  const { data, isFetching } = useCategoryList(params)

  // TanStack Table needs SortingState shape — derive it from params
  const sorting: SortingState = params.sortBy
    ? [{ id: params.sortBy, desc: params.sortDesc === true }]
    : []

  const tableColumns = useMemo(
    () => columns({ onView, onEdit, onSoftDelete, onHardDelete }),
    [onView, onEdit, onSoftDelete, onHardDelete]
  )

  const table = useReactTable({
    data: data?.items ?? [],
    columns: tableColumns,
    pageCount: data?.totalPages ?? -1, // tell TanStack the total pages

    // Manual mode — table doesn't process data itself
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
          placeholder="Search by name and description..."
          onChange={(e) =>
            dispatch({ type: "SET_SEARCH", payload: e.target.value })
          }
          className="max-w-sm"
        />
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

      {/* Table renders exactly as before */}
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
