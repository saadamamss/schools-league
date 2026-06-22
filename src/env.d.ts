/// <reference types="vite/client" />

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

declare module 'vue-router/auto' {
  export * from 'vue-router'
}

declare module '@/@core/utils/helpers' {
  export function saveToStorage(key: string, value: unknown): void
  export function getFromStorage<T = any>(key: string): T | null
  export function removeFromStorage(key: string): void
  export function toSnakeCase(str: string): string
  export function toSnakeCaseDeep<T>(obj: T): T
  export function exportData(url: string, params?: Record<string, any>): Promise<void>
}

declare module '@/utils/usersTypes' {
  export function userHasRoutePermission(routeName: RouteLocationNamedRaw | string | symbol | undefined | null): boolean
}

declare module 'vue-tel-input' {
  const component: any
  export default component
  export const VueTelInput: any
}

declare module 'vue3-otp-input' {
  const component: any
  export default component
}

declare module 'vue3-perfect-scrollbar' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{ tag?: string; options?: any }, {}, any>
  export default component
  export const PerfectScrollbar: typeof component
  export const PerfectScrollbarPlugin: any
}

declare module '@/plugins/axios' {
  import type { AxiosInstance } from 'axios'
  const axiosIns: AxiosInstance
  export default axiosIns
}

declare module '@/config' {
  export const mapStyleURL: string
  export const API_BASE_URL: string
  export const LOCAL_STORAGE_USER_KEY: string
  export const API_ENDPOINTS: {
    LOGIN: string
    LOGOUT: string
    REFRESH_TOKEN: string
  }
}

declare module 'js-cookie' {
  const Cookies: {
    get: (key: string) => string | undefined
    set: (key: string, value: string, options?: Record<string, any>) => void
    remove: (key: string, options?: Record<string, any>) => void
  }
  export default Cookies
}
