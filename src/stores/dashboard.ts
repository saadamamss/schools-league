import { referenceApi } from '@/api'
import { defineStore } from 'pinia'
import { useApiError } from '@/composables/useApiError'
import type { Location, Nationality, UserCity, UserType } from '@/types/models'

interface DashboardState {
  userTypes: UserType[]
  city: UserCity[]
  nationalities: Nationality[]
  locations: Location[]
}

export const useDashboardStore = defineStore('DashboardStore', {
  state: (): DashboardState => ({
    userTypes: [],
    city: [],
    nationalities: [],
    locations: [],
  }),
  actions: {
    async fetchUserTypes () {
      const { handleError } = useApiError('fetchUserTypes')
      try {
        const response = await referenceApi.getUserTypes()
        if (response.data) {
          this.userTypes = response.data?.data
        }
      } catch (error: unknown) {
        handleError(error, { fallback: 'حدث خطأ في تحميل أنواع المستخدمين' })
      }
    },

    async fetchCities (search = '') {
      const { handleError } = useApiError('fetchCities')
      try {
        const searchFilter = !search.trim() ? {} : { search: search.trim() }
        const response = await referenceApi.getCities({ per_page: 15, ...searchFilter })
        if (response.data) {
          this.city = response.data?.data
        }
      } catch (error: unknown) {
        handleError(error, { fallback: 'حدث خطأ في تحميل المدن' })
      }
    },

    async fetchNationalities (search = '') {
      const { handleError } = useApiError('fetchNationalities')
      try {
        const searchFilter = !search.trim() ? {} : { search: search.trim() }
        const response = await referenceApi.getNationalities({ per_page: 15, ...searchFilter })
        if (response.data) {
          this.nationalities = response.data?.data
        }
      } catch (error: unknown) {
        handleError(error, { fallback: 'حدث خطأ في تحميل الجنسيات' })
      }
    },

    async fetchLocations (search = '', cityId: number | undefined = undefined) {
      const { handleError } = useApiError('fetchLocations')
      try {
        const searchFilter = !search.trim() ? {} : { search: search.trim() }
        const response = await referenceApi.getLocations({ per_page: 15, ...searchFilter, city_id: cityId })
        if (response.data) {
          this.locations = response.data?.data
        }
      } catch (error: unknown) {
        handleError(error, { fallback: 'حدث خطأ في تحميل المواقع' })
      }
    },
  },
})
