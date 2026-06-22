import { useDynamicVhCssProperty } from './composable/useDynamicVhCssProperty'
import { config } from './config'
import { ContentWidth } from './enums'
import { useLayouts } from './composable/useLayouts'

const { _setAppDir } = useLayouts()

export const createLayouts = (userConfig: Record<string, Record<string, unknown>>) => {
  const localStorageIsRtl = localStorage.getItem(`${userConfig.app.title}-isRtl`)
  const localStorageIsVerticalNavCollapsed = localStorage.getItem(`${userConfig.app.title}-isVerticalNavCollapsed`)

  const localStorageContentWidth = (() => {
    const storageValue = localStorage.getItem(`${userConfig.app.title}-contentWidth`)

    return Object.values(ContentWidth).find(v => v === storageValue)
  })()

  const localStorageNavbarBlur = localStorage.getItem(`${userConfig.app.title}-navbarBlur`)

  config.app.title = userConfig.app.title as string
  config.app.logo = userConfig.app.logo as never
  config.app.contentWidth.value = (localStorageContentWidth || userConfig.app.contentWidth) as 'boxed' | 'fluid'
  config.app.contentLayoutNav.value = (userConfig.app.contentLayoutNav || config.app.contentLayoutNav.value) as 'vertical' | 'horizontal'
  config.app.overlayNavFromBreakpoint = userConfig.app.overlayNavFromBreakpoint as number
  config.app.enableI18n = userConfig.app.enableI18n as boolean
  config.app.isRtl.value = localStorageIsRtl ? JSON.parse(localStorageIsRtl) : (userConfig.app.isRtl as boolean)
  config.navbar.type.value = userConfig.navbar.type as 'sticky' | 'static' | 'hidden'
  config.navbar.navbarBlur.value = localStorageNavbarBlur ? JSON.parse(localStorageNavbarBlur) : (userConfig.navbar.navbarBlur as boolean)
  config.footer.type.value = userConfig.footer.type as 'sticky' | 'static' | 'hidden'
  config.verticalNav.isVerticalNavCollapsed.value =
        localStorageIsVerticalNavCollapsed
          ? JSON.parse(localStorageIsVerticalNavCollapsed)
          : (userConfig.verticalNav.isVerticalNavCollapsed as boolean)
  config.verticalNav.defaultNavItemIconProps = (userConfig.verticalNav.defaultNavItemIconProps || config.verticalNav.defaultNavItemIconProps) as { icon: string }
  config.horizontalNav.type.value = userConfig.horizontalNav.type as string
  config.icons.chevronDown = userConfig.icons.chevronDown as { icon: string }
  config.icons.chevronRight = userConfig.icons.chevronRight as { icon: string }
  config.icons.close = userConfig.icons.close as { icon: string }
  config.icons.verticalNavPinned = userConfig.icons.verticalNavPinned as { icon: string }
  config.icons.verticalNavUnPinned = userConfig.icons.verticalNavUnPinned as { icon: string }
  config.icons.sectionTitlePlaceholder = userConfig.icons.sectionTitlePlaceholder as { icon: string }

  return () => {
    useDynamicVhCssProperty()
    _setAppDir(config.app.isRtl.value ? 'rtl' : 'ltr')
  }
}

export const injectionKeyIsVerticalNavHovered = Symbol('isVerticalNavHovered')
export * from './components'
export { useLayouts } from './composable/useLayouts'
