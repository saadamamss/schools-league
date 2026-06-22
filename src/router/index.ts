import { createRouter, createWebHistory } from 'vue-router'
import type {
  RouteLocationNormalized,
  Router,
  RouteRecordRaw,
} from 'vue-router'

import routes from './routes'
import { authService } from '@/services/auth.service'
import { userHasRoutePermission } from '@/utils/usersTypes'

const router: Router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes as RouteRecordRaw[],
  scrollBehavior (to, from, savedPosition) {
    return savedPosition || { top: 0 }
  },
})

export async function authGuard (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: any,
) {
  const isAuthenticated = authService.isAuthenticated()

  if (isAuthenticated) {
    if (to.name && !userHasRoutePermission(to.name)) {
      if (to.path === '/') {
        authService.logout()
        return next('/auth/login')
      }
      return next('/')
    }
  }

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next('/auth/login')
  } else if (to.meta.guest && isAuthenticated) {
    return next('/')
  } else {
    return next()
  }
}

router.beforeEach(authGuard)

router.onError((err, to) => {
  if (err?.message?.includes?.('Failed to fetch dynamically imported module')) {
    if (!localStorage.getItem('vuetify:dynamic-reload')) {
      console.warn('Reloading page to fix dynamic import error')
      localStorage.setItem('vuetify:dynamic-reload', 'true')
      location.assign(to.fullPath)
    } else {
      console.error('Dynamic import error, reloading page did not fix it', err)
    }
  } else {
    console.error(err)
  }
})

router.isReady().then(() => {
  localStorage.removeItem('vuetify:dynamic-reload')
})

export default router
