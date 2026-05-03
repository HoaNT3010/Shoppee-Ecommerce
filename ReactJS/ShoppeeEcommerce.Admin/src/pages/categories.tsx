import { useState } from "react"
import { Button } from "@/components/ui/button"
import { CreateEditModal } from "@/components/category/create-edit-modal"
import { DetailSheet } from "@/components/category/detail-sheet"
import { ConfirmDialog } from "@/components/category/confirm-dialog"
import { CategoryDataTable } from "./categories/data-table"
import CategoryService from "@/services/category-service"

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
  const isSoftDelete = dialog.type === "soft-delete"
  const isRestore = isSoftDelete && dialog.isDeleted

  return (
    <div className="container mx-auto mt-4">
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
        open={dialog.type === "hard-delete"}
        title="Permanently Delete Category"
        description="This cannot be undone. The category will be permanently removed."
        confirmLabel="Delete Forever"
        variant="destructive"
        invalidateKey={["categories"]}
        onConfirm={() => {
          if (dialog.type !== "hard-delete") return Promise.resolve()
          return CategoryService.hardDelete(dialog.id)
        }}
        onClose={close}
        successMsg="Category permanently deleted."
      />

      <ConfirmDialog
        open={isSoftDelete}
        title={isRestore ? "Restore Category" : "Delete Category"}
        description={
          isRestore
            ? "This will restore the category and make it active again."
            : "This will soft-delete the category. It can be restored later."
        }
        confirmLabel={isRestore ? "Restore" : "Delete"}
        variant={isRestore ? "default" : "destructive"}
        invalidateKey={["categories"]}
        onConfirm={() => {
          if (dialog.type !== "soft-delete") return Promise.resolve()
          return isRestore
            ? CategoryService.restore(dialog.id)
            : CategoryService.softDelete(dialog.id)
        }}
        onClose={close}
        successMsg={isRestore ? "Category restored." : "Category deleted."}
      />
    </div>
  )
}
