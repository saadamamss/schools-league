import { authService } from '@/services/auth.service'

interface UserTypeMap {
  [key: string]: string[]
}

export const userTypes: UserTypeMap = {
  operations_manager: [
    'home',
    'users',
    'manage-locations',
    'attendances',
    'profile',
  ],
  project_manager: ['home', 'users', 'manage-locations', 'attendances', 'profile'],
  supervisor: ['home', 'users', 'manage-locations', 'attendances', 'profile'],
  protocol: ['home', 'users', 'manage-locations', 'attendances', 'profile'],
  organizer: ['home', 'profile', 'logout'],
}

export const userHasRoutePermission = (route: string): boolean => {
  const stored = authService.getStoredUser()
  const user = stored?.user ?? stored
  const code = user?.userType?.code || user?.user_type?.code

  if (!code) return true
  const allowed = userTypes[code]
  if (!allowed) return false

  return allowed.includes(route)
}
