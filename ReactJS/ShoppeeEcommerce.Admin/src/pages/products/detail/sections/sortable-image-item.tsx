import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import { GripVertical, Star } from "lucide-react"

interface SortableImageItemProps {
  image: {
    id: number
    url: string
    isMain: boolean
    displayOrder: number
  }
  isSelected: boolean
  onSelect: (id: number) => void
}

export function SortableImageItem({
  image,
  isSelected,
  onSelect,
}: SortableImageItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: image.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.4 : 1,
    zIndex: isDragging ? 10 : undefined,
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={`group relative aspect-square overflow-hidden rounded-lg border-2 transition-colors ${
        isSelected
          ? "border-primary shadow-md"
          : "border-transparent hover:border-muted-foreground/30"
      }`}
    >
      <img
        src={image.url}
        alt={`Product image ${image.displayOrder}`}
        className="h-full w-full object-cover"
      />

      {/* Drag handle — top left */}
      <button
        {...attributes}
        {...listeners}
        className="absolute top-1.5 left-1.5 hidden h-6 w-6 cursor-grab items-center justify-center rounded bg-black/50 text-white group-hover:flex active:cursor-grabbing"
      >
        <GripVertical className="h-3.5 w-3.5" />
      </button>

      {/* Main image badge */}
      {image.isMain && (
        <div className="absolute bottom-1.5 left-1.5 flex items-center gap-1 rounded-full bg-black/60 px-2 py-0.5">
          <Star className="h-2.5 w-2.5 fill-yellow-400 text-yellow-400" />
          <span className="text-[10px] font-medium text-white">Main</span>
        </div>
      )}

      {/* Set as main overlay — shown on hover if not already main */}
      {!image.isMain && (
        <button
          onClick={() => onSelect(image.id)}
          className="absolute inset-x-0 bottom-0 hidden bg-black/60 py-1.5 text-center text-[11px] font-medium text-white group-hover:block"
        >
          Set as main
        </button>
      )}

      {/* Display order */}
      <span className="absolute top-1.5 right-1.5 rounded bg-black/50 px-1.5 py-0.5 text-[10px] text-white">
        {image.displayOrder}
      </span>
    </div>
  )
}
