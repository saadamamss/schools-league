<script setup>
import { useAuthStore } from "@/stores/Auth";
import { useThemeConfig } from "@core/composable/useThemeConfig";
import { hexToRgb } from "@layouts/utils";
import { useTheme } from "vuetify";
import { useAppStore } from "@/stores/app";
import "vue3-toastify/dist/index.css";

const authStore = useAuthStore();
const appStore = useAppStore();

const {
  syncInitialLoaderTheme,
  syncVuetifyThemeWithTheme: syncConfigThemeWithVuetifyTheme,
} = useThemeConfig();

const { global } = useTheme();

// ℹ️ Sync current theme with initial loader theme
syncInitialLoaderTheme();
syncConfigThemeWithVuetifyTheme();

onMounted(() => {
  // if(localStorage.getItem("SAR_TOKEN")) {
  //    const user = JSON.parse(localStorage.getItem("SAR_TOKEN"));
  //    authStore.updateUser(user);
  //  }
});
</script>

<template>
  <VLocaleProvider rtl>
    <VApp
      :style="`--v-global-theme-primary: ${hexToRgb(
        global.current.value.colors.primary
      )}`"
    >
      <RouterView />

      <!-- Add Snackbar -->
      <VSnackbar
        v-model="appStore.snackbar.show"
        :color="appStore.snackbar.color"
        :timeout="appStore.snackbar.timeout"
      >
        {{ appStore.snackbar.message }}
      </VSnackbar>
    </VApp>
  </VLocaleProvider>
</template>

<style>
@import "@layouts/styles/index.scss";

@font-face {
  font-family: "IBMPlexArabic";
  src: url("/fonts/IBMPlexArabic/IBMPlexArabic.ttf") format("truetype");
  font-weight: 400;
}

body {
  block-size: fill-available;
  font-family: "IBMPlexArabic", Tahoma, sans-serif;
}

body pre {
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  line-height: 2;
}

.whitespace-nowrap {
  white-space: nowrap;
}

.v-enter-active,
.v-leave-active {
  transition: opacity 0.5s ease;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.25s ease-out;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(30px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-30px);
}

.custom-panel .v-expansion-panel {
  background-color: #f8f9f9 !important;
}

.custom-panel .v-expansion-panel:not(:first-child)::after {
  border: none;
}

.custom-panel.v-expansion-panels--variant-accordion > .v-expansion-panel {
  border-radius: 1rem !important;
}

.v-select.custom-select,
.v-autocomplete.custom-select,
.v-input.v-filter-input {
  &.v-input--density-default .v-field--variant-filled {
    --v-input-control-height: 22px !important;
    --v-field-padding-bottom: 4px;
  }

  .v-field--variant-filled .v-field__outline::before,
  .v-field--variant-underlined .v-field__outline::before {
    border-width: 0;
  }

  .v-field__append-inner > .v-icon {
    background-color: rgba(0, 0, 0, 0.1);
    border-radius: 50%;
  }

  .v-field__input {
    padding: 12px 10px;
  }

  .v-field .v-field__input > input {
    top: 50%;
    transform: translateY(-50%);
    font-size: 0.9rem !important;
  }

  .v-select__selection-text {
    font-size: 0.9rem !important;
    color: #1b2559;
  }
}

body .v-btn--variant-outlined {
  border-color: #e9ebec;
}

.text-subtitle-2,
.whitespace-wrap {
  white-space: normal !important;
}

.v-input--density-compact .v-field--variant-filled {
  --v-input-control-height: 48px;
  --v-field-padding-bottom: 0px;
}

.responsive-text {
  font-size: 0.7rem; /* text-xs equivalent */

  @media (min-width: 1024px) {
    font-size: 1rem; /* larger on lg screens */
  }
}
.v-field__input[type="date"] {
  display: table !important;
}
body .v-card {
  overflow: initial !important;
}

body .v-date-picker-month__day--selected .v-btn {
  background-color: #667178 !important;
  color: rgb(var(--v-theme-on-surface-variant));
}
table tr td,
table tr th {
  white-space: nowrap;
}
.v-btn--size-small {
  height: 40px !important;
}
.v-slide-group__container {
  align-items: center;
}

.v-application {
  background-color: rgb(var(--v-theme-background)) !important;
}
</style>
