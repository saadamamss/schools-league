export const mapStyleURL: string = import.meta.env.VITE_MAPBOX_STYLE_URL || 'mapbox://styles/reda2508/cm3jws2cc00mg01scatducfs3'

export const API_BASE_URL: string = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5231/api/dashboard'
export const LOCAL_STORAGE_USER_KEY: string = 'app_storage_key_user'

export const API_ENDPOINTS = {
  LOGIN: '/login',
  LOGOUT: '/logout',
  REFRESH_TOKEN: '/auth/refresh',
} as const
