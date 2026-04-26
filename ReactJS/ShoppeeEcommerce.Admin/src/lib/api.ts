import { clearAuthCredentials } from "@/contexts/auth-context"
import axios from "axios"

const api = axios.create({
  baseURL: "http://localhost:8080/api/v1",
})

// Request Interceptor: Attach Access Token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("shoppee_access")
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})
// Response Interceptor: Handle Token Rotation
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    // If the 401 comes from the login endpoint, don't intercept it.
    if (originalRequest.url?.includes("/auth/login")) {
      return Promise.reject(error)
    }
    // If error is 401 and we haven't tried to refresh yet
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true
      const oldRefreshToken = localStorage.getItem("shoppee_refresh")

      if (!oldRefreshToken) {
        // If no refresh token exists, kick to login
        window.location.href = "/login"
        return Promise.reject(error)
      }

      try {
        // Call your rotation endpoint
        const { data } = await api.post("/auth/refresh", {
          refreshToken: oldRefreshToken,
        })
        // Save new tokens
        localStorage.setItem("shoppee_access", data.accessToken)
        localStorage.setItem("shoppee_refresh", data.refreshToken)
        // Retry the original request with new token
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`
        return api(originalRequest)
      } catch (refreshError) {
        // Refresh token is also expired or invalid -> Force Logout
        // localStorage.clear()
        clearAuthCredentials()
        window.location.href = "/login"
        return Promise.reject(refreshError)
      }
    }
    return Promise.reject(error)
  }
)

export default api
