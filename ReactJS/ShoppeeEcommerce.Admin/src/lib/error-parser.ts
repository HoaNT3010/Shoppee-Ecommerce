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
