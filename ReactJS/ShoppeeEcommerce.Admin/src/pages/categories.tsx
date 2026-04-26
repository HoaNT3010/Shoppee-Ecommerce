import { CategoryDataTable } from "./categories/data-table"

export default function CategoriesPage() {
  // const data = await CategoryService.list()

  return (
    <div className="container mx-auto py-10">
      <h1 className="mb-6 text-2xl font-bold">Categories</h1>
      <CategoryDataTable />
    </div>
  )
}
