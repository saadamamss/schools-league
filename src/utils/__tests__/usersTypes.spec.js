import { beforeEach, describe, expect, it, vi } from 'vitest'
import { userHasRoutePermission, userTypes } from '@/utils/usersTypes'

vi.mock('@/services/auth.service', () => ({
  authService: {
    getStoredUser: vi.fn(),
  },
}))

const { authService } = await import('@/services/auth.service')

describe('userHasRoutePermission', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('allows known role to access its permitted routes', () => {
    authService.getStoredUser.mockReturnValue({
      user: { userType: { code: 'organizer' } },
    })
    expect(userHasRoutePermission('home')).toBe(true)
    expect(userHasRoutePermission('profile')).toBe(true)
  })

  it('denies known role from accessing restricted routes', () => {
    authService.getStoredUser.mockReturnValue({
      user: { userType: { code: 'organizer' } },
    })
    expect(userHasRoutePermission('users')).toBe(false)
    expect(userHasRoutePermission('attendances')).toBe(false)
  })

  it('returns false for unknown role codes', () => {
    authService.getStoredUser.mockReturnValue({
      user: { userType: { code: 'hacker' } },
    })
    expect(userHasRoutePermission('home')).toBe(false)
  })

  it('allows access when no user is stored (authenticated but no stored data = unrestricted)', () => {
    authService.getStoredUser.mockReturnValue(null)
    expect(userHasRoutePermission('home')).toBe(true)
  })

  it('allows access when user has no userType (admin/superuser = unrestricted)', () => {
    authService.getStoredUser.mockReturnValue({ user: {} })
    expect(userHasRoutePermission('home')).toBe(true)
  })

  it('handles snake_case user_type.code', () => {
    authService.getStoredUser.mockReturnValue({
      user: { user_type: { code: 'organizer' } },
    })
    expect(userHasRoutePermission('home')).toBe(true)
  })

  it('handles stored user directly (not nested in .user)', () => {
    authService.getStoredUser.mockReturnValue({
      userType: { code: 'organizer' },
    })
    expect(userHasRoutePermission('home')).toBe(true)
  })

  it('allows all routes listed in userTypes', () => {
    for (const [role, routes] of Object.entries(userTypes)) {
      authService.getStoredUser.mockReturnValue({
        user: { userType: { code: role } },
      })
      for (const route of routes) {
        expect(userHasRoutePermission(route)).toBe(true)
      }
    }
  })
})
