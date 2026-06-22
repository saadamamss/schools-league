import { beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('@/services/auth.service', () => ({
  authService: {
    isAuthenticated: vi.fn(),
    logout: vi.fn(),
  },
}))

vi.mock('@/utils/usersTypes', () => ({
  userHasRoutePermission: vi.fn(() => true),
}))

vi.mock('@/router/routes', () => ({
  default: [],
}))

function makeRoute (overrides = {}) {
  return {
    path: '/',
    fullPath: '/',
    name: undefined,
    meta: {},
    params: {},
    query: {},
    hash: '',
    redirectedFrom: undefined,
    matched: [],
    ...overrides,
  }
}

describe('authGuard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('redirects unauthenticated user to /auth/login for protected routes', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(false)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/users', meta: { requiresAuth: true } })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith('/auth/login')
  })

  it('allows authenticated user with permission to access route', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(true)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/users', name: 'users', meta: { requiresAuth: true } })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith()
  })

  it('redirects authenticated user without permission to home', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(true)

    const { userHasRoutePermission } = await import('@/utils/usersTypes')
    userHasRoutePermission.mockReturnValue(false)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/users', name: 'users', meta: { requiresAuth: true } })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith('/')
  })

  it('redirects authenticated guest user to home', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(true)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/auth/login', name: 'login', meta: { guest: true } })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith('/')
  })

  it('allows unauthenticated guest user to access /auth/login', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(false)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/auth/login', name: 'login', meta: { guest: true } })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith()
  })

  it('logs out and redirects to /auth/login when user lacks home permission', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(true)

    const { userHasRoutePermission } = await import('@/utils/usersTypes')
    userHasRoutePermission.mockReturnValue(false)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/', name: 'home', meta: { requiresAuth: true } })

    authGuard(to, makeRoute(), next)

    expect(authService.logout).toHaveBeenCalled()
    expect(next).toHaveBeenCalledWith('/auth/login')
  })

  it('returns early for routes without meta', async () => {
    const { authService } = await import('@/services/auth.service')
    authService.isAuthenticated.mockReturnValue(false)

    const { authGuard } = await import('@/router/index')
    const next = vi.fn()
    const to = makeRoute({ path: '/about', name: 'about' })

    authGuard(to, makeRoute(), next)

    expect(next).toHaveBeenCalledWith()
  })
})
