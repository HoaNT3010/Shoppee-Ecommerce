import { useNavigate } from "react-router"
import { ChevronLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card"
import { ProductStepper } from "./product-stepper"

interface StepLayoutProps {
  currentStep: number // 1-4
  productId?: string // undefined on step 1
  title: string
  description: string
  children: React.ReactNode
}

export function StepLayout({
  currentStep,
  productId,
  title,
  description,
  children,
}: StepLayoutProps) {
  const navigate = useNavigate()

  return (
    <div className="container mx-auto max-w-2xl py-10">
      {/* Back to products list */}
      <Button
        variant="ghost"
        className="mb-6 gap-2 text-muted-foreground"
        onClick={() => navigate("/products")}
      >
        <ChevronLeft className="h-4 w-4" />
        Back to Products
      </Button>

      {/* Stepper */}
      <ProductStepper currentStep={currentStep} productId={productId} />

      {/* Step card */}
      <Card className="mt-8">
        <CardHeader>
          <CardTitle>{title}</CardTitle>
          <CardDescription>{description}</CardDescription>
        </CardHeader>
        <CardContent>{children}</CardContent>
      </Card>
    </div>
  )
}
