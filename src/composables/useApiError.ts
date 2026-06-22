import { useAppStore } from '@/stores/app'

interface HandleErrorOptions {
  fallback?: string
  timeout?: number
  log?: boolean
}

export function useApiError (_source?: string) {
  const appStore = useAppStore()

  function getErrorMessage (error: unknown): string {
    if (error && typeof error === 'object') {
      const err = error as { response?: { data?: { status?: { message?: string }; message?: string } }; message?: string }
      return err.response?.data?.status?.message ||
        err.response?.data?.message ||
        err.message ||
        'حدث خطأ غير متوقع'
    }
    if (error instanceof Error) return error.message
    return 'حدث خطأ غير متوقع'
  }

  function handleError (error: unknown, options: HandleErrorOptions = {}) {
    const {
      fallback = 'حدث خطأ غير متوقع',
      timeout = 5000,
      log = true,
    } = options

    if (log) {
      // Error logging disabled in production
    }

    const err = error as { response?: { data?: { status?: { message?: string }; message?: string } }; message?: string }

    appStore.showSnackbar({
      message: err.response?.data?.status?.message ||
        err.response?.data?.message ||
        err.message ||
        fallback,
      color: 'error',
      timeout,
    })
  }

  return { handleError, getErrorMessage }
}
