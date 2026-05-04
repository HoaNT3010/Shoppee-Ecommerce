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
import { toast } from "sonner"

interface ConfirmDialogProps {
  open: boolean
  title: string
  description: string
  confirmLabel: string
  variant: "default" | "destructive"
  invalidateKey: unknown[]
  onConfirm: () => Promise<void>
  onClose: () => void
  successMsg?: string
  onSuccess?: () => void
}

export function ConfirmDialog({
  open,
  title,
  description,
  confirmLabel,
  variant,
  invalidateKey,
  onConfirm,
  onClose,
  successMsg = "Done.",
  onSuccess,
}: ConfirmDialogProps) {
  const queryClient = useQueryClient()
  const mutation = useMutation({
    mutationFn: onConfirm,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: invalidateKey })
      onSuccess?.()
      toast.success(successMsg)
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
