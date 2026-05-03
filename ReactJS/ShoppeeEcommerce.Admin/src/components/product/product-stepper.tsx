import { Check } from "lucide-react"

const STEPS = [
  { label: "Basic Info" },
  { label: "Images" },
  { label: "Main Image" },
  { label: "Publish" },
]

interface ProductStepperProps {
  currentStep: number // 1-4
  productId?: string
}

export function ProductStepper({ currentStep }: ProductStepperProps) {
  return (
    <div className="flex items-center gap-0">
      {STEPS.map((step, index) => {
        const stepNumber = index + 1
        const isCompleted = stepNumber < currentStep
        const isActive = stepNumber === currentStep

        return (
          <div key={step.label} className="flex flex-1 items-center">
            {/* Step circle + label */}
            <div className="flex flex-col items-center gap-1.5">
              <div
                className={`flex h-8 w-8 items-center justify-center rounded-full border-2 text-xs font-semibold transition-colors ${
                  isCompleted
                    ? "border-primary bg-primary text-primary-foreground"
                    : isActive
                      ? "border-primary text-primary"
                      : "border-muted text-muted-foreground"
                }`}
              >
                {isCompleted ? <Check className="h-4 w-4" /> : stepNumber}
              </div>
              <span
                className={`text-xs font-medium ${
                  isActive ? "text-primary" : "text-muted-foreground"
                }`}
              >
                {step.label}
              </span>
            </div>

            {/* Connector line — skip after last step */}
            {index < STEPS.length - 1 && (
              <div
                className={`mb-5 h-0.5 flex-1 transition-colors ${
                  isCompleted ? "bg-primary" : "bg-muted"
                }`}
              />
            )}
          </div>
        )
      })}
    </div>
  )
}
