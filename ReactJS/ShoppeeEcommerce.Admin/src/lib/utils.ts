import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

const fixUtc = (s: string) => s + "Z"

export function formatDate(utc: string) {
  return new Date(fixUtc(utc)).toLocaleDateString("en-GB").replace(/\//g, "-")
}

export function formatDateTime(utc: string) {
  const d = new Date(fixUtc(utc))
  return (
    d.toLocaleDateString("en-GB").replace(/\//g, "-") +
    " " +
    d.toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    })
  )
}
