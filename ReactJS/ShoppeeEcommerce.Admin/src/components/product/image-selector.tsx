import { Check, Star } from "lucide-react"
import type { CreateProductImageResponse } from "@/types/product"

interface ImageSelectorProps {
  images: CreateProductImageResponse[]
  selectedId?: number
  onSelect: (id: number) => void
}

export function ImageSelector({
  images,
  selectedId,
  onSelect,
}: ImageSelectorProps) {
  return (
    <div className="grid grid-cols-3 gap-3 sm:grid-cols-4">
      {images.map((image) => {
        const isSelected = image.id === selectedId
        const wasMain = image.isMain

        return (
          <button
            key={image.id}
            type="button"
            onClick={() => onSelect(image.id)}
            className={`group relative aspect-square overflow-hidden rounded-lg border-2 transition-all focus:outline-none focus-visible:ring-2 focus-visible:ring-primary ${
              isSelected
                ? "border-primary shadow-md"
                : "border-transparent hover:border-muted-foreground/40"
            }`}
          >
            <img
              src={image.url}
              alt={`Product image ${image.displayOrder}`}
              className="h-full w-full object-cover transition-transform group-hover:scale-105"
            />

            {/* Dark overlay when not selected */}
            {!isSelected && (
              <div className="absolute inset-0 bg-black/10 transition-colors group-hover:bg-black/0" />
            )}

            {/* Selected checkmark */}
            {isSelected && (
              <div className="absolute inset-0 bg-primary/10">
                <div className="absolute top-1.5 right-1.5 flex h-5 w-5 items-center justify-center rounded-full bg-primary shadow">
                  <Check className="h-3 w-3 text-primary-foreground" />
                </div>
              </div>
            )}

            {/* Previously set main badge — shown for draft pickup */}
            {wasMain && !isSelected && (
              <div className="absolute top-1.5 left-1.5 flex items-center gap-1 rounded-full bg-black/50 px-1.5 py-0.5">
                <Star className="h-2.5 w-2.5 text-yellow-400" />
                <span className="text-[10px] text-white">Main</span>
              </div>
            )}

            {/* Display order badge */}
            <span className="absolute bottom-1 left-1 rounded bg-black/50 px-1 text-[10px] text-white">
              {image.displayOrder}
            </span>
          </button>
        )
      })}
    </div>
  )
}
