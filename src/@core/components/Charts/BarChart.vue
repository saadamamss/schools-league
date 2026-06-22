<script setup lang="ts">
  import { useTheme } from 'vuetify'
  import BarChart from '@/@core/libs/chartjs/components/BarChart'
  import { getLatestBarChartConfig } from '@core/libs/chartjs/chartjsConfig'

  const props = defineProps<{
    colors: Record<string, any>
    chartData: any[]
  }>()

  const vuetifyTheme = useTheme()
  const chartOptions = computed(() =>
    getLatestBarChartConfig(vuetifyTheme.current.value)
  )

  interface Dataset {
    maxBarThickness: number
    backgroundColor: string
    borderColor: string
    borderRadius: { topRight: number; topLeft: number }
    data: number[]
  }

  const Data = reactive<{
    labels: string[]
    datasets: Dataset[]
  }>({
    labels: [
      'يناير',
      'فبراير',
      'مارس',
      'إبريل',
      'مايو',
      'يونيو',
      'يوليو',
      'أغسطس',
      'سبتمبر',
      'أكتوبر',
      'نوفمبر',
      'ديسمبر',
    ],
    datasets: [
      {
        maxBarThickness: 15,
        backgroundColor: props.colors.primary,
        borderColor: 'transparent',
        borderRadius: {
          topRight: 15,
          topLeft: 15,
        },
        data: [],
      },
    ],
  })

  onUpdated(() => {
    const data: any[] = []
    props.chartData.forEach((ctd: any) => {
      data.push(ctd.count)
    })
    Data.datasets[0].data = data
  })

  const data = {
    labels: [
      'يناير',
      'فبراير',
      'مارس',
      'إبريل',
      'مايو',
      'يونيو',
      'يوليو',
      'أغسطس',
      'سبتمبر',
      'أكتوبر',
      'نوفمبر',
      'ديسمبر',
    ],
    datasets: [
      {
        maxBarThickness: 15,
        backgroundColor: props.colors.primary,
        borderColor: 'transparent',
        borderRadius: {
          topRight: 15,
          topLeft: 15,
        },
        data: [],
      },
    ],
  }
</script>

<template>
  <VCard height="400" :loading="!props.chartData">
    <BarChart :chart-data="Data" :chart-options="chartOptions" :height="300" />
  </VCard>
</template>
