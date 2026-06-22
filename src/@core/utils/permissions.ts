import { authService } from '@/services/auth.service'

export const hasPermission = (permissionLabel: string | string[], hasSome = false): boolean => {
  const userPermissions: string[] = authService.getStoredUser()?.user?.permissions || []
  if (!userPermissions) return false
  if (Array.isArray(permissionLabel) && permissionLabel.length) {
    if (hasSome) {
      return permissionLabel.some(permission => userPermissions?.includes(permission))
    }
    return permissionLabel.every(permission => userPermissions?.includes(permission))
  }
  return userPermissions?.includes(permissionLabel as string)
}

export const canAccessObserverInfo = (): boolean => {
  const user = authService.getStoredUser()?.user || null
  return Array.isArray(user?.parent_companies)
    ? user.parent_companies.length === 0
    : true
}

export const hasRole = (roleId: number): boolean => {
  const user = authService.getStoredUser()?.user || null
  return user ? user.role?.id === roleId : false
}
