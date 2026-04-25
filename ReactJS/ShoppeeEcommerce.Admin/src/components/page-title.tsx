import { useEffect } from "react"
import { useLocation } from "react-router"

const routeTitles: Record<string, string> = {
  "/": "Dashboard",
  "/categories": "Categories",
  "/products": "Products",
  "/customers": "Customers",
  "/orders": "Orders",
  "/settings": "System Settings",
  "/account": "Account",
  "/account/notifications": "Notifications",
  "/account/settings": "Account Settings",
  "/login": "Login",
}

export function PageTitle() {
  const { pathname } = useLocation()

  useEffect(() => {
    const pageTitle = routeTitles[pathname] || "Admin"
    document.title = `${pageTitle} - ShoppeeEcommerce`
  }, [pathname])

  return null
}
