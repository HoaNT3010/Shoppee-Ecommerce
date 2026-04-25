import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import AppSidebar from "./components/sidebar/AppSidebar"
import { Outlet } from "react-router"
import { PageTitle } from "./components/page-title"

export function App() {
  return (
    <SidebarProvider defaultOpen={true}>
      <PageTitle />
      <div className="flex min-h-screen w-full">
        <AppSidebar />
        <main className="flex-1 p-6">
          <header className="flex h-16 items-center border-b px-6">
            {/* This button lets you collapse the sidebar */}
            <SidebarTrigger />
            <h2 className="ml-4 text-sm font-medium">Shoppee Admin</h2>
          </header>
          <div className="flex-1 p-6">
            {/* THIS IS WHERE THE PAGES WILL RENDER */}
            <Outlet />
          </div>
        </main>
      </div>
    </SidebarProvider>
  )
}

export default App
