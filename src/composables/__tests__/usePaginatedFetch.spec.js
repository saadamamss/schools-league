import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('@/plugins/axios', () => ({
  default: {
    get: vi.fn(),
  },
}))

vi.mock('@/stores/app', () => ({
  useAppStore: vi.fn(() => ({
    showSnackbar: vi.fn(),
  })),
}))

const axiosIns = (await import('@/plugins/axios')).default

describe('usePaginatedFetch', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('fetches data on mount and sets response', async () => {
    const mockData = [{ id: 1, name: 'Item 1' }]
    axiosIns.get.mockResolvedValue({
      data: {
        data: mockData,
        pagination: { i_per_page: 5, i_total_objects: 1, i_current_page: 1 },
      },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint')

    // fetchData is called inside onMounted (automatically on mount)
    // In test context without component mount, trigger it manually
    await result.fetchData()
    await vi.runAllTimersAsync()
    expect(axiosIns.get).toHaveBeenCalledWith('test-endpoint', expect.any(Object))
  })

  it('handles empty pagination gracefully', async () => {
    axiosIns.get.mockResolvedValue({
      data: { data: [] },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    await result.fetchData()
    await vi.runAllTimersAsync()

    expect(result.data.value).toEqual([])
    expect(result.totalItems.value).toBe(0)
  })

  it('includes search params when search is set', async () => {
    axiosIns.get.mockResolvedValue({
      data: {
        data: [],
        pagination: { i_per_page: 5, i_total_objects: 0, i_current_page: 1 },
      },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    result.search.value = 'test query'
    result.handleSearch()
    await vi.runAllTimersAsync()

    expect(axiosIns.get).toHaveBeenCalledWith(
      'test-endpoint',
      expect.objectContaining({
        params: expect.objectContaining({ search: 'test query' }),
      })
    )
  })

  it('resets to page 1 when perPage changes', async () => {
    axiosIns.get.mockResolvedValue({
      data: { data: [], pagination: { i_per_page: 5, i_total_objects: 0, i_current_page: 1 } },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    result.currentPage.value = 3
    result.perPage.value = 10
    await vi.runAllTimersAsync()

    expect(result.currentPage.value).toBe(1)
  })

  it('applies filters and resets page', async () => {
    axiosIns.get.mockResolvedValue({
      data: { data: [], pagination: { i_per_page: 5, i_total_objects: 0, i_current_page: 1 } },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    result.applyFilters({ status: 'active' })
    await vi.runAllTimersAsync()

    expect(result.currentPage.value).toBe(1)
    expect(result.currentFilters.value).toEqual({ status: 'active' })
  })

  it('clears filters and resets page', async () => {
    axiosIns.get.mockResolvedValue({
      data: { data: [], pagination: { i_per_page: 5, i_total_objects: 0, i_current_page: 1 } },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    result.currentFilters.value = { status: 'active' }
    result.clearFilters()
    await vi.runAllTimersAsync()

    expect(result.currentPage.value).toBe(1)
    expect(result.currentFilters.value).toEqual({})
  })

  it('sets error state when request fails', async () => {
    axiosIns.get.mockRejectedValue(new Error('Network error'))

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false })

    await result.fetchData()
    await vi.runAllTimersAsync()

    expect(result.fetchError.value).toBe(true)
    expect(result.isLoading.value).toBe(false)
  })

  it('extracts statistics when extractStatistics is true', async () => {
    axiosIns.get.mockResolvedValue({
      data: {
        data: [],
        pagination: { i_per_page: 5, i_total_objects: 0, i_current_page: 1 },
        statistics: { total: 100, active: 50 },
      },
    })

    const { usePaginatedFetch } = await import('@/composables/usePaginatedFetch')
    const result = usePaginatedFetch('test-endpoint', { fetchOnMount: false, extractStatistics: true })

    await result.fetchData()
    await vi.runAllTimersAsync()

    expect(result.widgetsData.value).toEqual({ total: 100, active: 50 })
  })
})
