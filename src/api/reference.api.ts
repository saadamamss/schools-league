import axiosIns from '@/plugins/axios'
import type { ApiResponse, Location, Nationality, UserCity, UserType } from '@/types/models'

export const referenceApi = {
  getUserTypes () {
    return axiosIns.get<ApiResponse<UserType[]>>('reference/user-types')
  },

  getCities (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<UserCity[]>>('reference/cities', { params })
  },

  getNationalities (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<Nationality[]>>('reference/nationalities', { params })
  },

  getLocations (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<Location[]>>('reference/locations', { params })
  },
}
