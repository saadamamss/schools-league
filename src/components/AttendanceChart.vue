<template>
  <div
    dir="rtl"
    class="py-2"
    style="overflow-x: auto"
    ref="chartContainer"
    v-safari-chart
  >
    <apexchart
      type="bar"
      :height="height"
      :width="(attendanceData.length - 1) * 2 * (32 + 10)"
      :options="chartOptions"
      :series="series"
      dir="rtl"
      v-show="hasData"
      ref="chartRef"
    ></apexchart>
    <div v-else class="no-data">لا يوجد بيانات متاحة</div>
  </div>
</template>

<script>
import VueApexCharts from "vue3-apexcharts";
import {
  isSafari,
  getSafariChartOptions,
  initSafariChartFixes,
} from "@/utils/safariChartFix";

export default {
  name: "AttendanceChart",
  components: {
    apexchart: VueApexCharts,
  },
  props: {
    height: { type: Number, default: 250 },
    attendanceData: {
      type: Array,
      required: true,
      default: () => [],
      validator: (data) => {
        return (
          Array.isArray(data) &&
          data.every(
            (item) =>
              item &&
              typeof item.attended === "number" &&
              typeof item.absent === "number"
          )
        );
      },
    },
  },
  mounted() {
    // Apply Safari-specific fixes after chart is mounted
    this.$nextTick(() => {
      if (isSafari()) {
        initSafariChartFixes(this.$refs.chartRef, this.$refs.chartContainer);
      }
    });
  },
  computed: {
    hasData() {
      return this.attendanceData && this.attendanceData.length > 0;
    },
    series() {
      return [
        {
          name: "Students",
          data: this.attendanceData,
        },
      ];
    },
    chartOptions() {
      const baseOptions = {
        chart: {
          width: "100%",
          type: "bar",
          defaultLocale: "en",
          toolbar: {
            show: false,
          },
        },
        grid: {
          show: false,
        },
        colors: ["#297A6542", "#E91E63", "#9C27B0"],
        plotOptions: {
          bar: {
            borderRadius: 16,
            horizontal: false,
            columnWidth: 32,
            colors: {
              ranges: [
                {
                  from: 0,
                  to: 10000,
                  colors: ["#297A6542", "#E91E63", "#9C27B0"],
                },
              ],
            },
          },
        },
        xaxis: {
          type: "category",
          reversed: true,
          opposite: true,
          labels: {
            rotate: -45,
            style: {
              colors: "#929A9F",
              fontSize: "12px",
              fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
              cssClass: "text-right",
              // Arabic text specific styles
              direction: "rtl",
              textAlign: "right",
            },
            // Arabic text formatter
            formatter: function (value) {
              // Ensure proper Arabic text rendering
              const text = String(value);
              return text.replace(/\s+/g, " ").trim();
            },
            tick: {},
          },
          axisBorder: {
            show: false,
          },
        },
        yaxis: {
          opposite: true,
          labels: {
            formatter: function (val) {
              return Math.round(val);
            },
            style: {
              colors: "#929A9F",
              fontSize: "12px",
              fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
            },
          },
        },
        tooltip: {
          custom: ({ series, seriesIndex, dataPointIndex, w }) => {
            const data =
              w.globals.initialSeries[seriesIndex].data[dataPointIndex];

            const attended = data.attended;
            const absent = data.absent;

            return `
                <div class="shadow-sm px-3 py-2 bg-surface">
                  ${
                    attended
                      ? `
                  <span class="d-block mb-2">
                        <b class="text-primary">${attended} حضور</b>
                    </span>
                  `
                      : ``
                  }
                  ${
                    absent
                      ? `<span class="d-block">
                        <b class="text-error">${absent} غياب</b>
                    </span> `
                      : ``
                  }

                </div>
            `;
          },
        },
        dataLabels: {
          enabled: false, // Disable all data labels
        },
        legend: {
          horizontalAlign: "right",
        },
      };

      // Apply Safari-specific options
      return getSafariChartOptions(baseOptions);
    },
  },
};
</script>

<style scoped>
.no-data {
  padding: 20px;
  text-align: center;
  color: #666;
  font-style: italic;
}

:deep(.apexcharts-bar-area):hover {
  fill: #297a65 !important;
}

/* Safari-specific styles - Arabic text focus */
:deep(.apexcharts-canvas) {
  /* -webkit-transform: translateZ(0); */
  /* transform: translateZ(0); */
}

:deep(.apexcharts-xaxis-label) {
  /* -webkit-transform: translateZ(0); */
  /* transform: translateZ(0); */
  -webkit-backface-visibility: hidden;
  backface-visibility: hidden;

  /* Arabic text specific fixes */
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  direction: rtl !important;
  text-align: right !important;
  unicode-bidi: bidi-override !important;
  letter-spacing: normal !important;
  word-spacing: normal !important;
  white-space: nowrap !important;
}

/* Force proper rendering for RTL text in Safari - Arabic specific */
:deep(.apexcharts-xaxis-label[dir="rtl"]) {
  text-anchor: end !important;
  -webkit-text-anchor: end !important;
  direction: rtl !important;
  unicode-bidi: bidi-override !important;
  text-align: right !important;
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  letter-spacing: normal !important;
  word-spacing: normal !important;
}

/* Additional Safari fixes for chart elements */
:deep(.safari-chart-canvas) {
  /* -webkit-transform: translate3d(0, 0, 0); */
  /* transform: translate3d(0, 0, 0); */
  -webkit-backface-visibility: hidden;
  backface-visibility: hidden;
}

:deep(.safari-xaxis-label) {
  /* -webkit-transform: translateZ(0); */
  /* transform: translateZ(0); */
  /* will-change: transform; */

  /* Arabic text specific fixes */
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  direction: rtl !important;
  text-align: right !important;
  unicode-bidi: bidi-override !important;
  letter-spacing: normal !important;
  word-spacing: normal !important;
}

:deep(.safari-rtl-label) {
  text-anchor: end !important;
  -webkit-text-anchor: end !important;
  direction: rtl !important;
  unicode-bidi: bidi-override !important;
  text-align: right !important;
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  letter-spacing: normal !important;
  word-spacing: normal !important;
}
</style>
