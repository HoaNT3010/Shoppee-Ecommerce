"use client"

import { useQuery } from "@tanstack/react-query"
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetFooter,
} from "@/components/ui/sheet"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import CategoryService from "@/services/category-service"
import type { DetailedCategoryResponse } from "@/types/category"
import { CalendarDays, Clock, Pencil, Trash2, User } from "lucide-react"

interface DetailSheetProps {
  open: boolean
  id?: string
  onEdit: (id: string) => void
  onClose: () => void
}

export function DetailSheet({ open, id, onEdit, onClose }: DetailSheetProps) {
  const { data, isLoading } = useQuery({
    queryKey: ["category", id],
    queryFn: () => CategoryService.getById(id!),
    enabled: open && !!id,
  })

  return (
    <Sheet open={open} onOpenChange={(v) => !v && onClose()}>
      <SheetContent className="flex flex-col gap-0 overflow-hidden p-0 sm:max-w-md">
        {/* ── Header ── */}
        <SheetHeader className="border-b px-6 py-5">
          <div className="flex items-start justify-between gap-4">
            <div className="space-y-1">
              <p className="text-xs font-medium tracking-widest text-muted-foreground uppercase">
                Category
              </p>
              {isLoading ? (
                <Skeleton className="h-6 w-40" />
              ) : (
                <SheetTitle className="text-xl font-semibold tracking-tight">
                  {data?.name ?? "—"}
                </SheetTitle>
              )}
            </div>
            {!isLoading && data && (
              <Badge
                variant={data.isDeleted ? "destructive" : "default"}
                className="mt-1 shrink-0 rounded-full px-3 py-0.5 text-xs font-medium"
              >
                {data.isDeleted ? "Deleted" : "Active"}
              </Badge>
            )}
          </div>
        </SheetHeader>

        {/* ── Body ── */}
        <div className="flex-1 overflow-y-auto px-6 py-6">
          {isLoading ? (
            <LoadingSkeleton />
          ) : data ? (
            <div className="space-y-5">
              {/* Description */}
              <Card>
                <CardLabel>Description</CardLabel>
                <p className="text-sm leading-relaxed text-foreground">
                  {data.description || (
                    <span className="text-muted-foreground italic">
                      No description provided.
                    </span>
                  )}
                </p>
              </Card>

              {/* Dates */}
              <Card>
                <CardLabel>Timeline</CardLabel>
                <div className="space-y-3">
                  <DateRow
                    icon={<CalendarDays className="h-3.5 w-3.5" />}
                    label="Created"
                    value={formatDate(data.createdDate)}
                  />
                  <DateRow
                    icon={<Clock className="h-3.5 w-3.5" />}
                    label="Last updated"
                    value={
                      data.updatedDate ? formatDate(data.updatedDate) : "—"
                    }
                  />
                  {data.isDeleted && (
                    <DateRow
                      icon={<Trash2 className="h-3.5 w-3.5 text-destructive" />}
                      label="Deleted on"
                      value={
                        data.deletedDate ? formatDate(data.deletedDate) : "—"
                      }
                      destructive
                    />
                  )}
                </div>
              </Card>

              {/* Creator */}
              {data.creator && (
                <Card>
                  <CardLabel>Created by</CardLabel>
                  <div className="flex items-center gap-3">
                    {/* Avatar */}
                    <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-muted text-muted-foreground">
                      <User className="h-4 w-4" />
                    </div>
                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium">
                        {formatCreatorName(data.creator)}
                      </p>
                      <p className="truncate text-xs text-muted-foreground">
                        {data.creator.email}
                      </p>
                    </div>
                    <span className="ml-auto shrink-0 rounded-full bg-muted px-2.5 py-0.5 text-xs text-muted-foreground">
                      @{data.creator.userName}
                    </span>
                  </div>
                </Card>
              )}
            </div>
          ) : null}
        </div>

        {/* ── Footer ── */}
        <SheetFooter className="border-t bg-muted/30 px-6 py-4">
          <div className="flex w-full items-center justify-between gap-3">
            <Button variant="outline" className="flex-1" onClick={onClose}>
              Close
            </Button>
            <Button
              className="flex-1 gap-2"
              onClick={() => {
                onClose()
                onEdit(id!)
              }}
              disabled={!data || data.isDeleted}
            >
              <Pencil className="h-3.5 w-3.5" />
              Edit
            </Button>
          </div>
        </SheetFooter>
      </SheetContent>
    </Sheet>
  )
}

// ── Sub-components ────────────────────────────────────────────

function Card({ children }: { children: React.ReactNode }) {
  return (
    <div className="space-y-3 rounded-lg border bg-card p-4 shadow-sm">
      {children}
    </div>
  )
}

function CardLabel({ children }: { children: React.ReactNode }) {
  return (
    <p className="text-[11px] font-semibold tracking-widest text-muted-foreground uppercase">
      {children}
    </p>
  )
}

function DateRow({
  icon,
  label,
  value,
  destructive = false,
}: {
  icon: React.ReactNode
  label: string
  value: string
  destructive?: boolean
}) {
  return (
    <div className="flex items-center justify-between gap-4">
      <div
        className={`flex items-center gap-2 text-xs ${destructive ? "text-destructive" : "text-muted-foreground"}`}
      >
        {icon}
        <span>{label}</span>
      </div>
      <span
        className={`text-xs font-medium tabular-nums ${destructive ? "text-destructive" : ""}`}
      >
        {value}
      </span>
    </div>
  )
}

function LoadingSkeleton() {
  return (
    <div className="space-y-5">
      {[80, 120, 60].map((w, i) => (
        <div key={i} className="space-y-3 rounded-lg border p-4">
          <Skeleton className="h-3 w-16" />
          <Skeleton className={`h-4 w-${w}`} />
          {i === 1 && <Skeleton className="h-4 w-24" />}
        </div>
      ))}
    </div>
  )
}

// ── Helpers ───────────────────────────────────────────────────

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleString(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  })
}

function formatCreatorName(creator: DetailedCategoryResponse["creator"]) {
  if (!creator) return "—"
  const full = [creator.firstName, creator.lastName].filter(Boolean).join(" ")
  return full || creator.userName
}
