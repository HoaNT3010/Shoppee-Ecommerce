import App from "@/App"
import { ProtectedRoute } from "@/components/auth/protected-route"
import AccountPage from "@/pages/account"
import AccountNotificationsPage from "@/pages/account-notifications"
import AccountSettingsPage from "@/pages/account-settings"
import CategoriesPage from "@/pages/categories"
import CustomersPage from "@/pages/customers"
import DashboardPage from "@/pages/dashboard"
import LoginPage from "@/pages/login"
import OrdersPage from "@/pages/orders"
import ProductsPage from "@/pages/products"
import BasicInfoStep from "@/pages/products/create"
import ImagesStep from "@/pages/products/create/images-step"
import MainImageStep from "@/pages/products/create/main-image-step"
import PublishStep from "@/pages/products/create/publish-step"
import ProductDetailPage from "@/pages/products/detail"
import SystemSettingsPage from "@/pages/system-settings"
import UnauthorizedPage from "@/pages/unauthorized"
import { createBrowserRouter, Navigate } from "react-router"

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        path: "/",
        element: <App />, // App serves as our Layout (Sidebar + Header)
        children: [
          { index: true, element: <DashboardPage /> },
          { path: "categories", element: <CategoriesPage /> },
          { path: "products/new", element: <BasicInfoStep /> },
          { path: "products/new/:id/images", element: <ImagesStep /> },
          { path: "products/new/:id/main-image", element: <MainImageStep /> },
          { path: "products/new/:id/publish", element: <PublishStep /> },
          { path: "products/:id", element: <ProductDetailPage /> },
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
    ],
  },
  {
    path: "/unauthorized",
    element: <UnauthorizedPage />, // 403 error page
  },
  {
    path: "*",
    element: <Navigate to="/" replace />, // Redirect 404s to Dashboard
  },
])
