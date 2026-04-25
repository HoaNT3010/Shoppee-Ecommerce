import App from "@/App"
import AccountPage from "@/pages/account"
import AccountNotificationsPage from "@/pages/account-notifications"
import AccountSettingsPage from "@/pages/account-settings"
import CategoriesPage from "@/pages/categories"
import CustomersPage from "@/pages/customers"
import DashboardPage from "@/pages/Dashboard"
import LoginPage from "@/pages/login"
import OrdersPage from "@/pages/orders"
import ProductsPage from "@/pages/products"
import SystemSettingsPage from "@/pages/system-settings"
import { createBrowserRouter, Navigate } from "react-router"

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/",
    element: <App />, // App serves as our Layout (Sidebar + Header)
    children: [
      { index: true, element: <DashboardPage /> },
      { path: "categories", element: <CategoriesPage /> },
      { path: "products", element: <ProductsPage /> },
      { path: "customers", element: <CustomersPage /> },
      { path: "orders", element: <OrdersPage /> },
      { path: "settings", element: <SystemSettingsPage /> },
      {
        path: "account",
        children: [
          { index: true, element: <AccountPage /> },
          { path: "settings", element: <AccountSettingsPage /> },
          { path: "notifications", element: <AccountNotificationsPage /> },
        ],
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />, // Redirect 404s to Dashboard
  },
])
