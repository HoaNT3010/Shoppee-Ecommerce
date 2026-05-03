import { useQuery } from "@tanstack/react-query"
import { Check, ChevronsUpDown, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command"
import CategoryService from "@/services/category-service"

interface CategoryMultiSelectProps {
  value: string[]
  onChange: (value: string[]) => void
  error?: string
}

export function CategoryMultiSelect({
  value,
  onChange,
  error,
}: CategoryMultiSelectProps) {
  const { data: categories, isLoading } = useQuery({
    queryKey: ["categories-general"],
    queryFn: () => CategoryService.generalList(),
  })

  function toggle(id: string) {
    if (value.includes(id)) {
      onChange(value.filter((v) => v !== id))
    } else if (value.length < 10) {
      onChange([...value, id])
    }
  }

  function remove(id: string) {
    onChange(value.filter((v) => v !== id))
  }

  const selectedCategories =
    categories?.filter((c) => value.includes(c.id)) ?? []

  return (
    <div className="space-y-2">
      <Popover>
        <PopoverTrigger>
          <Button
            variant="outline"
            role="combobox"
            className="w-full justify-between font-normal"
          >
            {value.length > 0
              ? `${value.length} categor${value.length === 1 ? "y" : "ies"} selected`
              : "Select categories..."}
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-full p-0" align="start">
          <Command>
            <CommandInput placeholder="Search categories..." />
            <CommandList>
              <CommandEmpty>
                {isLoading ? "Loading..." : "No categories found."}
              </CommandEmpty>
              <CommandGroup>
                {categories?.map((category) => {
                  const isSelected = value.includes(category.id)
                  const isDisabled = !isSelected && value.length >= 10
                  return (
                    <CommandItem
                      key={category.id}
                      value={category.name}
                      onSelect={() => toggle(category.id)}
                      disabled={isDisabled}
                      className={
                        isDisabled ? "cursor-not-allowed opacity-40" : ""
                      }
                    >
                      <div
                        className={`mr-2 flex h-4 w-4 items-center justify-center rounded-sm border ${
                          isSelected
                            ? "border-primary bg-primary"
                            : "border-muted-foreground"
                        }`}
                      >
                        {isSelected && (
                          <Check className="h-3 w-3 text-primary-foreground" />
                        )}
                      </div>
                      {category.name}
                    </CommandItem>
                  )
                })}
              </CommandGroup>
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>

      {/* Selected category badges */}
      {selectedCategories.length > 0 && (
        <div className="flex flex-wrap gap-1.5">
          {selectedCategories.map((category) => (
            <Badge key={category.id} variant="secondary" className="gap-1 pr-1">
              {category.name}
              <Button
                type="button"
                onClick={() => remove(category.id)}
                className="ml-1 rounded-full hover:bg-muted"
              >
                <X className="h-3 w-3" />
              </Button>
            </Badge>
          ))}
        </div>
      )}

      {/* Max categories hint or error */}
      {error ? (
        <p className="text-sm text-destructive">{error}</p>
      ) : (
        <p className="text-xs text-muted-foreground">
          {value.length}/10 categories selected
        </p>
      )}
    </div>
  )
}
