import { breakpointsVuetify } from '@vueuse/core'
import { h, ref } from 'vue'
import type { Ref, VNode } from 'vue'
import {
  AppContentLayoutNav,
  ContentWidth,
  FooterType,
  NavbarType,
} from './enums'

export const config: {
  app: {
    title: string
    logo: VNode
    contentWidth: Ref<'boxed' | 'fluid'>
    contentLayoutNav: Ref<'vertical' | 'horizontal'>
    overlayNavFromBreakpoint: number
    enableI18n: boolean
    isRtl: Ref<boolean>
  }
  navbar: {
    type: Ref<'sticky' | 'static' | 'hidden'>
    navbarBlur: Ref<boolean>
  }
  footer: { type: Ref<'sticky' | 'static' | 'hidden'> }
  verticalNav: {
    isVerticalNavCollapsed: Ref<boolean>
    defaultNavItemIconProps: { icon: string }
  }
  horizontalNav: {
    type: Ref<string>
  }
  icons: {
    chevronDown: { icon: string }
    chevronRight: { icon: string }
    close: { icon: string }
    verticalNavPinned: { icon: string }
    verticalNavUnPinned: { icon: string }
    sectionTitlePlaceholder: { icon: string }
  }
} = {
  app: {
    title: 'Title',
    logo: h('img', { src: '/src/assets/logo.svg' }),
    contentWidth: ref(ContentWidth.Boxed) as Ref<'boxed' | 'fluid'>,
    contentLayoutNav: ref(AppContentLayoutNav.Vertical) as Ref<'vertical' | 'horizontal'>,
    overlayNavFromBreakpoint: breakpointsVuetify.md,
    enableI18n: false,
    isRtl: ref(true),
  },
  navbar: {
    type: ref(NavbarType.Sticky) as Ref<'sticky' | 'static' | 'hidden'>,
    navbarBlur: ref(true),
  },
  footer: { type: ref(FooterType.Static) as Ref<'sticky' | 'static' | 'hidden'> },
  verticalNav: {
    isVerticalNavCollapsed: ref(false),
    defaultNavItemIconProps: { icon: 'mdi-circle' },
  },
  horizontalNav: {
    type: ref('sticky'),
  },
  icons: {
    chevronDown: { icon: 'mdi-chevron-down' },
    chevronRight: { icon: 'mdi-chevron-right' },
    close: { icon: 'mdi-close' },
    verticalNavPinned: { icon: 'mdi-pin' },
    verticalNavUnPinned: { icon: 'mdi-pin-off' },
    sectionTitlePlaceholder: { icon: 'mdi-minus' },
  },
}
