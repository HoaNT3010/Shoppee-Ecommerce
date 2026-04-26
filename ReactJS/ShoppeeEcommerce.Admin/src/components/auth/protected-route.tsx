import { Navigate, Outlet, useLocation } from "react-router"
import { useAuth } from "@/contexts/auth-context"
import { Loader2 } from "lucide-react"

export const ProtectedRoute = () => {
  const { isAuthenticated, isLoading, user } = useAuth()
  const location = useLocation()

  // While checking localStorage/API for the session, show a loading screen
  if (isLoading) {
    return (
      <div className="flex h-screen w-full items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  // If not logged in, redirect to login page
  // Save the current location so we can redirect them back after they log in
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }
  // Optional: Role-based protection
  // App is only for Admins, kick anyone else out
  if (user?.roles.includes("Admin") === false) {
    console.log("Unauthorized")
    return <Navigate to="/unauthorized" replace />
  }
  //If all checks pass, render the child routes (the Sidebar + Page)
  return <Outlet />
}
