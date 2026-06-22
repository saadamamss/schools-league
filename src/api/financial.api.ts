import axiosIns from '@/plugins/axios'

export const financialApi = {
  getByUser (userId: number | string, params: Record<string, unknown> = {}) {
    return axiosIns.get(`financial-transactions/user/${userId}`, { params })
  },
}
