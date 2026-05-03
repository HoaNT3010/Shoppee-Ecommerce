import { useRef } from "react"
import { Upload, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "../ui/input"

const ACCEPTED_FORMATS = ["image/jpg", "image/jpeg", "image/png", "image/webp"]
const MAX_FILES = 10

interface ImageDropzoneProps {
  files: File[]
  onChange: (files: File[]) => void
  error?: string
}

export function ImageDropzone({ files, onChange, error }: ImageDropzoneProps) {
  const inputRef = useRef<HTMLInputElement>(null)

  function handleFiles(incoming: FileList | null) {
    if (!incoming) return

    const valid = Array.from(incoming).filter((f) =>
      ACCEPTED_FORMATS.includes(f.type)
    )
    const merged = [...files, ...valid].slice(0, MAX_FILES)
    onChange(merged)
  }

  function remove(index: number) {
    onChange(files.filter((_, i) => i !== index))
  }

  function handleDrop(e: React.DragEvent) {
    e.preventDefault()
    handleFiles(e.dataTransfer.files)
  }

  return (
    <div className="space-y-4">
      {/* Drop zone */}
      <div
        onDrop={handleDrop}
        onDragOver={(e) => e.preventDefault()}
        onClick={() => inputRef.current?.click()}
        className={`flex cursor-pointer flex-col items-center justify-center gap-3 rounded-lg border-2 border-dashed p-10 transition-colors hover:bg-muted/50 ${
          error ? "border-destructive" : "border-muted-foreground/30"
        }`}
      >
        <div className="flex h-12 w-12 items-center justify-center rounded-full bg-muted">
          <Upload className="h-5 w-5 text-muted-foreground" />
        </div>
        <div className="text-center">
          <p className="text-sm font-medium">
            Drop images here or{" "}
            <span className="text-primary underline underline-offset-2">
              browse
            </span>
          </p>
          <p className="mt-1 text-xs text-muted-foreground">
            JPG, PNG, WEBP — up to {MAX_FILES} images
          </p>
        </div>
        <Input
          ref={inputRef}
          type="file"
          accept=".jpg,.jpeg,.png,.webp"
          multiple
          className="hidden"
          onChange={(e) => handleFiles(e.target.files)}
        />
      </div>

      {/* Error */}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {/* Preview grid */}
      {files.length > 0 && (
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <p className="text-sm font-medium">
              Selected ({files.length}/{MAX_FILES})
            </p>
            <Button
              type="button"
              variant="ghost"
              size="sm"
              className="text-muted-foreground"
              onClick={() => onChange([])}
            >
              Clear all
            </Button>
          </div>
          <div className="grid grid-cols-5 gap-2">
            {files.map((file, index) => (
              <div key={index} className="group relative aspect-square">
                <img
                  src={URL.createObjectURL(file)}
                  alt={`Preview ${index + 1}`}
                  className="h-full w-full rounded-md border object-cover"
                />
                {/* Remove button */}
                <Button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation()
                    remove(index)
                  }}
                  className="text-destructive-foreground absolute -top-1.5 -right-1.5 hidden h-5 w-5 items-center justify-center rounded-full bg-destructive shadow group-hover:flex"
                >
                  <X className="h-3 w-3" />
                </Button>
                {/* Order badge */}
                <span className="absolute bottom-1 left-1 rounded bg-black/50 px-1 text-[10px] text-white">
                  {index + 1}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
