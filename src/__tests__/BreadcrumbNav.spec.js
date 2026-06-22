import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { createRouter, createWebHistory } from 'vue-router'
import { h } from 'vue'

import BreadcrumbNav from '@/components/BreadcrumbNav.vue'

const vuetify = createVuetify({ components, directives })

const router = createRouter({
  history: createWebHistory(),
  routes: [{ path: '/', component: { render: () => h('div', 'home') } }],
})

function mountBreadcrumb (items) {
  return mount(BreadcrumbNav, {
    props: { items },
    global: { plugins: [vuetify, router] },
  })
}

describe('BreadcrumbNav', () => {
  it('renders breadcrumb items', () => {
    const wrapper = mountBreadcrumb([
      { title: 'الرئيسية', href: '/' },
      { title: 'المستخدمين', href: '/users' },
    ])
    expect(wrapper.text()).toContain('الرئيسية')
    expect(wrapper.text()).toContain('المستخدمين')
  })

  it('renders disabled item as text', () => {
    const wrapper = mountBreadcrumb([
      { title: 'الصفحة الحالية', disabled: true },
    ])
    expect(wrapper.text()).toContain('الصفحة الحالية')
  })
})
