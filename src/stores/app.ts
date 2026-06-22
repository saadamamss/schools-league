import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { SnackbarState } from '@/types/models'

export const useAppStore = defineStore('app', () => {
  const appBreadcrumb = ref<Array<{ label: string; to: string }>>()
  const appPageTitle = ref('')

  const snackbar = ref<SnackbarState>({
    show: false,
    message: '',
    color: 'success',
    timeout: 3000,
  })

  const setAppBreadcrumb = (breadcrumb: Array<{ label: string; to: string }> = [], isBaseRoute = false) => {
    const baseHomeRoute = [{ label: 'الرئيسية', to: '/' }]

    if (isBaseRoute) {
      appBreadcrumb.value = [...baseHomeRoute]
      return
    }

    appBreadcrumb.value = [...baseHomeRoute, ...breadcrumb]
  }

  const updateAppPageTitle = (pageTitle = '') => {
    appPageTitle.value = pageTitle
  }

  const showSnackbar = ({ message, color = 'success', timeout = 5000 }: Omit<SnackbarState, 'show' | 'timeout'> & { timeout?: number }) => {
    snackbar.value = {
      show: true,
      message,
      color,
      timeout,
    }
  }

  return {
    appBreadcrumb,
    setAppBreadcrumb,
    appPageTitle,
    updateAppPageTitle,
    snackbar,
    showSnackbar,
  }
})
