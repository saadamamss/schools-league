<script setup>
import VueApexCharts from "vue3-apexcharts";
import { onMounted, ref } from "vue";

const chartOptions = ref({
  chart: {
    chart: {
      id: "vuechart-example",
    },
    xaxis: {
      categories: [1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998],
    },
  },
  series: [
    {
      name: "series-1",
      data: [30, 40, 35, 50, 49, 60, 70, 91],
    },
  ],
});
</script>

<template>
  <VueApexCharts
    width="500"
    type="bar"
    :options="chartOptions.chart"
    :series="chartOptions.series"
  ></VueApexCharts>
</template>
