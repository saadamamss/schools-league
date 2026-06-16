import api from './client'

export interface LoginCredentials {
  email: string
  password: string
}

export const authApi = {
  login: (credentials: LoginCredentials) => api.post('auth/login', credentials),
  register: (data: any) => api.post('auth/register', data),
  verifyRegistration: (data: { email: string; otp: string }) =>
    api.post('auth/verify-registration', data),
  refresh: (token: string) => api.post('auth/refresh', { token }),
  profile: () => api.get('auth/profile'),
}
