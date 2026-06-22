import axiosIns from '@/plugins/axios'
import type { User } from '@/types/models'

export const authApi = {
  login (credentials: { email: string; password: string }) {
    return axiosIns.post('auth/login', credentials)
  },

  logout () {
    return axiosIns.post('auth/logout')
  },

  register (data: Record<string, unknown>) {
    return axiosIns.post('auth/register', data)
  },

  verifyRegistration (data: { email: string; otp: string }) {
    return axiosIns.post('auth/verify-registration', data)
  },

  forgotPassword (email: string) {
    return axiosIns.post('auth/password/forgot', { email })
  },

  verifyOTP (data: { email: string; otp: string }) {
    return axiosIns.post('auth/password/verify-otp', data)
  },

  resetPassword (data: Record<string, unknown>) {
    return axiosIns.post('auth/password/reset', data)
  },

  resendRegistrationOTP (email: string) {
    return axiosIns.post('auth/resend-registration-otp', { email })
  },

  refreshToken (token: string) {
    return axiosIns.post('auth/refresh', { token })
  },

  getProfile () {
    return axiosIns.get<{ data: User }>('auth/profile')
  },

  changePassword (data: { current_password: string; new_password: string; new_password_confirmation: string }) {
    return axiosIns.post('auth/change-password', data)
  },
}
