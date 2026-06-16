/**
 * Safari Chart Plugin
 * Automatically applies Safari-specific fixes to all ApexCharts components
 */

import { isSafari, initSafariChartFixes } from "@/utils/safariChartFix";

export default {
  install(app) {
    // Only apply fixes if Safari is detected
    if (!isSafari()) return;

    // Global mixin to apply Safari fixes to all components with charts
    app.mixin({
      mounted() {
        // Check if this component has ApexCharts
        this.$nextTick(() => {
          this.applySafariChartFixes();
        });
      },
      updated() {
        // Re-apply fixes when component updates
        this.$nextTick(() => {
          this.applySafariChartFixes();
        });
      },
      methods: {
        applySafariChartFixes() {
          // Find all ApexCharts components in this component
          const chartRefs = this.$refs
            ? Object.keys(this.$refs).filter(
                (key) =>
                  this.$refs[key] &&
                  (this.$refs[key].$options?.name === "apexchart" ||
                    this.$refs[key].chart ||
                    this.$refs[key].$el?.classList?.contains(
                      "apexcharts-canvas"
                    ))
              )
            : [];

          chartRefs.forEach((refKey) => {
            const chartRef = this.$refs[refKey];
            const container = this.$el;

            if (chartRef) {
              initSafariChartFixes(chartRef, container);
            }
          });

          // Also check for any ApexCharts canvases in the DOM
          const canvases =
            this.$el?.querySelectorAll?.(".apexcharts-canvas") || [];
          canvases.forEach((canvas) => {
            canvas.classList.add("safari-chart-canvas");

            // Apply fixes to x-axis labels
            const xaxisLabels = canvas.querySelectorAll(
              ".apexcharts-xaxis-label"
            );
            xaxisLabels.forEach((label) => {
              label.classList.add("safari-xaxis-label");
              if (label.getAttribute("dir") === "rtl") {
                label.classList.add("safari-rtl-label");
              }
            });
          });
        },
      },
    });

    // Global directive for chart containers
    app.directive("safari-chart", {
      mounted(el) {
        if (!isSafari()) return;

        // Add Safari-specific classes
        el.classList.add("safari-chart-container");

        // Apply fixes to any charts within this element
        const observer = new MutationObserver(() => {
          const canvases = el.querySelectorAll(".apexcharts-canvas");
          canvases.forEach((canvas) => {
            canvas.classList.add("safari-chart-canvas");

            const xaxisLabels = canvas.querySelectorAll(
              ".apexcharts-xaxis-label"
            );
            xaxisLabels.forEach((label) => {
              label.classList.add("safari-xaxis-label");
              if (label.getAttribute("dir") === "rtl") {
                label.classList.add("safari-rtl-label");
              }
            });
          });
        });

        observer.observe(el, {
          childList: true,
          subtree: true,
        });

        // Store observer for cleanup
        el._safariObserver = observer;
      },
      unmounted(el) {
        // Cleanup observer
        if (el._safariObserver) {
          el._safariObserver.disconnect();
        }
      },
    });

    // Global property to check if Safari
    app.config.globalProperties.$isSafari = isSafari;

    // Global method to apply chart fixes
    app.config.globalProperties.$applySafariChartFixes = initSafariChartFixes;
  },
};
