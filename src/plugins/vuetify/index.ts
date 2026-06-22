import { createVuetify } from 'vuetify'
import defaults from './defaults'
import { icons } from './icons'
import theme from './theme'
import { ar } from 'vuetify/locale'

import '@mdi/font/css/materialdesignicons.css'
import '@core/scss/template/libs/vuetify/index.scss'
import 'vuetify/styles'

export default createVuetify({
  defaults,
  icons,
  theme,
  locale: {
    locale: 'ar',
    fallback: 'en',
    messages: { ar },
  },
})
