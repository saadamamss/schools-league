<script setup lang="ts">
  import { useSkins } from '@core/composable/useSkins'
  import { useThemeConfig } from '@core/composable/useThemeConfig'

  import { AppContentLayoutNav } from '@layouts/enums'

  const DefaultLayoutWithVerticalNav = defineAsyncComponent(() =>
    import('./components/DefaultLayoutWithVerticalNav.vue')
  )
  const { width: windowWidth } = useWindowSize()
  const { appContentLayoutNav, switchToVerticalNavOnLtOverlayNavBreakpoint } =
    useThemeConfig()

  const isShow = ref(false)

  // Remove below composable usage if you are not using horizontal nav layout in your app
  switchToVerticalNavOnLtOverlayNavBreakpoint(windowWidth.value)

  const { layoutAttrs, injectSkinClasses } = useSkins()

  injectSkinClasses()
</script>

<template>
  <div>
    <DefaultLayoutWithVerticalNav v-bind="layoutAttrs" />
  </div>
</template>

<style lang="scss">
// As we are using `layouts` plugin we need its styles to be imported
@use "@layouts/styles/default-layout";
</style>
