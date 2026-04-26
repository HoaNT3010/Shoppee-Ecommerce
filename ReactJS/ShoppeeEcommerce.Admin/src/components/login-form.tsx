import { useState } from "react"
import { useForm } from "react-hook-form"
import { useLocation, useNavigate } from "react-router"
import { useAuth } from "@/contexts/auth-context"
import api from "@/lib/api"
import { cn } from "@/lib/utils" // Standard shadcn utility

// UI Components
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip"
import { Loader2 } from "lucide-react"
import { parseBackendError } from "@/lib/error-parser"
import { Label } from "./ui/label"

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [globalError, setGlobalError] = useState<string | null>(null)

  const from = location.state?.from?.pathname || "/"

  // Initialize the form hook
  const {
    register,
    handleSubmit,
    setError: setFormFieldError,
    formState: { errors },
  } = useForm({
    defaultValues: {
      email: "",
      password: "",
    },
  })

  const onSubmit = async (values: any) => {
    setIsSubmitting(true)
    setGlobalError(null)

    try {
      const response = await api.post("/auth/login", values)
      const isAuthorized = await login(response.data)
      if (!isAuthorized) {
        setGlobalError("Access denied. Admin privileges required.")
        return
      }
      navigate(from, { replace: true })
    } catch (err: any) {
      const { message, fieldErrors } = parseBackendError(err)
      if (fieldErrors && Object.keys(fieldErrors).length > 0) {
        Object.entries(fieldErrors).forEach(([field, msg]) => {
          setFormFieldError(field as any, { type: "server", message: msg })
        })
      } else {
        setGlobalError(message)
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader className="text-center">
          <CardTitle className="text-xl">ShoppeeEcommerce</CardTitle>
          <CardDescription>Admin Management Portal</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid gap-6">
              {globalError && (
                <div className="rounded-md bg-destructive/10 p-3 text-center text-sm font-medium text-destructive">
                  {globalError}
                </div>
              )}

              <div className="grid gap-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  {...register("email", { required: true })} // Connect to form
                  id="email"
                  type="email"
                  placeholder="admin@example.com"
                  disabled={isSubmitting}
                  className={
                    errors.email
                      ? "border-destructive focus-visible:ring-destructive"
                      : ""
                  }
                />
                {errors.email && (
                  <p className="text-xs font-medium text-destructive">
                    {errors.email.message}
                  </p>
                )}
              </div>

              <div className="grid gap-2">
                <div className="flex items-center">
                  <Label htmlFor="password">Password</Label>
                  <Tooltip>
                    <TooltipTrigger
                      type="button"
                      className="ml-auto text-sm underline-offset-4 hover:cursor-help hover:underline"
                    >
                      Forgot your password?
                    </TooltipTrigger>
                    <TooltipContent side="right">
                      <p>Please contact creator for password recovery.</p>
                    </TooltipContent>
                  </Tooltip>
                </div>
                <Input
                  {...register("password", { required: true })} // Connect to form
                  id="password"
                  type="password"
                  disabled={isSubmitting}
                  className={
                    errors.password
                      ? "border-destructive focus-visible:ring-destructive"
                      : ""
                  }
                />
                {errors.password && (
                  <p className="text-xs font-medium text-destructive">
                    {errors.password.message}
                  </p>
                )}
              </div>

              <div className="grid gap-4">
                <Button
                  type="submit"
                  className="w-full hover:cursor-pointer"
                  disabled={isSubmitting}
                >
                  {isSubmitting && (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  )}
                  Login
                </Button>

                <Tooltip>
                  <TooltipTrigger
                    type="button"
                    className="text-center text-sm underline-offset-4 hover:cursor-help hover:underline"
                  >
                    Don&apos;t have an account?
                  </TooltipTrigger>
                  <TooltipContent side="bottom">
                    <p>Try admin1@gmail.com - AdminPass123!</p>
                  </TooltipContent>
                </Tooltip>
              </div>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
