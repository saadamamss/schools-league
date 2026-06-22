import { CategoryScale, Chart as ChartJS, Legend, LinearScale, LineElement, PointElement, Title, Tooltip } from 'chart.js'
import { defineComponent, h } from 'vue'
import { Line } from 'vue-chartjs'

ChartJS.register(Title, Tooltip, Legend, LineElement, LinearScale, PointElement, CategoryScale)
export default defineComponent({
  name: 'LineChart',
  props: {
    chartId: { type: String, default: 'line-chart' },
    width: { type: Number, default: 400 },
    height: { type: Number, default: 400 },
    cssClasses: { default: '', type: String },
    styles: { type: Object, default: () => ({}) },
    plugins: { type: Array, default: () => [] },
    chartData: { type: Object, default: () => ({}) },
    chartOptions: { type: Object, default: () => ({}) },
  },
  setup (props: Record<string, unknown>) {
    return () => h(h(Line as never), {
      chartId: props.chartId,
      width: props.width,
      height: props.height,
      cssClasses: props.cssClasses,
      styles: props.styles,
      plugins: props.plugins,
      chartOptions: props.chartOptions,
      chartData: props.chartData,
    })
  },
})
