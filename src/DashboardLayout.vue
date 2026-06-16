<template>
  <VNavigationDrawer v-model="drawer" location="right" temporary>
    <VList>
      <VListItem
        v-for="item in navItems"
        :key="item.title"
        :to="item.to"
        :prepend-icon="item.icon"
        :title="item.title"
      />
    </VList>
  </VNavigationDrawer>

  <VAppBar>
    <VAppBarTitle class="text-h6">
      <b>نظام إدارة الحضور</b>
    </VAppBarTitle>
    <VSpacer />
    <VBtn icon @click="toggleTheme">
      <VIcon>{{ isDark ? 'tabler-sun' : 'tabler-moon' }}</VIcon>
    </VBtn>
    <VBtn icon @click="drawer = !drawer">
      <VIcon>tabler-menu-2</VIcon>
    </VBtn>
  </VAppBar>

  <VMain>
    <RouterView v-slot="{ Component, route }">
      <ErrorBoundary :key="route.fullPath">
        <Component :is="Component" />
      </ErrorBoundary>
    </RouterView>
  </VMain>
</template>

<script setup>
import { ref } from 'vue'
import { useTheme } from 'vuetify'
import navItems from '@/navigation/vertical'
import ErrorBoundary from '@core/components/ErrorBoundary.vue'

const drawer = ref(false)
const theme = useTheme()
const isDark = ref(false)

const toggleTheme = () => {
  isDark.value = !isDark.value
  theme.global.name.value = isDark.value ? 'dark' : 'light'
}
</script>
