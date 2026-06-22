import { beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('@/services/auth.service', () => ({
  authService: {
    getToken: vi.fn(),
    getRefreshToken: vi.fn(),
    getStoredUser: vi.fn(),
    setUser: vi.fn(),
    logout: vi.fn(),
  },
}))

vi.mock('@/stores/app', () => ({
  useAppStore: vi.fn(() => ({
    showSnackbar: vi.fn(),
  })),
}))

vi.mock('axios', async importOriginal => {
  const actual = await importOriginal()
  return {
    ...actual,
    default: {
      ...actual.default,
      post: vi.fn(),
      create: actual.default.create,
    },
    post: vi.fn(),
  }
})

describe('axios interceptor', () => {
  let mockShowSnackbar

  beforeEach(async () => {
    vi.clearAllMocks()
    vi.resetModules()

    const { useAppStore } = await import('@/stores/app')
    mockShowSnackbar = vi.fn()
    useAppStore.mockReturnValue({ showSnackbar: mockShowSnackbar })
  })

  describe('requestInterceptor', () => {
    it('adds Bearer token to config headers when token exists', async () => {
      const { authService } = await import('@/services/auth.service')
      authService.getToken.mockReturnValue('test-token')

      const { requestInterceptor } = await import('@/plugins/axios')
      const config = { headers: {} }

      const result = requestInterceptor(config)

      expect(result.headers.Authorization).toBe('Bearer test-token')
    })

    it('does not add Authorization header when no token', async () => {
      const { authService } = await import('@/services/auth.service')
      authService.getToken.mockReturnValue(null)

      const { requestInterceptor } = await import('@/plugins/axios')
      const config = { headers: {} }

      const result = requestInterceptor(config)

      expect(result.headers.Authorization).toBeUndefined()
    })

    it('preserves existing headers', async () => {
      const { authService } = await import('@/services/auth.service')
      authService.getToken.mockReturnValue('tok')

      const { requestInterceptor } = await import('@/plugins/axios')
      const config = { headers: { 'Content-Type': 'application/json' } }

      const result = requestInterceptor(config)

      expect(result.headers.Authorization).toBe('Bearer tok')
      expect(result.headers['Content-Type']).toBe('application/json')
    })
  })

  describe('handle422Errors', () => {
    it('shows snackbar for each validation error field', async () => {
      const { handle422Errors } = await import('@/plugins/axios')
      const error = {
        response: {
          status: 422,
          data: {
            errors: {
              email: ['Email is required'],
              password: ['Password too short'],
            },
          },
        },
        config: {},
      }

      await expect(handle422Errors(error)).rejects.toBe(error)

      expect(mockShowSnackbar).toHaveBeenCalledTimes(2)
      expect(mockShowSnackbar).toHaveBeenCalledWith({
        message: 'Email is required',
        color: 'error',
        timeout: 5000,
      })
      expect(mockShowSnackbar).toHaveBeenCalledWith({
        message: 'Password too short',
        color: 'error',
        timeout: 5000,
      })
    })

    it('rejects other errors without snackbar', async () => {
      const { handle422Errors } = await import('@/plugins/axios')
      const error = { response: { status: 500, data: {} }, config: {} }

      await expect(handle422Errors(error)).rejects.toBe(error)
      expect(mockShowSnackbar).not.toHaveBeenCalled()
    })

    it('rejects non-http errors without snackbar', async () => {
      const { handle422Errors } = await import('@/plugins/axios')
      const error = new Error('Network Error')

      await expect(handle422Errors(error)).rejects.toBe(error)
      expect(mockShowSnackbar).not.toHaveBeenCalled()
    })
  })

  describe('responseErrorHandler — 401 token refresh', () => {
    beforeEach(async () => {
      const { authService } = await import('@/services/auth.service')
      authService.getToken.mockReturnValue('expired-token')
      authService.getRefreshToken.mockReturnValue('valid-refresh')
      authService.getStoredUser.mockReturnValue({ name: 'Test User' })
    })

    it('refreshes token and retries the request', async () => {
      const { authService } = await import('@/services/auth.service')
      const axios = await import('axios')
      axios.default.post.mockResolvedValue({
        data: { token: 'new-token', refreshToken: 'new-refresh' },
      })

      const { default: axiosIns, responseErrorHandler } = await import('@/plugins/axios')
      axiosIns.defaults.adapter = vi.fn().mockResolvedValue({ data: 'retried' })

      const originalRequest = { url: '/test', headers: {} }
      const error = { response: { status: 401 }, config: originalRequest }

      const result = await responseErrorHandler(error)

      expect(result.data).toBe('retried')
      expect(authService.setUser).toHaveBeenCalledWith(
        expect.objectContaining({ token: 'new-token', refreshToken: 'new-refresh' }),
      )
    })

    it('redirects to login when refresh fails', async () => {
      const axios = await import('axios')
      axios.default.post.mockRejectedValue(new Error('Refresh failed'))

      delete window.location
      window.location = { href: '' }

      const { authService } = await import('@/services/auth.service')
      const { responseErrorHandler } = await import('@/plugins/axios')
      const originalRequest = { url: '/test', headers: {} }
      const error = { response: { status: 401 }, config: originalRequest }

      await expect(responseErrorHandler(error)).rejects.toThrow('Refresh failed')

      expect(authService.logout).toHaveBeenCalled()
      expect(window.location.href).toBe('/auth/login')
    })
  })
})
