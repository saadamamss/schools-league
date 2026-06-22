import { createVuetify } from 'vuetify'
import { mount } from '@vue/test-utils'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

export function createVuetifyTest () {
  return createVuetify({
    components,
    directives,
  })
}

export function mountWithVuetify (component, options = {}) {
  const vuetify = createVuetifyTest()
  return mount(component, {
    ...options,
    global: {
      plugins: [vuetify],
      ...options.global,
    },
  })
}
