import api from "@/lib/api"
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react"

interface User {
  id: string
  username: string
  email: string
  fisrtName?: string
  lastName?: string
  roles: string[]
}

interface LoginResponse {
  accessToken: string
  refreshToken: string
}

interface AuthContextType {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  isLoading: boolean
  login: (tokens: LoginResponse) => Promise<boolean>
  logout: () => Promise<void>
  refreshUser: () => Promise<void>
}

export const clearAuthCredentials = () => {
  localStorage.removeItem("shoppee_access")
  localStorage.removeItem("shoppee_refresh")
  localStorage.removeItem("shoppee_user")
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null)
  const [accessToken, setAccessToken] = useState<string | null>(null)
  const [refreshToken, setRefreshToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  // Fetch User Identity (The "Second Hop")
  // This uses the bearer token to get details for the UI.
  const fetchUserInfo = useCallback(
    async (token?: string): Promise<User | null> => {
      try {
        // If a token is passed (during login), use it directly.
        // Otherwise, the axios interceptor will handle it.
        const config = token
          ? { headers: { Authorization: `Bearer ${token}` } }
          : {}
        const { data } = await api.get<User>("/auth/info", config)
        return data
      } catch (error) {
        console.error("Identity fetch failed:", error)
        return null
      }
    },
    []
  )

  // Login Logic
  // Handles saving tokens and immediately fetching the user profile.
  // Wrap login in useCallback
  const login = useCallback(
    async (tokens: LoginResponse): Promise<boolean> => {
      setAccessToken(tokens.accessToken)
      setRefreshToken(tokens.refreshToken)
      localStorage.setItem("shoppee_access", tokens.accessToken)
      localStorage.setItem("shoppee_refresh", tokens.refreshToken)

      const userData = await fetchUserInfo(tokens.accessToken)

      // If user is admin
      if (userData && userData.roles.includes("Admin")) {
        setUser(userData)
        localStorage.setItem("shoppee_user", JSON.stringify(userData))
        return true
      } else {
        // If user is not admin
        await logout()
        return false
      }
    },
    [fetchUserInfo]
  )

  // Secure Logout
  // Notifies backend to invalidate the refresh token before clearing local state.
  const logout = useCallback(async () => {
    try {
      const currentRefresh =
        refreshToken || localStorage.getItem("shoppee_refresh")

      if (currentRefresh) {
        // Invalidate session on server
        await api.post("/auth/logout", { refreshToken: currentRefresh })
      }
    } catch (error) {
      console.warn(
        "Server-side logout failed, clearing local session anyway.",
        error
      )
    } finally {
      setUser(null)
      setAccessToken(null)
      setRefreshToken(null)
      clearAuthCredentials()
    }
  }, [refreshToken])

  //  Manual UI Refresh
  // Wrap refreshUser in useCallback
  const refreshUser = useCallback(async () => {
    const data = await fetchUserInfo()
    if (data) setUser(data)
  }, [fetchUserInfo])

  // Hydration (Page Load)
  // Restores session from localStorage on refresh.
  useEffect(() => {
    const initAuth = async () => {
      const savedAccess = localStorage.getItem("shoppee_access")
      const savedRefresh = localStorage.getItem("shoppee_refresh")
      const savedUser = localStorage.getItem("shoppee_user")

      if (savedAccess && savedRefresh) {
        setAccessToken(savedAccess)
        setRefreshToken(savedRefresh)

        if (savedUser) {
          setUser(JSON.parse(savedUser))
        } else {
          const data = await fetchUserInfo(savedAccess)
          if (data) {
            setUser(data)
            localStorage.setItem("shoppee_user", JSON.stringify(data))
          } else {
            // If tokens exist but identity check fails, session is likely dead
            // Manual clear instead of calling logout() to avoid dependency loops
            clearAuthCredentials()
            setAccessToken(null)
            setRefreshToken(null)
          }
        }
      }
      setIsLoading(false)
    }

    initAuth()
    // SYNC LOGOUT ACROSS TABS
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === "shoppee_access" && !e.newValue) {
        setUser(null)
        setAccessToken(null)
      }
    }
    window.addEventListener("storage", handleStorageChange)
    return () => window.removeEventListener("storage", handleStorageChange)
  }, [])

  const value = {
    user,
    accessToken,
    refreshToken,
    isAuthenticated: !!accessToken,
    isLoading,
    login,
    logout,
    refreshUser,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
