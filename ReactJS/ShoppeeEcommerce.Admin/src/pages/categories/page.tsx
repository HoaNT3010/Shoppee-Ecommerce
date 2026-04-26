"use client"

import { useState } from "react"
import { CategoryDataTable } from "./data-table"
import { Button } from "@/components/ui/button"
import { CreateEditModal } from "@/components/category/create-edit-modal"
import { DetailSheet } from "@/components/category/detail-sheet"
import { ConfirmDialog } from "@/components/category/confirm-dialog"

type DialogState =
  | { type: "none" }
  | { type: "create" }
  | { type: "edit"; id: string }
  | { type: "detail"; id: string }
  | { type: "soft-delete"; id: string; isDeleted: boolean }
  | { type: "hard-delete"; id: string }

export default function CategoriesPage() {
  const [dialog, setDialog] = useState<DialogState>({ type: "none" })
  const close = () => setDialog({ type: "none" })

  return (
    <div className="container mx-auto py-10">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Categories</h1>
        <Button onClick={() => setDialog({ type: "create" })}>
          + New Category
        </Button>
      </div>

      <CategoryDataTable
        onView={(id) => setDialog({ type: "detail", id })}
        onEdit={(id) => setDialog({ type: "edit", id })}
        onSoftDelete={(id, isDeleted) =>
          setDialog({ type: "soft-delete", id, isDeleted })
        }
        onHardDelete={(id) => setDialog({ type: "hard-delete", id })}
      />

      <CreateEditModal
        open={dialog.type === "create" || dialog.type === "edit"}
        id={dialog.type === "edit" ? dialog.id : undefined}
        onClose={close}
      />

      <DetailSheet
        open={dialog.type === "detail"}
        id={dialog.type === "detail" ? dialog.id : undefined}
        onEdit={(id) => setDialog({ type: "edit", id })}
        onClose={close}
      />

      <ConfirmDialog
        open={dialog.type === "soft-delete"}
        title={
          dialog.type === "soft-delete" && dialog.isDeleted
            ? "Restore Category"
            : "Delete Category"
        }
        description={
          dialog.type === "soft-delete" && dialog.isDeleted
            ? "This will restore the category and make it active again."
            : "This will soft-delete the category. It can be restored later."
        }
        confirmLabel={
          dialog.type === "soft-delete" && dialog.isDeleted
            ? "Restore"
            : "Delete"
        }
        variant={
          dialog.type === "soft-delete" && dialog.isDeleted
            ? "default"
            : "destructive"
        }
        id={dialog.type === "soft-delete" ? dialog.id : undefined}
        action="soft-delete"
        onClose={close}
      />

      <ConfirmDialog
        open={dialog.type === "hard-delete"}
        title="Permanently Delete Category"
        description="This cannot be undone. The category will be permanently removed."
        confirmLabel="Delete Forever"
        variant="destructive"
        id={dialog.type === "hard-delete" ? dialog.id : undefined}
        action="hard-delete"
        onClose={close}
      />
    </div>
  )
}
