import { computed, watch } from 'vue'
import { useLayouts } from '@layouts'
import { themeConfig } from '@themeConfig'
import { useTheme } from 'vuetify'

export const useThemeConfig = () => {
  const theme = computed({
    get () {
      return themeConfig.app.theme.value
    },
    set (value) {
      themeConfig.app.theme.value = value
      localStorage.setItem(`${themeConfig.app.title}-theme`, value.toString())
    },
  })

  const isVerticalNavSemiDark = computed({
    get () {
      return themeConfig.verticalNav.isVerticalNavSemiDark.value
    },
    set (value) {
      themeConfig.verticalNav.isVerticalNavSemiDark.value = value
      localStorage.setItem(`${themeConfig.app.title}-isVerticalNavSemiDark`, value.toString())
    },
  })

  const syncVuetifyThemeWithTheme = () => {
    const vuetifyTheme = useTheme()

    watch(theme, val => {
      vuetifyTheme.global.name.value = val
    })
  }

  const syncInitialLoaderTheme = () => {
    const vuetifyTheme = useTheme()

    watch(theme, val => {
      localStorage.setItem(`${themeConfig.app.en_title}-initial-loader-bg`, vuetifyTheme.themes.value[val].colors.surface)
      localStorage.setItem(`${themeConfig.app.en_title}-initial-loader-color`, vuetifyTheme.themes.value[val].colors.primary)
    }, {
      immediate: true,
    })
  }

  const skin = computed({
    get () {
      return themeConfig.app.skin.value
    },
    set (value) {
      themeConfig.app.skin.value = value
      localStorage.setItem(`${themeConfig.app.title}-skin`, value)
    },
  })

  const appRouteTransition = computed({
    get () {
      return themeConfig.app.routeTransition.value
    },
    set (value) {
      themeConfig.app.routeTransition.value = value
      localStorage.setItem(`${themeConfig.app.title}-transition`, value)
    },
  })

  const { navbarType, isNavbarBlurEnabled, footerType, isVerticalNavCollapsed, appContentWidth, appContentLayoutNav, horizontalNavType, isLessThanOverlayNavBreakpoint, isAppRtl, switchToVerticalNavOnLtOverlayNavBreakpoint } = useLayouts()

  return {
    theme,
    isVerticalNavSemiDark,
    syncVuetifyThemeWithTheme,
    syncInitialLoaderTheme,
    skin,
    appRouteTransition,
    navbarType,
    isNavbarBlurEnabled,
    footerType,
    isVerticalNavCollapsed,
    appContentWidth,
    appContentLayoutNav,
    horizontalNavType,
    isLessThanOverlayNavBreakpoint,
    isAppRtl,
    switchToVerticalNavOnLtOverlayNavBreakpoint,
  }
}
