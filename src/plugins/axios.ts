import axios from 'axios'
import type { AxiosResponse } from 'axios'
import axiosRetry from 'axios-retry'
import { API_BASE_URL, API_ENDPOINTS } from '@/config'
import { authService } from '@/services/auth.service'
import { useAppStore } from '@/stores/app'

const axiosIns = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    Accept: 'application/json',
  },
  withCredentials: true,
})

axiosRetry(axiosIns, {
  retries: 3,
  retryDelay: axiosRetry.exponentialDelay,
  shouldResetTimeout: true,
  retryCondition: error => {
    return axiosRetry.isNetworkOrIdempotentRequestError(error) ||
      (error.response?.status != null && error.response.status >= 500 && error.response.status < 600)
  },
})

export function requestInterceptor (config: any) {
  const token = authService.getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}

axiosIns.interceptors.request.use(
  requestInterceptor,
  error => Promise.reject(error),
)

let isRefreshing = false
let failedQueue: Array<{ resolve: (token: string) => void; reject: (error: unknown) => void }> = []

function processQueue (error: unknown, token: string | null = null) {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token as string)
    }
  })
  failedQueue = []
}

export function handle422Errors (error: any) {
  if (error.response?.status === 422) {
    const errorsObj = error.response?.data?.errors
    if (errorsObj) {
      for (const key in errorsObj) {
        if (Object.prototype.hasOwnProperty.call(errorsObj, key)) {
          const messages = errorsObj[key]
          ;(Array.isArray(messages) ? messages : [messages]).forEach((value: string) => {
            useAppStore().showSnackbar({
              message: value,
              color: 'error',
              timeout: 5000,
            })
          })
        }
      }
    }
  }
  return Promise.reject(error)
}

export async function responseErrorHandler (error: any) {
  const originalRequest = error.config

  if (
    !originalRequest ||
    error.response?.status !== 401 ||
    originalRequest._retry
  ) {
    return handle422Errors(error)
  }

  if (isRefreshing) {
    return new Promise<string>((resolve, reject) => {
      failedQueue.push({ resolve, reject })
    }).then(token => {
      originalRequest.headers.Authorization = `Bearer ${token}`
      return axiosIns(originalRequest)
    })
  }

  originalRequest._retry = true
  isRefreshing = true

  try {
    // The HttpOnly refresh cookie requires withCredentials for cross-origin requests.
    // Also send the in-memory refresh token as body fallback for browsers that
    // block third-party cookies.
    const refreshToken = authService.getRefreshToken()
    const response = await axios.post(
      API_BASE_URL + API_ENDPOINTS.REFRESH_TOKEN,
      refreshToken ? { token: refreshToken } : {},
      { withCredentials: true },
    )
    const newToken = response.data?.token || response.data?.data?.token
    const newRefreshToken = response.data?.refreshToken || response.data?.data?.refreshToken
    if (!newToken) throw new Error('No token in refresh response')

    const user = authService.getStoredUser() || {}
    authService.setUser({ ...user, token: newToken, ...(newRefreshToken && { refreshToken: newRefreshToken }) })
    axiosIns.defaults.headers.common.Authorization = `Bearer ${newToken}`

    processQueue(null, newToken)
    originalRequest.headers.Authorization = `Bearer ${newToken}`
    return axiosIns(originalRequest)
  } catch (refreshError: any) {
    processQueue(refreshError, null)
    authService.logout()
    window.location.href = '/auth/login'
    return Promise.reject(refreshError)
  } finally {
    isRefreshing = false
  }
}

axiosIns.interceptors.response.use(
  (response: AxiosResponse) => response,
  responseErrorHandler,
)

export default axiosIns
