// Utilities
import { defineStore } from "pinia";
import { ref } from "vue";

export const useAppStore = defineStore("app", () => {
  const appBreadcrumb = ref();
  const appPageTitle = ref("");

  // Add snackbar state
  const snackbar = ref({
    show: false,
    message: "",
    color: "success",
    timeout: 3000,
  });

  const setAppBreadcrumb = (breadcrumb = [], isBaseRoute = false) => {
    const baseHomeRoute = [{ label: $translate("home"), to: "/" }];

    if (isBaseRoute) {
      appBreadcrumb.value = [...baseHomeRoute];
      return;
    }

    appBreadcrumb.value = [...baseHomeRoute, ...breadcrumb];
  };

  const updateAppPageTitle = (pageTitle = "") => {
    appPageTitle.value = pageTitle;
  };

  // Add showSnackbar method
  const showSnackbar = ({ message, color = "success", timeout = 3000 }) => {
    snackbar.value = {
      show: true,
      message,
      color,
      timeout,
    };
  };

  return {
    appBreadcrumb,
    setAppBreadcrumb,
    appPageTitle,
    updateAppPageTitle,
    snackbar,
    showSnackbar,
  };
});
