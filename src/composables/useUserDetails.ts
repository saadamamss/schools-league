import { onUnmounted, ref } from 'vue'
import { usersApi } from '@/api'
import { useApiError } from '@/composables/useApiError'
import type { User, UserWithLocation } from '@/types/models'

interface UseUserDetailsOptions {
  mergeLocation?: boolean
}

export function useUserDetails (options: UseUserDetailsOptions = {}) {
  const { mergeLocation = false } = options

  const { handleError } = useApiError('useUserDetails')
  let abortController: AbortController | null = null
  const detailsDialog = ref(false)
  const selectedUser = ref<UserWithLocation | null>(null)
  const isUserDetailsLoading = ref(false)

  onUnmounted(() => {
    if (abortController) {
      abortController.abort()
    }
  })

  const fetchSelectedUserData = async (userId: number) => {
    if (abortController) {
      abortController.abort()
    }
    abortController = new AbortController()
    isUserDetailsLoading.value = true
    try {
      const response = await usersApi.getById(userId, { signal: abortController.signal })
      if (response.data?.data) {
        if (mergeLocation) {
          const resData = response.data as { data: UserWithLocation; location?: UserWithLocation['locations'] }
          selectedUser.value = {
            ...resData.data,
            locations: (resData as any).locations ?? resData.data.locations,
          } as UserWithLocation
        } else {
          selectedUser.value = response.data?.data as UserWithLocation
        }
      }
    } catch (err: unknown) {
      if ((err as { name?: string })?.name === 'CanceledError') return
      handleError(err, { fallback: 'حدث خطأ فى جلب بيانات هذا المستخدم' })
    } finally {
      isUserDetailsLoading.value = false
    }
  }

  const showDetails = (user: User) => {
    detailsDialog.value = true
    selectedUser.value = user as UserWithLocation
    fetchSelectedUserData(user.id)
  }

  return {
    detailsDialog,
    selectedUser,
    isUserDetailsLoading,
    fetchSelectedUserData,
    showDetails,
  }
}
