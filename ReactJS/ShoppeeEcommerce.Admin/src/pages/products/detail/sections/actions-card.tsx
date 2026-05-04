import { useState } from "react"
import { useNavigate } from "react-router"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { ConfirmDialog } from "@/components/category/confirm-dialog"
import ProductService from "@/services/product-service"

interface ActionsCardProps {
  productId: string
}

export function ActionsCard({ productId }: ActionsCardProps) {
  const [confirmOpen, setConfirmOpen] = useState(false)
  const navigate = useNavigate()

  return (
    <Card className="border-destructive/40">
      <CardHeader className="pb-3">
        <CardTitle className="text-base text-destructive">
          Danger Zone
        </CardTitle>
      </CardHeader>
      <CardContent>
        <Button
          variant="destructive"
          className="w-full"
          onClick={() => setConfirmOpen(true)}
        >
          Permanently Delete
        </Button>
      </CardContent>

      <ConfirmDialog
        open={confirmOpen}
        title="Permanently Delete Product"
        description="This cannot be undone. The product and all its images will be permanently removed."
        confirmLabel="Delete Forever"
        variant="destructive"
        invalidateKey={["products"]}
        onConfirm={() => ProductService.hardDelete(productId)}
        onClose={() => setConfirmOpen(false)}
        successMsg="Product permanently deleted."
        onSuccess={() => {
          navigate("/products")
        }}
      />
    </Card>
  )
}
