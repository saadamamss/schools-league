<script setup lang="ts">
  import navItems from '@/navigation/vertical'
  import { useThemeConfig } from '@core/composable/useThemeConfig'
  import { useAppStore } from '@/stores/app'

  import UserProfile from '@/layouts/components/UserProfile.vue'
  import ErrorBoundary from '@/@core/components/ErrorBoundary.vue'

  // @layouts plugin
  import { VerticalNavLayout } from '@layouts'
  import ToggleTheme from '@/components/ToggleTheme.vue'
  import { useAuthStore } from '@/stores/auth'

  const authStore = useAuthStore()
  const appStore = useAppStore()

  const { appRouteTransition, isLessThanOverlayNavBreakpoint } = useThemeConfig()
  const { width: windowWidth } = useWindowSize()

  const user = computed(() => (authStore.user as any)?.user ?? {})
</script>

<template>
  <VerticalNavLayout :nav-items="navItems">
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center ga-3 text-dark">
        <VBtn
          v-if="isLessThanOverlayNavBreakpoint(windowWidth)"
          class="ms-n3"
          color="default"
          icon
          size="small"
          variant="text"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="mdi-menu" size="24" />
        </VBtn>
        <!-- search box -->
        <v-btn
          class="btn-icon d-sm-none"
          color="default"
          elevation="0"
          size="small"
        >
          <svg
            fill="none"
            height="1.5rem"
            viewBox="0 0 24 24"
            width="1.5rem"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19.25 19.25L15.5 15.5M4.75 11C4.75 7.54822 7.54822 4.75 11 4.75C14.4518 4.75 17.25 7.54822 17.25 11C17.25 14.4518 14.4518 17.25 11 17.25C7.54822 17.25 4.75 14.4518 4.75 11Z"
              stroke="#667178"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="1.5"
            />
          </svg>
        </v-btn>

        <div class="d-none d-sm-flex align-center gap-2">
          <div>
            <img
              v-if="user?.avatar"
              class="avatar-img"
              height="50"
              :src="user?.avatar"
              style="width: 50px; height: 50px"
              width="50"
            >
          </div>
          <div>
            <h2 class="text-h6">
              <b>مرحبًا بك {{ user?.fullName || user?.full_name }} ! </b>
            </h2>
            <p class="text-sm text-sec-text">
              نظام إدارة الحضور — مؤشرات الأداء والإحصائيات
            </p>
          </div>
        </div>
        <VSpacer />
        <div class="d-flex gap-3">
          <ToggleTheme />
        </div>
        <div class="d-flex gap-3">
          <UserProfile />
        </div>
      </div>
    </template>

    <RouterView v-slot="{ Component }">
      <ErrorBoundary>
        <Transition mode="out-in" :name="appRouteTransition">
          <Component :is="Component" />
        </Transition>
      </ErrorBoundary>
    </RouterView>

  </VerticalNavLayout>
</template>
