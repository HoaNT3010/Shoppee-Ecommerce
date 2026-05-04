import { useRef } from "react"
import {
  DndContext,
  closestCenter,
  PointerSensor,
  KeyboardSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core"
import {
  SortableContext,
  rectSortingStrategy,
  sortableKeyboardCoordinates,
} from "@dnd-kit/sortable"
import { ImagePlus, Save } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { SortableImageItem } from "./sortable-image-item"
import { useImageSection } from "./useImageSection"
import type { DetailedProductResponse } from "@/types/product"
import { toast } from "sonner"
import { Input } from "@/components/ui/input"

const ACCEPTED_FORMATS = ".jpg,.jpeg,.png,.webp"
const MAX_IMAGES = 10

interface ImagesSectionProps {
  product: DetailedProductResponse
}

export function ImagesSection({ product }: ImagesSectionProps) {
  const uploadInputRef = useRef<HTMLInputElement>(null)

  const {
    images,
    isDirty,
    handleDragEnd,
    reorderMutation,
    setMainMutation,
    uploadMutation,
  } = useImageSection(product.id, product.images)

  // dnd-kit sensors — PointerSensor for mouse/touch, KeyboardSensor for a11y
  const sensors = useSensors(
    useSensor(PointerSensor, {
      // Require 8px movement before drag starts
      // so clicks (like "Set as main") still register
      activationConstraint: { distance: 8 },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  function handleUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const files = Array.from(e.target.files ?? [])
    if (!files.length) return

    const remaining = MAX_IMAGES - images.length
    if (remaining <= 0) {
      toast.error("Maximum of 10 images reached.")
      return
    }

    uploadMutation.mutate(files.slice(0, remaining))

    // Reset input so the same files can be selected again if needed
    e.target.value = ""
  }

  const canUploadMore = images.length < MAX_IMAGES

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between pb-3">
        <div>
          <CardTitle className="text-base">Images</CardTitle>
          <p className="mt-0.5 text-xs text-muted-foreground">
            {images.length}/{MAX_IMAGES} images · Drag to reorder
          </p>
        </div>
        <div className="flex items-center gap-2">
          {/* Save order button — only shown when reorder is pending */}
          {isDirty && (
            <Button
              size="sm"
              variant="outline"
              className="gap-2"
              onClick={() => reorderMutation.mutate()}
              disabled={reorderMutation.isPending}
            >
              <Save className="h-3.5 w-3.5" />
              {reorderMutation.isPending ? "Saving..." : "Save Order"}
            </Button>
          )}

          {/* Upload more */}
          {canUploadMore && (
            <Button
              size="sm"
              variant="outline"
              className="gap-2"
              onClick={() => uploadInputRef.current?.click()}
              disabled={uploadMutation.isPending}
            >
              <ImagePlus className="h-3.5 w-3.5" />
              {uploadMutation.isPending ? "Uploading..." : "Add Images"}
            </Button>
          )}
          <Input
            ref={uploadInputRef}
            type="file"
            accept={ACCEPTED_FORMATS}
            multiple
            className="hidden"
            onChange={handleUpload}
          />
        </div>
      </CardHeader>

      <CardContent>
        {images.length === 0 ? (
          <div className="flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed py-12">
            <p className="text-sm text-muted-foreground">
              No images uploaded yet.
            </p>
            {canUploadMore && (
              <Button
                variant="outline"
                size="sm"
                onClick={() => uploadInputRef.current?.click()}
              >
                Upload Images
              </Button>
            )}
          </div>
        ) : (
          <DndContext
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handleDragEnd}
          >
            <SortableContext
              items={images.map((img) => img.id)}
              strategy={rectSortingStrategy}
            >
              <div className="grid grid-cols-4 gap-3 sm:grid-cols-5">
                {images.map((image) => (
                  <SortableImageItem
                    key={image.id}
                    image={image}
                    isSelected={false}
                    onSelect={(id) => setMainMutation.mutate(id)}
                  />
                ))}
              </div>
            </SortableContext>
          </DndContext>
        )}

        {/* Hint text */}
        {images.length > 0 && (
          <p className="mt-3 text-xs text-muted-foreground">
            Drag images to reorder · Hover an image to set it as main
          </p>
        )}
      </CardContent>
    </Card>
  )
}
