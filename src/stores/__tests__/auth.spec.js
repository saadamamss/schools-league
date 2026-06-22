import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'

const mockUser = { id: 1, full_name: 'Test', email: 'test@test.com' }

vi.mock('@/services/auth.service', () => ({
  authService: {
    getToken: vi.fn(() => null),
    getStoredUser: vi.fn(() => null),
    getUserData: vi.fn(() => null),
    setToken: vi.fn(),
    setUser: vi.fn(),
    logout: vi.fn(),
    initialize: vi.fn(() => Promise.resolve(false)),
  },
}))

vi.mock('@/plugins/axios', () => ({
  default: {
    post: vi.fn(),
    get: vi.fn(),
  },
}))

vi.mock('js-cookie', () => ({
  default: {
    set: vi.fn(),
    get: vi.fn(),
  },
}))

const { authService } = await import('@/services/auth.service')

describe('auth store', () => {
  let store

  beforeEach(async () => {
    vi.clearAllMocks()
    setActivePinia(createPinia())
    const { useAuthStore } = await import('@/stores/auth')
    store = useAuthStore()
  })

  it('starts with no token and no user', () => {
    expect(store.token).toBeFalsy()
    expect(store.user).toBeFalsy()
  })

  it('setToken stores token and syncs to authService', () => {
    store.setToken('test-token')
    expect(store.token).toBe('test-token')
    expect(authService.setToken).toHaveBeenCalledWith('test-token', undefined)
  })

  it('login succeeds and sets token + user', async () => {
    const mockResponse = {
      data: {
        token: 'login-token',
        user: mockUser,
      },
    }
    const axiosIns = (await import('@/plugins/axios')).default
    axiosIns.post.mockResolvedValue(mockResponse)

    const result = await store.login({ email: 'test@test.com', password: 'pass' })
    expect(result.token).toBe('login-token')
    expect(result.user.id).toBe(1)
    expect(store.token).toBe('login-token')
    expect(store.user).toEqual(mockUser)
  })

  it('login throws when no data returned', async () => {
    const axiosIns = (await import('@/plugins/axios')).default
    axiosIns.post.mockResolvedValue({ data: null })
    await expect(store.login({ email: 'test@test.com', password: 'pass' })).rejects.toThrow('No data received from server')
  })

  it('login throws when no token returned', async () => {
    const axiosIns = (await import('@/plugins/axios')).default
    axiosIns.post.mockResolvedValue({ data: { user: mockUser } })
    await expect(store.login({ email: 'test@test.com', password: 'pass' })).rejects.toThrow('No token received from server')
  })

  it('logout calls API and resets state', async () => {
    const axiosIns = (await import('@/plugins/axios')).default
    store.token = 'test-token'
    store.user = mockUser
    axiosIns.post.mockResolvedValue({})
    await store.logout()
    expect(axiosIns.post).toHaveBeenCalledWith('auth/logout')
    expect(store.token).toBeNull()
    expect(store.user).toBeNull()
  })

  it('isAuthenticated returns true when token exists', () => {
    store.token = 'valid-token'
    expect(store.isAuthenticated).toBe(true)
  })

  it('isAuthenticated returns false when no token', () => {
    store.token = null
    expect(store.isAuthenticated).toBe(false)
  })
})
