<script setup lang="ts">
  import StatsWidgetGrid from '@/components/StatsWidgetGrid.vue'
  import StatsEvents from '@/components/icons/stats-events.vue'
  import StatsAttendance from '@/components/icons/stats-attendance.vue'
  import StatsAbsence from '@/components/icons/stats-absence.vue'
  import StatsUsers from '@/components/icons/stats-users.vue'
  import type { Location, SitesWidgetData } from '@/types/models'
  import { computed } from 'vue'

  const props = defineProps<{
    isLoading?: boolean
    data?: SitesWidgetData | null
    sites?: Location[]
  }>()

  const widgets = computed(() => [
    { title: 'عدد المواقع النشطة', value: props.data?.total_locations ?? 0, icon: StatsEvents },
    { title: 'عدد الحضور اليومي', value: props.data?.total_attendance ?? 0, icon: StatsAttendance },
    { title: 'عدد الغياب اليومي', value: props.data?.total_absence ?? 0, icon: StatsAbsence },
    { title: 'إجمالي المستخدمين المسجلين', value: props.data?.total_users ?? 0, icon: StatsUsers },
  ])
</script>
<template>
  <StatsWidgetGrid :is-loading="isLoading ?? false" :widgets="widgets" />
</template>
