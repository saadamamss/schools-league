<script setup lang="ts">
  import ThemeSwitcher from '@/@core/components/ThemeSwitcher.vue'
  const themes = [
    {
      name: 'light',
      icon: 'tabler-sun-high',
    },
    {
      name: 'dark',
      icon: 'tabler-moon',
    },
  ]
</script>

<template>
  <ThemeSwitcher :themes="themes" />
</template>
