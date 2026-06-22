<script setup lang="ts">
  import { useAuthStore } from '@/stores/auth'
  import { useThemeConfig } from '@core/composable/useThemeConfig'
  import { hexToRgb } from '@layouts/utils'
  import { useTheme } from 'vuetify'
  import { useAppStore } from '@/stores/app'

  const authStore = useAuthStore()
  const appStore = useAppStore()

  const {
    syncInitialLoaderTheme,
    syncVuetifyThemeWithTheme: syncConfigThemeWithVuetifyTheme,
  } = useThemeConfig()

  const { global } = useTheme()

  // ℹ️ Sync current theme with initial loader theme
  syncInitialLoaderTheme()
  syncConfigThemeWithVuetifyTheme()

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
/* All other global styles live in src/styles/styles.scss */
</style>
