<template>
  <v-btn icon @click="toggleTheme">
    <v-icon :icon="isDark ? 'mdi-weather-sunny' : 'mdi-weather-night'" />
  </v-btn>
</template>

<script setup lang="ts">
  import { useTheme } from 'vuetify'
  import { onMounted, ref } from 'vue'

  const theme = useTheme()
  const isDark = ref(false)

  // Initialize from localStorage
  onMounted(() => {
    const savedTheme = localStorage.getItem('darkTheme')
    if (savedTheme !== null) {
      isDark.value = savedTheme === 'dark'
      theme.change(isDark.value ? 'dark' : 'light')
    }
  })

  const toggleTheme = () => {
    isDark.value = !isDark.value
    theme.change(isDark.value ? 'dark' : 'light')
    localStorage.setItem('darkTheme', isDark.value ? 'dark' : 'light')
  }
</script>
