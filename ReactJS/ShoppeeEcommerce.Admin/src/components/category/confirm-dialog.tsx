"use client"

import { useMutation, useQueryClient } from "@tanstack/react-query"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import CategoryService from "@/services/category-service"
import { toast } from "sonner"

interface ConfirmDialogProps {
  open: boolean
  title: string
  description: string
  confirmLabel: string
  variant: "default" | "destructive"
  id?: string
  action: "soft-delete" | "hard-delete"
  isRestore?: boolean
  onClose: () => void
}

export function ConfirmDialog({
  open,
  title,
  description,
  confirmLabel,
  variant,
  id,
  action,
  isRestore,
  onClose,
}: ConfirmDialogProps) {
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: () => {
      if (action === "hard-delete") return CategoryService.hardDelete(id!)
      if (isRestore) return CategoryService.restore(id!)
      return CategoryService.softDelete(id!)
    },
    onSuccess: () => {
      if (action === "hard-delete")
        toast.success("Category permanently deleted.")
      else if (isRestore) toast.success("Category restored.")
      else toast.success("Category deleted.")
      queryClient.invalidateQueries({ queryKey: ["categories"] })
      onClose()
    },
    onError: () => {
      toast.error("Something went wrong. Please try again.")
    },
  })

  return (
    <AlertDialog open={open} onOpenChange={(v) => !v && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>
          <AlertDialogDescription>{description}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Cancel</AlertDialogCancel>
          <AlertDialogAction
            className={
              variant === "destructive"
                ? "text-destructive-foreground bg-destructive hover:bg-destructive/90"
                : ""
            }
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending}
          >
            {mutation.isPending ? "Please wait..." : confirmLabel}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
