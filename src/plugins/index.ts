import type { App } from 'vue'
import vuetify from './vuetify'
import pinia from '@/stores'
import router from '@/router'
import VueTelInput from 'vue-tel-input'
import 'vue-tel-input/vue-tel-input.css'

export function registerPlugins (app: App) {
  app.use(vuetify).use(VueTelInput).use(pinia)
}

export { router }
