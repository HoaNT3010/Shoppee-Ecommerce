import type { ApiError } from "@/types/common"

export type ParsedError = {
  message: string
  fieldErrors?: Record<string, string>
}

export const parseBackendError = (error: any): ParsedError => {
  const data = error.response?.data

  // Handle Domain Errors (Array: [{ description: "..." }])
  if (Array.isArray(data)) {
    return {
      message: data[0]?.description || "An unexpected error occurred.",
    }
  }

  // Handle FluentValidation Errors (Dictionary: { "Password": ["..."] })
  if (typeof data === "object" && data !== null) {
    const fieldErrors: Record<string, string> = {}

    Object.keys(data).forEach((key) => {
      // Map to lowercase to match React form names (email, password)
      const formKey = key.charAt(0).toLowerCase() + key.slice(1)
      fieldErrors[formKey] = data[key][0]
    })

    // Use the first field error as the primary message
    const firstKey = Object.keys(fieldErrors)[0]
    return {
      message: fieldErrors[firstKey],
      fieldErrors,
    }
  }

  // Fallback for 500s, Network Errors, or strings
  return {
    message: typeof data === "string" ? data : "Server connection failed.",
  }
}

export function getFieldError(
  errors: ApiError[],
  code: string
): string | undefined {
  return errors.find((e) => e.code === code)?.description
}

export interface ParsedUpdateErrors {
  // Field-level errors from FluentValidation
  fieldErrors: Record<string, string>
  // Domain errors (SKU conflict etc.)
  domainErrors: ApiError[]
}

export function parseUpdateErrors(error: any): ParsedUpdateErrors {
  const data = error?.response?.data

  // FluentValidation shape: { "Name": ["message"], "Price": ["message"] }
  if (data && !Array.isArray(data) && typeof data === "object") {
    const fieldErrors: Record<string, string> = {}
    for (const [key, messages] of Object.entries(data)) {
      if (Array.isArray(messages) && messages.length > 0) {
        // Lowercase first letter to match react-hook-form field names
        const fieldKey = key.charAt(0).toLowerCase() + key.slice(1)
        fieldErrors[fieldKey] = messages[0] as string
      }
    }
    return { fieldErrors, domainErrors: [] }
  }

  // Domain error shape: [{ code, description, type }]
  if (Array.isArray(data)) {
    return { fieldErrors: {}, domainErrors: data as ApiError[] }
  }

  return { fieldErrors: {}, domainErrors: [] }
}
