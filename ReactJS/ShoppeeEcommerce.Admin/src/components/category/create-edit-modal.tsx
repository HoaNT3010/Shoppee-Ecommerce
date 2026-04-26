"use client"

import { useEffect } from "react"
import { useForm } from "react-hook-form"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import CategoryService from "@/services/category-service"
import type { UpdateCategoryRequest } from "@/types/category"
import { toast } from "sonner"

interface FormValues {
  name: string
  description: string
}

interface CreateEditModalProps {
  open: boolean
  id?: string // undefined = create mode, defined = edit mode
  onClose: () => void
}

export function CreateEditModal({ open, id, onClose }: CreateEditModalProps) {
  const isEdit = !!id
  const queryClient = useQueryClient()

  // Fetch existing data when in edit mode
  const { data: existing } = useQuery({
    queryKey: ["category", id],
    queryFn: () => CategoryService.getById(id!),
    enabled: isEdit && open,
  })

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>()

  // Pre-fill form when editing, clear when closed
  useEffect(() => {
    if (existing) {
      reset({ name: existing.name, description: existing.description })
    }
    if (!open) {
      reset({ name: "", description: "" })
    }
  }, [existing, open, reset])

  const mutation = useMutation({
    mutationFn: (values: FormValues) => {
      if (isEdit) {
        // Only send fields that actually changed (clean PATCH)
        const changes: UpdateCategoryRequest = { categoryId: id! }
        if (values.name !== existing?.name) changes.name = values.name
        if (values.description !== existing?.description)
          changes.description = values.description
        return CategoryService.update(changes)
      }
      // Create requires both fields
      return CategoryService.create({
        name: values.name,
        description: values.description,
      })
    },
    onSuccess: () => {
      toast.success(isEdit ? "Category updated." : "Category created.")
      queryClient.invalidateQueries({ queryKey: ["categories"] })
      if (isEdit) queryClient.invalidateQueries({ queryKey: ["category", id] })
      onClose()
    },
    onError: () => {
      toast.error(
        isEdit ? "Failed to update category." : "Failed to create category."
      )
    },
  })

  return (
    <Dialog open={open} onOpenChange={(v) => !v && onClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit Category" : "New Category"}</DialogTitle>
        </DialogHeader>

        <form
          onSubmit={handleSubmit((v) => mutation.mutate(v))}
          className="mt-4 space-y-4"
        >
          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              {...register("name", {
                required: isEdit ? false : "Name is required", // optional on PATCH
              })}
            />
            {errors.name && (
              <p className="text-sm text-destructive">{errors.name.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea id="description" {...register("description")} />
            {errors.description && (
              <p className="text-sm text-destructive">
                {errors.description.message}
              </p>
            )}
          </div>

          {mutation.isError && (
            <p className="text-sm text-destructive">
              Something went wrong. Please try again.
            </p>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending
                ? "Saving..."
                : isEdit
                  ? "Save changes"
                  : "Create"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
