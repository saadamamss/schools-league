<template>
  <div v-if="hasData" class="chart-container">
    <div
      :style="{
        width: width + 'px',
        height: height + 'px',
        position: 'relative',
        minWidth: width + 'px !important',
        maxWidth: width + 'px !important',
      }"
    >
      <canvas ref="chart" />
    </div>
  </div>
  <div v-else class="empty-state" :style="{ height: height + 'px' }">
    <p>لا توجد بيانات</p>
  </div>
</template>

<script setup lang="ts">
  import { computed, onBeforeUnmount, onBeforeUpdate, onMounted, ref, watch } from 'vue'
  import { watchDebounced } from '@vueuse/core'
  import {
    BarController,
    BarElement,
    CategoryScale,
    Chart,
    Legend,
    LinearScale,
    Tooltip,
  } from 'chart.js'
  import { useTheme } from 'vuetify'

  Chart.register(
    BarController,
    BarElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend
  )

  const theme = useTheme()

  const isDark = computed(() => theme.name.value === 'dark')

  const props = defineProps<{
    title?: string
    height?: number
    data: { labels: string[]; dataset: number[]; attendance?: number[]; absence?: number[]; statNames?: string[] }
    color?: string
  }>()

  const width = computed(() => Math.max((props.data?.labels?.length ?? 0) * 70, 500))
  const chartMaxYValue = computed(() => {
    if (!props.data?.dataset?.length) return 10
    return Math.max(...props.data.dataset) + 10
  })

  const chart = ref<HTMLCanvasElement | null>(null)
  const chartInstance = ref<Chart<'bar'> | null>(null)
  const isChartMounted = ref(true)

  const hasData = computed(() => !!(props.data?.labels?.length && props.data?.dataset?.length))

  // Destroy chart when there's no data to render (avoids Chart.js animation loop
  // crashing on a removed canvas when v-if flips to v-else)
  watch(hasData, newVal => {
    if (!newVal && chartInstance.value) {
      chartInstance.value.destroy()
      chartInstance.value = null
    }
  })

  const defaultOptions = computed<any>(() => ({
    responsive: false,
    maintainAspectRatio: false,
    grid: {
      display: false,
    },
    plugins: {
      legend: {
        display: false,
      },
      tooltip: {
        callbacks: {
          title (context: any[]) {
            const attendanceData = props.data.attendance
            const index = context[0].dataIndex
            if (attendanceData?.length) {
              return `${props.data.statNames?.[0] ?? ''} : ${attendanceData[index]}`
            }
          },
          label: () => {
            return []
          },
          footer: (context: any[]) => {
            const absenceData = props.data.absence
            const index = context[0].dataIndex

            if (absenceData?.length) {
              return `${props.data.statNames?.[1] ?? ''} : ${absenceData[index]}`
            }
          },
        },
        backgroundColor: isDark.value ? 'rgb(6,11,11)' : 'rgb(248,249,249)',
        titleColor: 'rgb(41,122,101)',
        footerColor: 'rgb(218,58,61)',
        borderColor: 'rgba(var(--v-theme-on-surface), 0.38)',
        cornerRadius: 8,
        borderDash: [],
        bodyFont: {
          family: 'Tajawal, sans-serif',
        },
        footerFont: {
          family: 'Tajawal, sans-serif',
          size: 14,
        },
        titleFont: {
          family: 'Tajawal, sans-serif',
          size: 14,
        },
        padding: {
          top: 8,
          right: 12,
          bottom: 4,
          left: 12,
        },
      },
    },
    scales: {
      x: {
        grid: {
          display: false,
          offset: false,
          drawBorder: false,
        },
        ticks: {
          font: {
            family: 'Tajawal, sans-serif',
          },
        },
      },
      y: {
        position: 'right',
        max: chartMaxYValue.value,
        beginAtZero: true,
        border: {
          display: false,
        },
        grid: {
          display: false,
          drawBorder: false,
        },
        ticks: {
          callback (value: string | number) {
            return Math.round(Number(value))
          },
          font: {
            family: 'Tajawal, sans-serif',
          },
        },
      },
    },
  }))

  onBeforeUnmount(() => {
    isChartMounted.value = false
    if (chartInstance.value) {
      chartInstance.value.destroy()
      chartInstance.value = null
    }
  })

  // Destroy chart BEFORE Vue updates the DOM — prevents Chart.js's
  // requestAnimationFrame from crashing on a removed canvas (v-if flip)
  onBeforeUpdate(() => {
    if (chartInstance.value) {
      chartInstance.value.destroy()
      chartInstance.value = null
    }
  })

  // Initial render after mount (canvas is guaranteed available then)
  onMounted(() => {
    renderChart()
  })

  watchDebounced(
    () => props.data,
    () => {
      // Component was unmounted before this debounced callback fired
      if (!isChartMounted.value) return
      if (chartInstance.value) {
        chartInstance.value.destroy()
        chartInstance.value = null
      }
      renderChart()
    },
    { deep: true, debounce: 100 }
  )

  function renderChart () {
    if (!chart.value) return
    if (!chart.value.isConnected) return
    if (!props.data?.labels?.length || !props.data?.dataset?.length) return
    const ctx = chart.value.getContext('2d')
    if (!ctx) return

    // Explicitly set canvas dimensions
    chart.value.width = width.value
    chart.value.height = props.height ?? 500

    // Store secondary data in chart instance for tooltip access
    const chartData = {
      labels: props.data.labels,
      datasets: [
        {
          label: '',
          data: props.data.dataset,
          backgroundColor: props.color,
          hoverBackgroundColor: '#1a1a1a',
          barThickness: 30,
          maxBarThickness: 30,
          borderWidth: 0,
          borderRadius: 40,
          borderSkipped: false,
        },
      ],
    }

    try {
      chartInstance.value = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: {
          ...defaultOptions.value,
        },
        plugins: [
          {
            id: 'xAxisSeparators',
            afterDraw: chart => {
              const ctx = chart.ctx
              const xAxis = chart.scales.x
              const yAxis = chart.scales.y

              ctx.strokeStyle = 'rgba(var(--v-theme-on-surface), 0.38)'
              ctx.lineWidth = 1.5
              ctx.setLineDash([3, 3])

              xAxis.ticks.forEach((tick, index) => {
                const x = xAxis.getPixelForTick(index)
                ctx.beginPath()
                ctx.moveTo(x + 30, yAxis.bottom) // Start from bottom
                ctx.lineTo(x + 30, yAxis.bottom + 5) // Draw upward to top
                ctx.stroke()
              })
            },
          },
        ],
      })
    } catch (e: any) {
      // Canvas was detached during Chart construction (e.g. route change mid-render)
      chartInstance.value = null
    }
  }
</script>

<style>
.chart-container {
  overflow: auto;
  max-width: 100%;
  max-height: 100%;
}

.empty-state {
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(var(--v-theme-on-surface), 0.6);
  font-size: 1rem;
}
</style>
