import { computed, inject, ref, watch } from 'vue'
import type { Ref } from 'vue'
import { useRoute } from 'vue-router'
import { AppContentLayoutNav, NavbarType } from '../enums'
import { config } from '../config'

const injectionKeyIsVerticalNavHovered = Symbol('isVerticalNavHovered')

export const useLayouts = () => {
  const navbarType = computed({
    get () {
      return config.navbar.type.value
    },
    set (value) {
      config.navbar.type.value = value
    },
  })

  const isNavbarBlurEnabled = computed({
    get () {
      return config.navbar.navbarBlur.value
    },
    set (value) {
      config.navbar.navbarBlur.value = value
      localStorage.setItem(`${config.app.title}-navbarBlur`, value.toString())
    },
  })

  const _setAppDir = (dir: string) => {
    document.documentElement.setAttribute('dir', dir)
  }

  const footerType = computed({
    get () {
      return config.footer.type.value
    },
    set (value) {
      config.footer.type.value = value
    },
  })

  const isVerticalNavCollapsed = computed({
    get () {
      return config.verticalNav.isVerticalNavCollapsed.value
    },
    set (val) {
      config.verticalNav.isVerticalNavCollapsed.value = val
      localStorage.setItem(`${config.app.title}-isVerticalNavCollapsed`, val.toString())
    },
  })

  const appContentWidth = computed({
    get () {
      return config.app.contentWidth.value
    },
    set (val) {
      config.app.contentWidth.value = val
      localStorage.setItem(`${config.app.title}-contentWidth`, val.toString())
    },
  })

  const appContentLayoutNav = computed({
    get (): 'vertical' | 'horizontal' {
      return config.app.contentLayoutNav.value
    },
    set (val: 'vertical' | 'horizontal') {
      config.app.contentLayoutNav.value = val
      if (val === AppContentLayoutNav.Horizontal) {
        if (navbarType.value === NavbarType.Hidden) { navbarType.value = NavbarType.Sticky }
        isVerticalNavCollapsed.value = false
      }
    },
  })

  const horizontalNavType = computed({
    get () {
      return config.horizontalNav.type.value
    },
    set (value) {
      config.horizontalNav.type.value = value
    },
  })

  const isLessThanOverlayNavBreakpoint = computed(() => {
    return (windowWidth: number) => windowWidth < config.app.overlayNavFromBreakpoint
  })

  const _layoutClasses = computed(() => (windowWidth: number, windowScrollY: number) => {
    const route = useRoute()

    return [
      `layout-nav-type-${appContentLayoutNav.value}`,
      `layout-navbar-${navbarType.value}`,
      `layout-footer-${footerType.value}`,
      {
        'layout-vertical-nav-collapsed': isVerticalNavCollapsed.value &&
                    (appContentLayoutNav.value as string) === 'vertical' &&
                    !isLessThanOverlayNavBreakpoint.value(windowWidth),
      },
      { [`horizontal-nav-${horizontalNavType.value}`]: appContentLayoutNav.value === 'horizontal' },
      `layout-content-width-${appContentWidth.value}`,
      { 'layout-overlay-nav': isLessThanOverlayNavBreakpoint.value(windowWidth) },
      { 'window-scrolled': windowScrollY > 0 },
      route.meta.layoutWrapperClasses as string | null,
    ]
  })

  const switchToVerticalNavOnLtOverlayNavBreakpoint = (windowWidth: number) => {
    const lgAndUpNav = ref(appContentLayoutNav.value)

    watch(appContentLayoutNav, value => {
      if (!isLessThanOverlayNavBreakpoint.value(windowWidth)) { lgAndUpNav.value = value }
    })

    watch(() => isLessThanOverlayNavBreakpoint.value(windowWidth), val => {
      if (!val) { appContentLayoutNav.value = lgAndUpNav.value } else { appContentLayoutNav.value = AppContentLayoutNav.Vertical }
    }, { immediate: true })
  }

  const isVerticalNavMini = (windowWidth: number, isVerticalNavHovered: boolean | null = null) => {
    const isVerticalNavHoveredLocal = isVerticalNavHovered !== null
      ? { value: isVerticalNavHovered }
      : (inject(injectionKeyIsVerticalNavHovered) as Ref<boolean> | undefined) || ref(false)

    return computed(() => isVerticalNavCollapsed.value && !isVerticalNavHoveredLocal.value && !isLessThanOverlayNavBreakpoint.value(windowWidth))
  }

  const dynamicI18nProps = computed(() => (key: string, tag = 'span') => {
    if (config.app.enableI18n) {
      return {
        keypath: key,
        tag,
        scope: 'global',
      }
    }

    return {} as Record<string, unknown>
  })

  const isAppRtl = computed({
    get () {
      return config.app.isRtl.value
    },
    set (value) {
      config.app.isRtl.value = value
      localStorage.setItem(`${config.app.title}-isRtl`, value.toString())
      _setAppDir(value ? 'rtl' : 'ltr')
    },
  })

  return {
    navbarType,
    isNavbarBlurEnabled,
    footerType,
    isVerticalNavCollapsed,
    appContentWidth,
    appContentLayoutNav,
    horizontalNavType,
    isLessThanOverlayNavBreakpoint,
    _layoutClasses,
    switchToVerticalNavOnLtOverlayNavBreakpoint,
    isVerticalNavMini,
    dynamicI18nProps,
    isAppRtl,
    _setAppDir,
  }
}
