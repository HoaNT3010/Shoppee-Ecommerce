import { SidebarMenuButton, SidebarMenuItem } from "../ui/sidebar"
import { Monitor, Moon, Sun } from "lucide-react"
import { useTheme } from "../theme-provider"

const AppThemeToggler = () => {
  const { theme, setTheme } = useTheme()
  // Logic to rotate through themes: Light -> Dark -> System
  const toggleTheme = () => {
    if (theme === "light") setTheme("dark")
    else if (theme === "dark") setTheme("system")
    else setTheme("light")
  }

  return (
    <SidebarMenuItem>
      <SidebarMenuButton onClick={toggleTheme} tooltip="Change Theme">
        {theme === "light" && <Sun className="size-4" />}
        {theme === "dark" && <Moon className="size-4" />}
        {theme === "system" && <Monitor className="size-4" />}
        <span className="capitalize">{theme} Mode</span>
      </SidebarMenuButton>
    </SidebarMenuItem>
  )
}

export default AppThemeToggler
