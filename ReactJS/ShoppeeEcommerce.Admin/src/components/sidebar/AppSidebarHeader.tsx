import { Store } from "lucide-react"
import {
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../ui/sidebar"
import { Link } from "react-router"

interface Props {
  mainTitle?: string
  altTitle?: string
  url?: string
}

const AppSidebarHeader = ({
  mainTitle = "ShoppeeEcommerce",
  altTitle = "Management Portal",
  url = "/",
}: Props) => {
  return (
    <SidebarHeader className="border-b border-sidebar-border p-4">
      <SidebarMenu>
        <Link to={url}>
          <SidebarMenuItem>
            <SidebarMenuButton size={"lg"} className="hover:cursor-pointer">
              <div className="flex aspect-square size-10 items-center justify-center rounded-lg bg-primary text-primary-foreground">
                <Store className="size-6" />
              </div>
              <div className="flex flex-col gap-0.5 leading-none">
                <span className="text-base font-bold">{mainTitle}</span>
                <span className="text-xs text-muted-foreground">
                  {altTitle}
                </span>
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </Link>
      </SidebarMenu>
    </SidebarHeader>
  )
}

export default AppSidebarHeader
