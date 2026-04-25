import {
  ShoppingCart,
  Users,
  Settings,
  ChartBarStacked,
  Home,
  Package,
} from "lucide-react"
import { Sidebar, SidebarContent } from "../ui/sidebar"
import AppSidebarHeader from "./AppSidebarHeader"
import AppSidebarFooter from "./AppSidebarFooter"
import AppSidebarNavigation from "./AppSidebarNavigation"

const data = {
  main: [
    { title: "Dashboard", url: "/", icon: Home },
    { title: "Categories", url: "/categories", icon: ChartBarStacked },
    { title: "Products", url: "/products", icon: Package },
    { title: "Customers", url: "/customers", icon: Users },
    { title: "Orders", url: "/orders", icon: ShoppingCart },
  ],
  system: [{ title: "Settings", url: "/settings", icon: Settings }],
}

const AppSidebar = () => {
  return (
    <Sidebar>
      {/* Header start here */}
      <AppSidebarHeader />

      {/* Content start here */}
      <SidebarContent>
        {/* Main */}
        <AppSidebarNavigation label="Main Management" items={data.main} />
        {/* System */}
        <AppSidebarNavigation label="System" items={data.system} />
      </SidebarContent>

      {/* Footer start here */}
      <AppSidebarFooter username="Hoa Nguyen" />
    </Sidebar>
  )
}

export default AppSidebar
