import { createApp } from 'vue'
import { registerPlugins } from '@/plugins'
import App from './App.vue'
import * as Sentry from '@sentry/vue'
import { authService } from '@/services/auth.service'

import '@core/scss/template/index.scss'
import '@styles/styles.scss'

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

registerPlugins(app)

app.config.errorHandler = (err, instance, info) => {
  console.error('[Global Error Handler]', err, info)
}

app.directive('can', {
  mounted(el, binding) {
    const requiredPermission = binding.value
    if (!requiredPermission) return
    const userPermissions = authService.getStoredUser()?.user?.permissions || []
    const hasPermission = Array.isArray(requiredPermission)
      ? requiredPermission.some(p => userPermissions.includes(p))
      : userPermissions.includes(requiredPermission)
    if (!hasPermission) el.remove()
  },
})

app.mount('#app')
