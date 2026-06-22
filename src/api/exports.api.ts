import axiosIns from '@/plugins/axios'

export const exportsApi = {
  downloadExcel (url: string, filters: Record<string, unknown> = {}) {
    return axiosIns.post(url, filters, { responseType: 'blob' })
  },

  downloadPdf (url: string, filters: Record<string, unknown> = {}) {
    return axiosIns.post(url, {}, { responseType: 'blob', ...filters })
  },
}
