<script setup>
const chartOptions = {
  colors: ["#00758D"],
  chart: {
    height: 350,
    type: "radialBar",
  },
  plotOptions: {
    radialBar: {
      dataLabels: {
        name: {
          fontSize: "20px",
          fontFamily: "29LTKaff-Light, Tahoma, sans-serif",
          color: "#797979",
        },
        value: {
          fontSize: "22px",
          fontFamily: "29LTKaff-Light, Tahoma, sans-serif",
          fontWeight: 600,
          color: "#00758D",
        },
        total: {
          show: false,
        },
      },
      track: {
        background: "#F0F2F5",
      },
      // strokeCap: "round",
    },
  },
  labels: ["الأداء"],
};
</script>

<template>
  <v-card elevation="0" rounded="lg" class="px-4 pt-4 border">
    <div class="mb-4">
      <h2 class="text-lg">نسبة الاداء</h2>
    </div>

    <apexchart
      type="radialBar"
      height="325"
      :options="chartOptions"
      :series="[44]"
    />
  </v-card>
</template>
