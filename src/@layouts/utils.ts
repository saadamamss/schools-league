import { computed, ref } from 'vue'
import type { Router } from 'vue-router'

interface NavLink {
  to?: string | Record<string, unknown>
  href?: string
  target?: string
  rel?: string
  children?: NavLink[]
}

export const openGroups = ref<string[]>([])

export const getComputedNavLinkToProp = computed(() => (link: NavLink) => {
  const props: Record<string, unknown> = {
    target: link.target,
    rel: link.rel,
  }

  if (link.to) {
    props.to = typeof link.to === 'string' ? { name: link.to } : link.to
  } else {
    props.href = link.href
  }

  return props
})

export const resolveNavLinkRouteName = (link: NavLink, router: Router): string | null => {
  if (!link.to) return null
  if (typeof link.to === 'string') return link.to
  return router.resolve(link.to).name as string | null
}

export const isNavLinkActive = (link: NavLink, router: Router): boolean => {
  const matchedRoutes = router.currentRoute.value.matched
  const resolveRoutedName = resolveNavLinkRouteName(link, router)
  if (!resolveRoutedName) return false

  return matchedRoutes.some(route => {
    return (
      route.name === resolveRoutedName ||
      (route.meta?.navActiveLink as string) === resolveRoutedName
    )
  })
}

export const isNavGroupActive = (children: NavLink[], router: Router): boolean =>
  children.some(child => {
    if ('children' in child) return isNavGroupActive(child.children || [], router)
    return isNavLinkActive(child, router)
  })

export const hexToRgb = (hex: string): string | null => {
  const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i
  hex = hex.replace(shorthandRegex, (_m, r, g, b) => r + r + g + g + b + b)
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
  return result
    ? `${parseInt(result[1], 16)},${parseInt(result[2], 16)},${parseInt(result[3], 16)}`
    : null
}
