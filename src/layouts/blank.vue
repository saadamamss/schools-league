<template>
  <div class="layout-wrapper layout-blank">
    <RouterView />
  </div>
</template>

<script setup lang="ts">
  import { useSkins } from '@core/composable/useSkins'

  const { injectSkinClasses } = useSkins()
  injectSkinClasses()
</script>

<style>
.layout-wrapper.layout-blank {
  flex-direction: column;
}
</style>
