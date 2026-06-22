import axios from 'axios'
import { API_BASE_URL, API_ENDPOINTS } from '@/config'
import type { User } from '@/types/models'

interface StoredUserData extends Partial<User> {
  role?: { id: number; name?: string }
  parent_companies?: unknown[]
  userType?: { code: string }
}

interface StoredUser extends StoredUserData {
  token?: string
  refreshToken?: string
  user?: StoredUserData
}

class AuthService {
  // In-memory token storage — NOT localStorage (prevents XSS token theft)
  private _token: string | null = null
  private _refreshToken: string | null = null
  private _user: StoredUserData | null = null

  /**
   * Initialize by attempting a token refresh via HttpOnly cookie.
   * Uses raw axios to bypass the 401 redirect interceptor — prevents
   * infinite reload loop when the refresh cookie is missing/expired.
   */
  async initialize (): Promise<boolean> {
    try {
      const response = await axios.post(
        API_BASE_URL + API_ENDPOINTS.REFRESH_TOKEN,
        this._refreshToken ? { token: this._refreshToken } : {},
        { withCredentials: true },
      )
      const newToken = response?.data?.token || response?.data?.data?.token
      const newRefreshToken = response?.data?.refreshToken || response?.data?.data?.refreshToken
      const user = response?.data?.user || response?.data?.data?.user

      if (newToken) {
        this._token = newToken
        if (newRefreshToken) this._refreshToken = newRefreshToken
        if (user) this._user = user
        return true
      }
    } catch {
      // No valid refresh cookie — user needs to log in
    }
    return false
  }

  getToken (): string | null {
    return this._token
  }

  getRefreshToken (): string | null {
    return this._refreshToken
  }

  getStoredUser (): StoredUser | null {
    if (!this._token && !this._user) return null
    return {
      ...(this._user || {}),
      token: this._token || undefined,
      refreshToken: this._refreshToken || undefined,
    } as StoredUser
  }

  getUserData (): StoredUserData | null {
    return this._user
  }

  setUser (user: StoredUser | null): void {
    if (user) {
      this._token = user.token ?? this._token
      this._refreshToken = user.refreshToken ?? this._refreshToken
      this._user = { ...this._user, ...user.user, ...user }
      // Remove token/refreshToken from the user object to avoid confusion
      delete (this._user as any).token
      delete (this._user as any).refreshToken
      delete (this._user as any).user
    } else {
      this._token = null
      this._refreshToken = null
      this._user = null
    }
  }

  setToken (token: string, refreshToken?: string): void {
    this._token = token
    if (refreshToken) this._refreshToken = refreshToken
  }

  logout (): void {
    this._token = null
    this._refreshToken = null
    this._user = null
  }

  isAuthenticated (): boolean {
    return !!this._token
  }
}

export const authService = new AuthService()
