import type { LucideIcon } from "lucide-react"
import {
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../ui/sidebar"
import { Link } from "react-router"

interface NavigationItem {
  title: string
  url: string
  icon: LucideIcon
}

interface Props {
  label?: string
  items: NavigationItem[]
}

const AppSidebarNavigation = ({ label, items }: Props) => {
  return (
    <SidebarGroup>
      {label && <SidebarGroupLabel>{label}</SidebarGroupLabel>}
      <SidebarGroupContent>
        <SidebarMenu>
          {items.map((item) => (
            <Link to={item.url}>
              <SidebarMenuItem key={item.title}>
                <SidebarMenuButton
                  tooltip={item.title}
                  className="hover:cursor-pointer"
                >
                  <item.icon className="mr-2 size-5" />
                  <span className="flex-1 font-medium">{item.title}</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </Link>
          ))}
        </SidebarMenu>
      </SidebarGroupContent>
    </SidebarGroup>
  )
}

export default AppSidebarNavigation
