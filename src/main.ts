import { createApp } from 'vue'
import type { DirectiveBinding } from 'vue'
import { registerPlugins, router } from '@/plugins'
import App from './App.vue'
import * as Sentry from '@sentry/vue'
import { authService } from '@/services/auth.service'

import '@core/scss/template/index.scss'
import '@styles/styles.scss'

if (!import.meta.env.VITE_API_BASE_URL) {
  console.warn('⚠️ VITE_API_BASE_URL is not set — API calls may fail')
}

async function bootstrap () {
  const app = createApp(App)

  if (import.meta.env.VITE_SENTRY_DSN) {
    Sentry.init({
      app,
      dsn: import.meta.env.VITE_SENTRY_DSN,
      environment: import.meta.env.MODE,
      integrations: [Sentry.browserTracingIntegration()],
      tracesSampleRate: 0.1,
    })
  }

  // Register non-router plugins first (router triggers auth guards immediately)
  registerPlugins(app)

  app.config.errorHandler = (
    err: unknown,
    _instance: unknown,
    info: string,
  ) => {
    console.error('[Global Error Handler]', err, info)
  }

  function checkPermission (el: HTMLElement, binding: DirectiveBinding) {
    const requiredPermission = binding.value
    if (!requiredPermission) return
    const userPermissions: string[] =
      authService.getStoredUser()?.permissions || []
    const hasPermission = Array.isArray(requiredPermission)
      ? requiredPermission.some((p: string) => userPermissions.includes(p))
      : userPermissions.includes(requiredPermission)

    if (!hasPermission) {
      const placeholder = document.createComment('v-can')
      el.parentNode?.replaceChild(placeholder, el);
      (el as any).__v_can_placeholder = placeholder
    }
  }

  function restoreElement (el: HTMLElement) {
    const placeholder = (el as any).__v_can_placeholder as Comment | undefined
    if (placeholder?.parentNode) {
      placeholder.parentNode.replaceChild(el, placeholder)
      delete (el as any).__v_can_placeholder
    }
  }

  app.directive('can', {
    mounted (el: HTMLElement, binding: DirectiveBinding) {
      checkPermission(el, binding)
    },
    updated (el: HTMLElement, binding: DirectiveBinding) {
      restoreElement(el)
      checkPermission(el, binding)
    },
    beforeUnmount (el: HTMLElement) {
      restoreElement(el)
    },
  })

  // Attempt to restore session from HttpOnly refresh cookie BEFORE router registers
  // This ensures authService._token is set before auth guards run
  await authService.initialize()

  // Now register router — authGuard will see _token if refresh succeeded
  app.use(router)

  app.mount('#app')
}

bootstrap()
