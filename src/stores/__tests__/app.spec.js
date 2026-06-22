import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it } from 'vitest'
import { useAppStore } from '@/stores/app'

describe('app store', () => {
  let store

  beforeEach(() => {
    setActivePinia(createPinia())
    store = useAppStore()
  })

  it('sets breadcrumb with base home route', () => {
    store.setAppBreadcrumb()
    expect(store.appBreadcrumb).toEqual([{ label: 'الرئيسية', to: '/' }])
  })

  it('sets breadcrumb with extended routes', () => {
    store.setAppBreadcrumb([{ label: 'المستخدمين', to: '/users' }])
    expect(store.appBreadcrumb).toHaveLength(2)
    expect(store.appBreadcrumb[1].label).toBe('المستخدمين')
  })

  it('sets base breadcrumb when isBaseRoute is true', () => {
    store.setAppBreadcrumb([{ label: 'المستخدمين', to: '/users' }], true)
    expect(store.appBreadcrumb).toHaveLength(1)
    expect(store.appBreadcrumb[0].label).toBe('الرئيسية')
  })

  it('updates page title', () => {
    store.updateAppPageTitle('لوحة التحكم')
    expect(store.appPageTitle).toBe('لوحة التحكم')
  })

  it('shows snackbar with defaults', () => {
    store.showSnackbar({ message: 'تم بنجاح' })
    expect(store.snackbar.show).toBe(true)
    expect(store.snackbar.message).toBe('تم بنجاح')
    expect(store.snackbar.color).toBe('success')
    expect(store.snackbar.timeout).toBe(5000)
  })

  it('shows snackbar with custom color', () => {
    store.showSnackbar({ message: 'خطأ', color: 'error', timeout: 3000 })
    expect(store.snackbar.color).toBe('error')
    expect(store.snackbar.timeout).toBe(3000)
  })
})
