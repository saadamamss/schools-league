export interface ThemeConfig {
  skin: 'default' | 'bordered'
  appRouteTransition: string
  isLessThanOverlayNavBreakpoint: (width: number) => boolean
}

export interface SkinConfig {
  name: string
  value: string
}
