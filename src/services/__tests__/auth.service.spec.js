import { beforeEach, describe, expect, it, vi } from 'vitest'
import { authService } from '@/services/auth.service'

describe('AuthService', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    authService.logout() // Reset in-memory state
  })

  describe('getStoredUser', () => {
    it('returns null when no user stored', () => {
      authService.logout()
      expect(authService.getStoredUser()).toBeNull()
    })

    it('returns stored user data', () => {
      const user = { token: 'abc', name: 'Test User', user: { id: 1, full_name: 'Test' } }
      authService.setUser(user)
      expect(authService.getStoredUser()?.token).toBe('abc')
    })
  })

  describe('getToken', () => {
    it('returns token from stored user', () => {
      authService.setToken('my-token')
      expect(authService.getToken()).toBe('my-token')
    })

    it('returns null when no token stored', () => {
      expect(authService.getToken()).toBeNull()
    })
  })

  describe('getRefreshToken', () => {
    it('returns refreshToken from stored user', () => {
      authService.setToken('tok', 'rt-123')
      expect(authService.getRefreshToken()).toBe('rt-123')
    })

    it('returns null when no user stored', () => {
      expect(authService.getRefreshToken()).toBeNull()
    })
  })

  describe('setUser', () => {
    it('stores user in memory', () => {
      const user = { token: 'tok', name: 'Test' }
      authService.setUser(user)
      expect(authService.getStoredUser()?.token).toBe('tok')
    })

    it('removes user from memory when null', () => {
      authService.setUser({ token: 'tok' })
      authService.setUser(null)
      expect(authService.getStoredUser()).toBeNull()
    })
  })

  describe('logout', () => {
    it('clears in-memory state', () => {
      authService.setToken('tok', 'rt')
      authService.logout()
      expect(authService.getToken()).toBeNull()
      expect(authService.getRefreshToken()).toBeNull()
      expect(authService.isAuthenticated()).toBe(false)
    })
  })

  describe('isAuthenticated', () => {
    it('returns true when token exists', () => {
      authService.setToken('tok')
      expect(authService.isAuthenticated()).toBe(true)
    })

    it('returns false when no token', () => {
      expect(authService.isAuthenticated()).toBe(false)
    })
  })
})
