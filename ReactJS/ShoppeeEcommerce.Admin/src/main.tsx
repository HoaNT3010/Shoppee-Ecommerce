import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import "./index.css"
import { ThemeProvider } from "@/components/theme-provider.tsx"
import { RouterProvider } from "react-router"
import { router } from "./lib/router.tsx"
import { TooltipProvider } from "./components/ui/tooltip.tsx"
import { AuthProvider } from "./contexts/auth-context.tsx"
import { Providers } from "./components/providers.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AuthProvider>
      <ThemeProvider>
        <Providers>
          <TooltipProvider>
            <RouterProvider router={router} />
          </TooltipProvider>
        </Providers>
      </ThemeProvider>
    </AuthProvider>
  </StrictMode>
)
