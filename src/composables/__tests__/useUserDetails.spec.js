import { beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('@/api/users.api', () => ({
  usersApi: {
    getById: vi.fn(),
  },
}))

vi.mock('@/stores/app', () => ({
  useAppStore: vi.fn(() => ({
    showSnackbar: vi.fn(),
  })),
}))

describe('useUserDetails', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('initialises with default state', async () => {
    const { useUserDetails } = await import('@/composables/useUserDetails')
    const result = useUserDetails()

    expect(result.detailsDialog.value).toBe(false)
    expect(result.selectedUser.value).toBeNull()
    expect(result.isUserDetailsLoading.value).toBe(false)
  })

  it('opens dialog and fetches user data on showDetails', async () => {
    const { usersApi } = await import('@/api/users.api')
    usersApi.getById.mockResolvedValue({
      data: { data: { id: 1, name: 'Test User', email: 'test@test.com' } },
    })

    const { useUserDetails } = await import('@/composables/useUserDetails')
    const result = useUserDetails()

    result.showDetails({ id: 1, name: 'Test User' })

    expect(result.detailsDialog.value).toBe(true)
    expect(result.selectedUser.value).toEqual({ id: 1, name: 'Test User' })
    expect(usersApi.getById).toHaveBeenCalledWith(1, expect.objectContaining({ signal: expect.any(AbortSignal) }))

    // Wait for the async fetch to complete
    await vi.waitFor(() => {
      expect(result.selectedUser.value).toEqual({ id: 1, name: 'Test User', email: 'test@test.com' })
    })
    expect(result.isUserDetailsLoading.value).toBe(false)
  })

  it('sets loading state while fetching', async () => {
    const { usersApi } = await import('@/api/users.api')
    let resolvePromise
    usersApi.getById.mockReturnValue(new Promise(resolve => { resolvePromise = resolve }))

    const { useUserDetails } = await import('@/composables/useUserDetails')
    const result = useUserDetails()

    result.showDetails({ id: 1 })

    expect(result.isUserDetailsLoading.value).toBe(true)

    resolvePromise({ data: { data: { id: 1 } } })
    await vi.waitFor(() => {
      expect(result.isUserDetailsLoading.value).toBe(false)
    })
  })

  it('shows snackbar on fetch error', async () => {
    const { usersApi } = await import('@/api/users.api')
    usersApi.getById.mockRejectedValue(new Error('Network error'))

    const { useAppStore } = await import('@/stores/app')
    const mockShowSnackbar = vi.fn()
    useAppStore.mockReturnValue({ showSnackbar: mockShowSnackbar })

    const { useUserDetails } = await import('@/composables/useUserDetails')
    const result = useUserDetails()

    result.showDetails({ id: 1 })

    await vi.waitFor(() => {
      expect(mockShowSnackbar).toHaveBeenCalled()
    })
    expect(result.isUserDetailsLoading.value).toBe(false)
  })

  it('handles mergeLocation option', async () => {
    const { usersApi } = await import('@/api/users.api')
    const userData = { id: 1, name: 'Test', locations: [{ id: 10, name: 'Cairo' }] }
    usersApi.getById.mockResolvedValue({
      data: { data: userData },
    })

    const { useUserDetails } = await import('@/composables/useUserDetails')
    const result = useUserDetails({ mergeLocation: true })

    result.showDetails({ id: 1, name: 'Test' })

    await vi.waitFor(() => {
      expect(result.selectedUser.value).toEqual(
        expect.objectContaining({ id: 1, name: 'Test', locations: [{ id: 10, name: 'Cairo' }] }),
      )
    })
  })
})
