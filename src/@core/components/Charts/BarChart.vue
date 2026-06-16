<script setup>
import { useTheme } from "vuetify";
import BarChart from "@/@core/libs/chartjs/components/BarChart";
import { getLatestBarChartConfig } from "@core/libs/chartjs/chartjsConfig";

const props = defineProps({
  colors: {
    type: null,
    required: true,
  },
  chartData: {
    type: Array,
    required: true,
  },
});

const vuetifyTheme = useTheme();
const chartOptions = computed(() =>
  getLatestBarChartConfig(vuetifyTheme.current.value)
);

const Data = reactive({
  labels: [
    "يناير",
    "فبراير",
    "مارس",
    "إبريل",
    "مايو",
    "يونيو",
    "يوليو",
    "أغسطس",
    "سبتمبر",
    "أكتوبر",
    "نوفمبر",
    "ديسمبر",
  ],
  datasets: [
    {
      maxBarThickness: 15,
      backgroundColor: props.colors.primary,
      borderColor: "transparent",
      borderRadius: {
        topRight: 15,
        topLeft: 15,
      },
      data: [],
    },
  ],
});

onUpdated(() => {
  let data = [];
  props.chartData.map((ctd) => {
    // Data.labels.push(ctd.month)
    data.push(ctd.count);
  });
  Data.datasets[0].data = data;
});

const data = {
  labels: [
    "يناير",
    "فبراير",
    "مارس",
    "إبريل",
    "مايو",
    "يونيو",
    "يوليو",
    "أغسطس",
    "سبتمبر",
    "أكتوبر",
    "نوفمبر",
    "ديسمبر",
  ],
  datasets: [
    {
      maxBarThickness: 15,
      backgroundColor: props.colors.primary,
      borderColor: "transparent",
      borderRadius: {
        topRight: 15,
        topLeft: 15,
      },
      data: [],
    },
  ],
};
</script>

<template>
  <VCard :loading="!props.chartData" height="400">
    <BarChart :height="300" :chart-data="Data" :chart-options="chartOptions" />
  </VCard>
</template>
