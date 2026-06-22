<script setup lang="ts">
  import { computed, onMounted, ref, watch } from 'vue'
  import Widgets from '@/components/Widgets.vue'
  import { statisticsApi } from '@/api'
  import { useAppStore } from '@/stores/app'
  import BarChart from '@/components/charts/BarChart.vue'
  import { useTheme } from 'vuetify'
  import type { DashboardStats } from '@/types/models'
  const attendanceStates = ref<any[]>([])
  const attendRelatedToSite = ref<any[]>([])
  const absentRelatedToSite = ref<any[]>([])
  const widgetsData = ref<DashboardStats | null>(null)
  const abortController = ref<AbortController | null>(null)
  const appStore = useAppStore()
  const theme = useTheme()
  const chartKey = ref(0)
  const initialLoading = ref(true)

  const isDark = computed(() => theme.name.value === 'dark')

  // Force chart re-render when theme changes
  watch(isDark, () => { chartKey.value++ })

  const FILTEROPTIONS = [
    { value: 'month', label: 'هذا الشهر' },
    { value: 'week', label: 'هذا الأسبوع' },
    { value: 'day', label: 'هذا اليوم' },
  ]
  const attendancePeriod = ref('month')
  const attendanceByLocationPeriod = ref('month')
  const absenceByLocationPeriod = ref('month')

  const filterLabel = (value: string) => FILTEROPTIONS.find(o => o.value === value)?.label || ''

  const formatDateLabel = (stringDate: string) => {
    if (!stringDate) return ''
    const date = new Date(stringDate)
    if (isNaN(date.getTime())) return ''
    return date.toLocaleString('ar-EG', { month: 'long', day: 'numeric' })
  }
  const fetchData = async (section?: string) => {
    if (abortController.value) abortController.value.abort()
    abortController.value = new AbortController()

    try {
      const params: Record<string, any> = { attendance_period: attendancePeriod.value }
      if (!section || section === 'attendance_by_location') params.attendance_by_location_period = attendanceByLocationPeriod.value
      if (!section || section === 'absence_by_location') params.absence_by_location_period = absenceByLocationPeriod.value

      const response = await statisticsApi.getDashboard(params, abortController.value.signal)
      if (response.data?.data) {
        if (!section || section === 'attendance') attendanceStates.value = response.data.statistics?.attendance_statistics ?? []
        if (!section || section === 'attendance_by_location') attendRelatedToSite.value = response.data.statistics?.attendance_by_location ?? []
        if (!section || section === 'absence_by_location') absentRelatedToSite.value = response.data.statistics?.absence_by_location ?? []
        if (!section) widgetsData.value = response.data.data
      }
    } catch (err: any) {
      if (err.name === 'CanceledError' || err.code === 'ERR_CANCELED') return
      appStore.showSnackbar({
        message: err.response?.data?.message || 'حدث خطأ فى جلب إحصائيات الحضور',
        color: 'error',
      })
    } finally {
      initialLoading.value = false
    }
  }

  // Initial load
  onMounted(() => {
    fetchData()
  })

  const emptyChart = { labels: [], dataset: [], attendance: [], absence: [], statNames: ['الحضور', 'الغياب'] }

  const attendStatistics = computed(() => {
    const items = attendanceStates.value
    if (!items?.length) return emptyChart
    return {
      labels: items.map(i => formatDateLabel(i.date)),
      dataset: items.map(item => item.attendance + item.absence),
      attendance: items.map(item => item.attendance),
      absence: items.map(item => item.absence),
      statNames: ['الحضور', 'الغياب'],
    }
  })

  const attendOnlyStatistics = computed(() => {
    const items = attendRelatedToSite.value
    if (!items?.length) return emptyChart
    return {
      labels: items.map(i => i.location_name),
      dataset: items.map(item => item.attendance_count),
      attendance: items.map(item => item.attendance_count),
      absence: [],
      statNames: ['الحضور', 'الغياب'],
    }
  })

  const absenceOnlyStatistics = computed(() => {
    const items = absentRelatedToSite.value
    if (!items?.length) return emptyChart
    return {
      labels: items.map(i => i.location_name),
      dataset: items.map(item => item.absence_count),
      attendance: [],
      absence: items.map(item => item.absence_count),
      statNames: ['الحضور', 'الغياب'],
    }
  })
</script>
<template>
  <v-container class="px-0" fluid>
    <Widgets :data="widgetsData" :is-loading="initialLoading" />
    <v-row class="mx-0">
      <v-col class="px-0 px-md-3" cols="12">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>إحصائيات الحضور</h5>
              <p class="text-sm text-gray">{{ filterLabel(attendancePeriod) }}</p>
            </div>
            <div>
              <v-select
                v-model="attendancePeriod"
                class="no-default-radius"
                hide-details
                hide-no-data
                item-title="label"
                item-value="value"
                :items="FILTEROPTIONS"
                min-width="100"
                width="130"
                @update:model-value="fetchData('attendance')"
              />
            </div>
          </VCardTitle>
          <v-divider />
          <VCardText class="px-0 pb-0" style="min-height: 355px">
            <BarChart
              v-if="!initialLoading"
              :key="chartKey"
              color="#297A6542"
              :data="attendStatistics"
              :height="300"
              title=""
            />

            <div v-else class="loading-spinner">
              <div class="spinner" />
            </div>
          </VCardText>
        </VCard>
      </v-col>
      <v-col class="px-0 px-md-3" cols="12" md="6">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>توزيع الحضور حسب الموقع</h5>
              <p class="text-sm text-gray">{{ filterLabel(attendanceByLocationPeriod) }}</p>
            </div>
            <div>
              <v-select
                v-model="attendanceByLocationPeriod"
                class="no-default-radius"
                hide-details
                hide-no-data
                item-title="label"
                item-value="value"
                :items="FILTEROPTIONS"
                min-width="100"
                width="130"
                @update:model-value="fetchData('attendance_by_location')"
              />
            </div>
          </VCardTitle>
          <v-divider />
          <VCardText class="px-0 pb-0" style="min-height: 455px">
            <BarChart
              v-if="!initialLoading"
              :key="chartKey"
              color="#8abeae"
              :data="attendOnlyStatistics"
              :height="400"
              title=""
            />

            <div v-else class="loading-spinner">
              <div class="spinner" />
            </div>
          </VCardText>
        </VCard>
      </v-col>

      <v-col class="px-0 px-md-3" cols="12" md="6">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>الغيابات حسب الموقع</h5>
              <p class="text-sm text-gray">{{ filterLabel(absenceByLocationPeriod) }}</p>
            </div>
            <div>
              <v-select
                v-model="absenceByLocationPeriod"
                class="no-default-radius"
                hide-details
                hide-no-data
                item-title="label"
                item-value="value"
                :items="FILTEROPTIONS"
                min-width="100"
                width="130"
                @update:model-value="fetchData('absence_by_location')"
              />
            </div>
          </VCardTitle>
          <v-divider />
          <VCardText class="px-0 pb-0" style="min-height: 455px">
            <BarChart
              v-if="!initialLoading"
              :key="chartKey"
              color="#8abeae"
              :data="absenceOnlyStatistics"
              :height="400"
              title=""
            />
            <div v-else class="loading-spinner">
              <div class="spinner" />
            </div>
          </VCardText>
        </VCard>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped lang="scss">
.loading-spinner {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: inherit;
}

.spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border-left-color: #4caf50;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
