import axiosIns from '@/plugins/axios'
import type { ApiResponse, AttendanceRecord } from '@/types/models'

export const attendanceApi = {
  getList (params: Record<string, unknown> = {}) {
    return axiosIns.get<ApiResponse<AttendanceRecord[]>>('attendance', { params })
  },
}
