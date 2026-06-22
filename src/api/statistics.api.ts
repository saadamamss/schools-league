import axiosIns from '@/plugins/axios'

export interface StatisticsParams {
  attendance_period?: string
  attendance_by_location_period?: string
  absence_by_location_period?: string
}

export const statisticsApi = {
  getDashboard (params: StatisticsParams, signal?: AbortSignal) {
    return axiosIns.get('statistics', { params, signal })
  },
}
