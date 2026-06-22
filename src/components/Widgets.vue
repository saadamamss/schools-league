<script setup lang="ts">
  import StatsWidgetGrid from '@/components/StatsWidgetGrid.vue'
  import StatsUsers from '@/components/icons/stats-users.vue'
  import StatsAttendance from '@/components/icons/stats-attendance.vue'
  import StatsEvents from '@/components/icons/stats-events.vue'
  import StatsHours from '@/components/icons/stats-hours.vue'
  import type { DashboardStats } from '@/types/models'
  import { computed } from 'vue'

  const props = defineProps<{ isLoading: boolean; data: DashboardStats | null }>()

  const widgets = computed(() => [
    { title: 'إجمالي المستخدمين المسجلين', value: props.data?.total_users ?? 0, icon: StatsUsers },
    { title: 'عدد الحضور اليومي', value: props.data?.daily_attendance ?? 0, icon: StatsAttendance },
    { title: 'عدد المواقع النشطة', value: props.data?.active_locations ?? 0, icon: StatsEvents },
    { title: 'ساعة هذا الأسبوع', value: Number(props.data?.weekly_hours ?? 0), icon: StatsHours },
  ])
</script>
<template>
  <StatsWidgetGrid :is-loading="isLoading" :widgets="widgets" />
</template>
