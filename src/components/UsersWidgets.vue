<script setup lang="ts">
  import StatsWidgetGrid from '@/components/StatsWidgetGrid.vue'
  import StatsUsers from '@/components/icons/stats-users.vue'
  import StatsAttendance from '@/components/icons/stats-attendance.vue'
  import StatsAbsence from '@/components/icons/stats-absence.vue'
  import StatsHours from '@/components/icons/stats-hours.vue'
  import type { DashboardStats } from '@/types/models'
  import { computed } from 'vue'

  const props = defineProps<{ isLoading: boolean; data: DashboardStats | null }>()

  const widgets = computed(() => [
    { title: 'إجمالي المستخدمين المسجلين', value: props.data?.total_users ?? 0, icon: StatsUsers },
    { title: 'عدد الحضور اليومي', value: props.data?.total_attendance ?? 0, icon: StatsAttendance },
    { title: 'عدد الغياب اليومى', value: props.data?.total_absence ?? 0, icon: StatsAbsence },
    { title: 'ساعة هذا الأسبوع', value: Number(props.data?.total_hours ?? 0), icon: StatsHours },
  ])
</script>
<template>
  <StatsWidgetGrid :is-loading="isLoading" :widgets="widgets" />
</template>
