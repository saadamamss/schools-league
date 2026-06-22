import axiosIns from '@/plugins/axios'
import type { ApiResponse, User } from '@/types/models'

export const usersApi = {
  getList (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<User[]>>('users', { params })
  },

  getById (id: number | string, config = {}) {
    return axiosIns.get<ApiResponse<User>>(`users/${id}`, config)
  },
}
