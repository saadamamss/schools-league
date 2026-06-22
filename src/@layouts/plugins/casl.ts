import ability from '@/plugins/casl/ability'
import { getCurrentInstance } from 'vue'

export const can = (action: string, subject: string) => {
  const vm = getCurrentInstance()
  if (!vm) { return false }
  const proxy = vm.proxy as Record<string, unknown> | null
  const localCan = proxy && '$can' in proxy

  return localCan ? (proxy?.$can as (a: string, b: string) => boolean)(action, subject) : true
}

export const canViewNavMenuGroup = (item: { children: Array<{ action: string; subject: string }>; action?: string; subject?: string }) => {
  const hasAnyVisibleChild = item.children.some((i: { action: string; subject: string }) => can(i.action, i.subject))

  if (!(item.action && item.subject)) { return hasAnyVisibleChild }

  return can(item.action, item.subject) && hasAnyVisibleChild
}

export const canNavigate = (to: { matched: Array<{ meta: { action: string; subject: string } }> }) => {
  return to.matched.some(route => ability.can(route.meta.action, route.meta.subject))
}
