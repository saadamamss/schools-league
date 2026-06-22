import { defineStore } from 'pinia'
import Cookies from 'js-cookie'
import { authService } from '@/services/auth.service'
import { authApi } from '@/api'
import type { LoginResponse, User } from '@/types/models'

interface AuthState {
  token: string | null
  user: User | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: authService.getToken(),
    user: authService.getUserData() as User ?? null,
  }),

  getters: {
    isAuthenticated: state => !!state.token,
  },

  actions: {
    setToken (token: string, refreshToken?: string) {
      this.token = token
      authService.setToken(token, refreshToken)
    },

    setUser (user: User) {
      this.user = user
      const stored = authService.getStoredUser() || {}
      authService.setUser({ ...stored, user })
    },

    async logout () {
      try {
        await authApi.logout()
      } catch {
        // Even if server logout fails, clear local state
      } finally {
        this.$reset()
        authService.logout()
      }
    },

    async login (credentials: { email: string; password: string }): Promise<LoginResponse> {
      const response = await authApi.login(credentials)
      const data = response?.data

      if (!data) {
        throw new Error('No data received from server')
      }

      const token = data.data?.token ?? data.token
      const refreshToken = data.data?.refreshToken ?? data.refreshToken
      const user = data.data?.user ?? data.user

      if (!token) {
        throw new Error('No token received from server')
      }

      this.setToken(token, refreshToken)
      this.setUser(user)

      return { token, user }
    },

    async register (registData: Record<string, unknown>) {
      const response = await authApi.register(registData)
      const data = response?.data

      if (data?.status?.success) {
        const expiresInMs = 300 * 1000
        Cookies.set('verif-mail', registData.email as string, {
          expires: 300 / 86400,
        })
        localStorage.setItem('verif-mail-expiry', String(Date.now() + expiresInMs))
      }
      return data
    },

    resetPassword (email: string) {
      return authApi.forgotPassword(email)
    },

    verifyOTP (data: { email: string; otp: string }) {
      return authApi.verifyOTP(data)
    },

    async verifyAccount (verData: { email: string; otp: string }): Promise<LoginResponse> {
      const response = await authApi.verifyRegistration(verData)

      const data = response?.data

      if (!data) {
        throw new Error('No data received from server')
      }

      const token = data.data?.token ?? data.token
      const refreshToken = data.data?.refreshToken ?? data.refreshToken
      const user = data.data?.user ?? data.user

      if (!token) {
        throw new Error('No token received from server')
      }

      this.setToken(token, refreshToken)
      this.setUser(user)

      return { token, user }
    },

    updatePassword (data: Record<string, unknown>) {
      return authApi.resetPassword(data)
    },

    resendVerificationOTP (data: string) {
      return authApi.resendRegistrationOTP(data)
    },
  },
})
