import { Store } from "lucide-react"
import {
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../ui/sidebar"

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
        <SidebarMenuItem>
          <SidebarMenuButton size={"lg"}>
            <a href={url} className="flex items-center gap-3">
              <div className="flex aspect-square size-10 items-center justify-center rounded-lg bg-primary text-primary-foreground">
                <Store className="size-6" />
              </div>
              <div className="flex flex-col gap-0.5 leading-none">
                <span className="text-base font-bold">{mainTitle}</span>
                <span className="text-xs text-muted-foreground">
                  {altTitle}
                </span>
              </div>
            </a>
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarHeader>
  )
}

export default AppSidebarHeader
