import { computed, type ComputedRef, type Ref, ref, watch } from 'vue'
import { useDebounceFn } from '@vueuse/core'
import axiosIns from '@/plugins/axios'
import { useAppStore } from '@/stores/app'
import type { ApiResponse } from '@/types/models'

interface UsePaginatedFetchOptions {
  debounceMs?: number
  defaultPerPage?: number
  fetchOnMount?: boolean
  extractStatistics?: boolean
}

interface UsePaginatedFetchReturn<T> {
  data: Ref<T[]>
  isLoading: Ref<boolean>
  initialLoading: Ref<boolean>
  perPage: Ref<number>
  totalItems: Ref<number>
  currentPage: Ref<number>
  search: Ref<string>
  currentFilters: Ref<Record<string, unknown>>
  widgetsData: Ref<Record<string, number> | null>
  searchFilter: ComputedRef<Record<string, string>>
  fetchData: (extraParams?: Record<string, unknown>) => Promise<void>
  handleSearch: () => void
  applyFilters: (filters: Record<string, unknown>) => void
  clearFilters: () => void
}

export function usePaginatedFetch<T = any> (
  endpoint: string,
  options: UsePaginatedFetchOptions = {}
): UsePaginatedFetchReturn<T> {
  const {
    debounceMs = 500,
    defaultPerPage = 5,
    fetchOnMount = true,
    extractStatistics = false,
  } = options

  const appStore = useAppStore()
  const data = ref<T[]>([]) as Ref<T[]>
  const isLoading = ref(true)
  const initialLoading = ref(true)
  const perPage = ref(defaultPerPage)
  const totalItems = ref(0)
  const currentPage = ref(1)
  const search = ref('')
  const currentFilters = ref<Record<string, unknown>>({})
  const widgetsData = ref<Record<string, number> | null>(null)

  const searchFilter = computed((): Record<string, string> =>
    search.value?.trim() ? { search: search.value.trim() } : {}
  )

  const fetchData = async (extraParams: Record<string, unknown> = {}) => {
    isLoading.value = true
    try {
      const response = await axiosIns.get(endpoint, {
        params: {
          per_page: perPage.value,
          page: currentPage.value,
          ...searchFilter.value,
          ...currentFilters.value,
          ...extraParams,
        },
      })
      const responseData = response.data as ApiResponse<T[]>
      if (responseData?.data) {
        data.value = responseData.data
        perPage.value = responseData?.pagination?.i_per_page ?? perPage.value
        totalItems.value = responseData?.pagination?.i_total_objects ?? 0
        currentPage.value = responseData?.pagination?.i_current_page ?? 1
        if (extractStatistics) {
          widgetsData.value = responseData?.statistics ?? null
        }
      }
    } catch (err: any) {
      console.error('Error fetching data:', err)
      appStore.showSnackbar({
        message: err.response?.data?.message || 'حدث خطأ فى جلب بيانات الجدول',
        color: 'error',
      })
    } finally {
      isLoading.value = false
      initialLoading.value = false
    }
  }

  watch(currentPage, () => fetchData())

  watch(perPage, () => {
    currentPage.value = 1
    fetchData()
  })

  const handleSearch = useDebounceFn(() => fetchData(), debounceMs)

  const applyFilters = (filters: Record<string, unknown>) => {
    currentPage.value = 1
    currentFilters.value = filters
    fetchData()
  }

  const clearFilters = () => {
    currentPage.value = 1
    currentFilters.value = {}
    fetchData()
  }

  if (fetchOnMount) {
    fetchData()
  }

  return {
    data,
    isLoading,
    initialLoading,
    perPage,
    totalItems,
    currentPage,
    search,
    currentFilters,
    widgetsData,
    searchFilter,
    fetchData,
    handleSearch,
    applyFilters,
    clearFilters,
  }
}
