<template>
  <div class="chart-container">
    <div
      :style="{
        width: width + 'px',
        height: height + 'px',
        position: 'relative',
        minWidth: width + 'px !important',
        maxWidth: width + 'px !important',
      }"
    >
      <canvas ref="chart"></canvas>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch, computed } from "vue";
import { watchDebounced } from "@vueuse/core";
import {
  BarController,
  BarElement,
  CategoryScale,
  Chart,
  LinearScale,
  Tooltip,
  Legend,
} from "chart.js";
import { useTheme } from "vuetify";

Chart.register(
  BarController,
  BarElement,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend
);

const theme = useTheme();

const isDark = computed(() => theme.name.value == "dark");

const props = defineProps({
  title: {
    type: String,
    default: "",
  },
  height: {
    type: Number,
    default: 500,
  },
  data: {
    type: Object,
    required: true,
    // validator: (value) => value.labels && value.dataset,
  },
  color: {
    type: String,
    default: "#c2cfcb",
  },
});

const width = computed(() => Math.max(props.data.labels.length * 70, 500));
const chartMaxYValue = computed(() => Math.max(...props.data.dataset) + 10);

const chart = ref(null);
const chartInstance = ref(null);

const defaultOptions = {
  responsive: false,
  maintainAspectRatio: false,
  width: width.value,
  height: props.height,
  grid: {
    display: false,
  },
  plugins: {
    legend: {
      display: false,
    },
    tooltip: {
      callbacks: {
        title: function (context) {
          // Your original data (replace with actual values)
          const attendanceData = props.data.attendance;
          const index = context[0].dataIndex;
          if (attendanceData.length) {
            return `${props.data.statNames[0]} : ${attendanceData[index]}`;
          }
        },
        label: () => {
          return [];
        },
        footer: (context) => {
          const absenceData = props.data.absence;
          const index = context[0].dataIndex;

          if (absenceData.length) {
            return `${props.data.statNames[1]} : ${absenceData[index]}`;
          }
        },
      },
      backgroundColor: isDark.value ? "rgb(6,11,11)" : "rgb(248,249,249)",
      titleColor: "rgb(41,122,101)",
      footerColor: "rgb(218,58,61)",
      borderColor: "#616161",
      cornerRadius: 8,
      borderDash: [],
      bodyFont: {
        family: "Tajawal, sans-serif",
      },
      footerFont: {
        family: "Tajawal, sans-serif",
        size: 14,
      },
      titleFont: {
        family: "Tajawal, sans-serif",
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
        LinearScale: true,
      },

      ticks: {
        font: {
          family: "Tajawal, sans-serif",
        },
      },
    },
    y: {
      position: "right",
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
        callback: function (value) {
          return Math.round(value); // Rounds to nearest integer
        },
        font: {
          family: "Tajawal, sans-serif",
        },
      },
    },
  },
};

onMounted(() => {
  renderChart();
});

onBeforeUnmount(() => {
  if (chartInstance.value) {
    // chartInstance.value.destroy();
  }
});

watchDebounced(
  () => props.data,
  () => {
    if (chartInstance.value) {
      chartInstance.value.destroy();
    }
    renderChart();
  },
  { deep: true, debounce: 100 }
);

function renderChart() {
  if (!chart.value) return;
  const ctx = chart.value.getContext("2d");

  // Explicitly set canvas dimensions
  chart.value.width = width.value;
  chart.value.height = props.height;

  // Store secondary data in chart instance for tooltip access
  const chartData = {
    labels: props.data.labels,
    datasets: [
      {
        label: "",
        data: props.data.dataset,
        backgroundColor: props.color,
        hoverBackgroundColor: "#000000",
        barThickness: 30,
        maxBarThickness: 30,
        borderWidth: 0,
        borderRadius: 40,
        borderSkipped: false,
      },
    ],
  };

  chartInstance.value = new Chart(ctx, {
    type: "bar",
    data: chartData,
    options: {
      ...defaultOptions,
      width: width.value,
      height: props.height,
    },
    plugins: [
      {
        id: "xAxisSeparators",
        afterDraw: (chart) => {
          const ctx = chart.ctx;
          const xAxis = chart.scales.x;
          const yAxis = chart.scales.y;

          ctx.strokeStyle = "#aaa";
          ctx.lineWidth = 1.5;
          ctx.setLineDash([3, 3]); // Dashed line

          xAxis.ticks.forEach((tick, index) => {
            const x = xAxis.getPixelForTick(index);
            ctx.beginPath();
            ctx.moveTo(x + 30, yAxis.bottom); // Start from bottom
            ctx.lineTo(x + 30, yAxis.bottom + 5); // Draw upward to top
            ctx.stroke();
          });
        },
      },
    ],
  });
}
</script>

<style>
@import url("https://fonts.googleapis.com/css2?family=Tajawal:wght@400;500;700&display=swap");
.chart-container {
  overflow: auto;
  max-width: 100%;
  max-height: 100%;
}
</style>
