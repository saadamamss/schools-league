import { ref, watch } from 'vue'
import { useDisplay } from 'vuetify'

export const useResponsiveLeftSidebar = (mobileBreakpoint?: boolean) => {
  const { mdAndDown, name: currentBreakpoint } = useDisplay()
  const _mobileBreakpoint = mobileBreakpoint !== undefined ? { value: mobileBreakpoint } : mdAndDown
  const isLeftSidebarOpen = ref(true)

  const setInitialValue = () => {
    isLeftSidebarOpen.value = !_mobileBreakpoint.value
  }

  setInitialValue()
  watch(currentBreakpoint, () => {
    isLeftSidebarOpen.value = !_mobileBreakpoint.value
  })

  return {
    isLeftSidebarOpen,
  }
}
