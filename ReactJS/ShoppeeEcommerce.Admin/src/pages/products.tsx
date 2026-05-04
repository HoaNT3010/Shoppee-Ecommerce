import { Button } from "@/components/ui/button"
import ProductService from "@/services/product-service"
import { useCallback, useState } from "react"
import { useNavigate } from "react-router"
import { ProductDataTable } from "./products/data-table"
import { ConfirmDialog } from "@/components/category/confirm-dialog"

type DialogState =
  | { type: "none" }
  | { type: "soft-delete"; id: string; isDeleted: boolean }

export default function ProductsPage() {
  const [dialog, setDialog] = useState<DialogState>({ type: "none" })
  const navigate = useNavigate()
  const isSoftDelete = dialog.type === "soft-delete"
  const isRestore = isSoftDelete && dialog.isDeleted

  const close = useCallback(() => setDialog({ type: "none" }), [])

  const handleView = useCallback(
    (id: string) => navigate(`/products/${id}`),
    [navigate]
  )
  const handleCreate = useCallback(() => navigate("/products/new"), [navigate])
  const handleSoftDelete = useCallback(
    (id: string, isDeleted: boolean) =>
      setDialog({ type: "soft-delete", id, isDeleted }),
    []
  )

  return (
    <div className="container mx-auto mt-4">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Products</h1>
        <Button onClick={handleCreate}>+ New Product</Button>
      </div>

      <ProductDataTable onView={handleView} onSoftDelete={handleSoftDelete} />

      <ConfirmDialog
        open={isSoftDelete}
        title={isRestore ? "Restore Product" : "Delete Product"}
        description={
          isRestore
            ? "This will restore the product and make it active again."
            : "This will soft-delete the product. It can be restored later."
        }
        confirmLabel={isRestore ? "Restore" : "Delete"}
        variant={isRestore ? "default" : "destructive"}
        onConfirm={() => {
          if (dialog.type !== "soft-delete") return Promise.resolve()
          return isRestore
            ? ProductService.restore(dialog.id)
            : ProductService.softDelete(dialog.id)
        }}
        onClose={close}
        invalidateKey={["products"]}
        successMsg={
          isRestore
            ? "Product restore successfully."
            : "Product soft-delete successfully."
        }
      />
    </div>
  )
}
