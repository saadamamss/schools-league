/**
 * Safari Chart Compatibility Fixes
 * This utility provides fixes for ApexCharts rendering issues in Safari browser
 * Specifically for Arabic text rendering in x-axis labels
 */

// Detect Safari browser
export const isSafari = () => {
  return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
};

// Detect WebKit-based browsers (Safari, Chrome on iOS)
export const isWebKit = () => {
  return /webkit/i.test(navigator.userAgent);
};

// Get Safari-specific chart options - focus on Arabic text
export const getSafariChartOptions = (baseOptions = {}) => {
  const safari = isSafari();

  return {
    ...baseOptions,
    chart: {
      ...baseOptions.chart,
      animations: {
        enabled: !safari,
        speed: 300,
        animateGradually: {
          enabled: !safari,
        },
        dynamicAnimation: {
          enabled: !safari,
        },
      },
      redrawOnWindowResize: true,
      redrawOnParentResize: true,
    },
    xaxis: {
      ...baseOptions.xaxis,
      labels: {
        ...baseOptions.xaxis?.labels,
        trim: false,
        maxHeight: 60,
        hideOverlappingLabels: false,
        rotateAlways: false,
        showDuplicates: false,
        formatter: function (value, opts) {
          // Ensure proper Arabic text rendering in Safari
          const text = String(value);
          // Remove any extra spaces that might cause letter separation
          return text.replace(/\s+/g, " ").trim();
        },
        style: {
          ...baseOptions.xaxis?.labels?.style,
          // Force Arabic font and RTL direction
          fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
          direction: "rtl",
          textAlign: "right",
        },
      },
      // tickPlacement: "on",
      crosshairs: {
        show: false,
      },
    },
  };
};

// Apply Safari fixes to chart after mounting - Arabic text focus
export const applySafariChartFixes = (chartRef, options = {}) => {
  if (!isSafari() || !chartRef) return;

  setTimeout(() => {
    if (chartRef.chart) {
      chartRef.chart.updateOptions(
        {
          chart: {
            animations: {
              enabled: false,
            },
          },
          xaxis: {
            labels: {
              style: {
                fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
                direction: "rtl",
                textAlign: "right",
              },
            },
          },
          ...options,
        },
        false,
        true
      );
    }
  }, 100);
};

// Force chart redraw for Safari
export const forceChartRedraw = (chartRef) => {
  if (!isSafari() || !chartRef) return;

  setTimeout(() => {
    if (chartRef.chart) {
      chartRef.chart.render();
    }
  }, 50);
};

// Get responsive options for Safari - Arabic text focus
export const getSafariResponsiveOptions = () => {
  return [
    {
      breakpoint: 480,
      options: {
        xaxis: {
          labels: {
            rotate: -90,
            maxHeight: 80,
            fontSize: "10px",
            style: {
              fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
              direction: "rtl",
              textAlign: "right",
            },
          },
        },
      },
    },
    {
      breakpoint: 768,
      options: {
        xaxis: {
          labels: {
            rotate: -45,
            maxHeight: 60,
            fontSize: "11px",
            style: {
              fontFamily: "'IBMPlexArabic', Tahoma, sans-serif",
              direction: "rtl",
              textAlign: "right",
            },
          },
        },
      },
    },
  ];
};

// CSS classes for Safari fixes
export const safariChartClasses = {
  canvas: "safari-chart-canvas",
  xaxisLabel: "safari-xaxis-label",
  rtlLabel: "safari-rtl-label",
};

// Apply CSS fixes to chart container - Arabic text focus
export const applySafariCSSFixes = (container) => {
  if (!isSafari() || !container) return;

  const canvas = container.querySelector(".apexcharts-canvas");
  if (canvas) {
    canvas.classList.add(safariChartClasses.canvas);
  }

  const xaxisLabels = container.querySelectorAll(".apexcharts-xaxis-label");
  xaxisLabels.forEach((label) => {
    label.classList.add(safariChartClasses.xaxisLabel);

    // Apply Arabic text fixes
    label.style.fontFamily = "'IBMPlexArabic', Tahoma, sans-serif";
    label.style.direction = "rtl";
    label.style.textAlign = "right";
    label.style.unicodeBidi = "bidi-override";
    label.style.letterSpacing = "normal";
    label.style.wordSpacing = "normal";

    if (label.getAttribute("dir") === "rtl") {
      label.classList.add(safariChartClasses.rtlLabel);
    }
  });
};

// Initialize Safari chart fixes - Arabic text focus
export const initSafariChartFixes = (chartRef, container) => {
  if (!isSafari()) return;

  // Apply CSS fixes
  if (container) {
    applySafariCSSFixes(container);
  }

  // Apply chart fixes
  applySafariChartFixes(chartRef);

  // Force redraw
  forceChartRedraw(chartRef);
};

export default {
  isSafari,
  isWebKit,
  getSafariChartOptions,
  applySafariChartFixes,
  forceChartRedraw,
  getSafariResponsiveOptions,
  safariChartClasses,
  applySafariCSSFixes,
  initSafariChartFixes,
};
