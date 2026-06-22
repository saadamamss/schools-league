import axiosIns from '@/plugins/axios'
import type { ApiResponse, Location } from '@/types/models'

export const locationsApi = {
  getList (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<Location[]>>('locations', { params })
  },

  getById (id: number | string) {
    return axiosIns.get<ApiResponse<Location>>(`locations/${id}`)
  },

  updateCorrection (id: number | string, data: Record<string, unknown>) {
    return axiosIns.put(`location-corrections/${id}`, data)
  },
}
