import { Bell, ChevronRightIcon, LogOut, Settings2, User2 } from "lucide-react"
import {
  SidebarFooter,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../ui/sidebar"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar"
import AppThemeToggler from "./AppThemeToggler"
import { Link, useNavigate } from "react-router"
import { useAuth } from "@/contexts/auth-context"

const AppSidebarFooter = () => {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  // Fallback values if user data is still loading or unavailable
  const username = user?.username || "Admin User"
  const email = user?.email || "admin@shoppee.com"
  const avatarUrl = "/images/common/default-avatar.jpg"

  const handleLogout = async () => {
    await logout()
    // Back to login
    navigate("/login")
  }

  return (
    <SidebarFooter>
      <SidebarMenu>
        {/* Theme toggler */}
        <AppThemeToggler />

        {/* Main footer content */}
        <SidebarMenuItem>
          <DropdownMenu>
            <DropdownMenuTrigger
              className="hover:cursor-pointer"
              render={
                <SidebarMenuButton
                  size="lg"
                  className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
                />
              }
            >
              <Avatar className="h-8 w-8 rounded-lg">
                <AvatarImage src={avatarUrl} alt={username} />
                <AvatarFallback className="rounded-lg">CN</AvatarFallback>
              </Avatar>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-medium">{username}</span>
                <span className="truncate text-xs">{email}</span>
              </div>
              <ChevronRightIcon className="ml-auto size-4" />
            </DropdownMenuTrigger>
            <DropdownMenuContent
              className="w-(--radix-dropdown-menu-trigger-width) min-w-56 rounded-lg"
              side={"right"}
              align="end"
              sideOffset={4}
            >
              <DropdownMenuGroup>
                <DropdownMenuLabel className="p-0 font-normal">
                  <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                    <Avatar className="h-8 w-8 rounded-lg">
                      <AvatarImage src={avatarUrl} alt={username} />
                      <AvatarFallback className="rounded-lg">CN</AvatarFallback>
                    </Avatar>
                    <div className="grid flex-1 text-left text-sm leading-tight">
                      <span className="truncate font-medium">{username}</span>
                      <span className="truncate text-xs">{email}</span>
                    </div>
                  </div>
                </DropdownMenuLabel>
              </DropdownMenuGroup>
              <DropdownMenuSeparator />
              <DropdownMenuGroup>
                <Link to={"account"}>
                  <DropdownMenuItem className="hover:cursor-pointer">
                    <User2 />
                    Account
                  </DropdownMenuItem>
                </Link>
                <Link to={"account/settings"}>
                  <DropdownMenuItem className="hover:cursor-pointer">
                    <Settings2 />
                    Settings
                  </DropdownMenuItem>
                </Link>
                <Link to={"account/notifications"}>
                  <DropdownMenuItem className="hover:cursor-pointer">
                    <Bell />
                    Notifications
                  </DropdownMenuItem>
                </Link>
              </DropdownMenuGroup>
              <DropdownMenuSeparator />
              <DropdownMenuGroup>
                <DropdownMenuItem
                  className="text-destructive hover:cursor-pointer focus:text-destructive"
                  onClick={handleLogout}
                >
                  <LogOut className="mr-2 h-4 w-4" />
                  Log out
                </DropdownMenuItem>
              </DropdownMenuGroup>
            </DropdownMenuContent>
          </DropdownMenu>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarFooter>
  )
}

export default AppSidebarFooter
