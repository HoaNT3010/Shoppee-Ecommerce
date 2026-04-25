import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import AppSidebar from "./components/sidebar/AppSidebar"

export function App() {
  return (
    <SidebarProvider defaultOpen={true}>
      <div className="flex min-h-screen w-full">
        <AppSidebar />
        <main className="flex-1 p-6">
          {/* This button lets you collapse the sidebar */}
          <SidebarTrigger />

          <div className="mt-4">
            <h1 className="text-2xl font-bold">Admin Dashboard</h1>
          </div>
        </main>
      </div>
    </SidebarProvider>
  )
}

export default App
