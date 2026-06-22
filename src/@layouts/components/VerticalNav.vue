<script setup lang="ts">
  import { injectionKeyIsVerticalNavHovered, useLayouts } from '@layouts'
  import {
    VerticalNavGroup,
    VerticalNavLink,
    VerticalNavSectionTitle,
  } from '@layouts/components'
  import { config } from '@layouts/config'
  import LogoutIcon from '@/components/icons/logouticon.vue'
  import { PerfectScrollbar } from 'vue3-perfect-scrollbar'
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { authService } from '@/services/auth.service'
  import { useRoute, useRouter } from 'vue-router'
  import { hasPermission } from '@core/utils/permissions'
  import Logo from '@/@core/components/icons/logo.vue'
  import { userHasRoutePermission } from '@/utils/usersTypes'
  // import { LOCAL_STORAGE_USER_KEY } from "@/config";

  const props = defineProps({
    tag: {
      type: [String, null],
      required: false,
      default: 'aside',
    },
    navItems: {
      type: null,
      required: true,
    },
    isOverlayNavActive: {
      type: Boolean,
      required: true,
    },
    toggleIsOverlayNavActive: {
      type: Function,
      required: true,
    },
  })
  const logoutItem = {
    title: 'تسجيل الخروج',
    icon: LogoutIcon,
  }
  const authUserType = computed(() => (useAuthStore().user as any)?.user?.user_type?.code)

  const refNav = ref()
  const { width: windowWidth } = useWindowSize()
  const isHovered = useElementHover(refNav)

  const auth = useAuthStore()
  const appStore = useAppStore()
  const router = useRouter()
  const isLoggingOut = ref(false)

  provide(injectionKeyIsVerticalNavHovered, isHovered)

  const {
    isVerticalNavCollapsed: isCollapsed,
    isLessThanOverlayNavBreakpoint,
    isVerticalNavMini,
    isAppRtl,
  } = useLayouts()

  // const hideTitleAndIcon = isVerticalNavMini(windowWidth, isHovered);

  const resolveNavItemComponent = (item: Record<string, any>) => {
    if ('heading' in item) return VerticalNavSectionTitle
    if ('children' in item) return VerticalNavGroup

    return VerticalNavLink
  }

  const route = useRoute()
  const userPermissions = ref<any[]>([])

  watch(
    () => route.name,
    () => {
      props.toggleIsOverlayNavActive(false)
    }
  )

  onMounted(() => {
    userPermissions.value = JSON.parse(localStorage.getItem('SAR_TOKEN') || '[]')
  })

  // const userNavItems = computed(() => {
  //   return props.navItems
  // })

  const isVerticalNavScrolled = ref(false)
  const updateIsVerticalNavScrolled = (val: boolean) =>
    (isVerticalNavScrolled.value = val)

  const handleNavScroll = (evt: any) => {
    isVerticalNavScrolled.value = evt.target.scrollTop > 0
  }

  const handleLogout = async () => {
    isLoggingOut.value = true
    try {
      // First clear the auth store
      await auth.logout()
      // Then clear any remaining session data
      // authService.logout();
      // Finally redirect to login
      await router.push('/auth/login')
    } catch (error: any) {
      appStore.showSnackbar({ message: 'حدث خطأ غير متوقع!', color: 'error' })
    } finally {
      isLoggingOut.value = false
    }
  }
</script>

<template>
  <Component
    :is="props.tag"
    ref="refNav"
    class="layout-vertical-nav"
    :class="[
      {
        'overlay-nav': isLessThanOverlayNavBreakpoint(windowWidth),
        hovered: isHovered,
        visible: isOverlayNavActive,
        scrolled: isVerticalNavScrolled,
        'sidebar-collapsed': isCollapsed,
      },
    ]"
  >
    <!-- 👉 Header -->
    <div
      class="nav-header d-flex align-center justify-space-between ga-4 position-relative overflow-hidden px-4 mb-0"
      :class="[isCollapsed ? 'pe-4' : 'px-4']"
    >
      <slot name="nav-header">
        <RouterLink
          v-if="!isCollapsed || isHovered"
          class="w-100 app-logo text-primary-text pt-5 mb-2 d-flex align-center justify-center"
          to="/"
        >
          <Logo />
        </RouterLink>
      </slot>
    </div>
    <!--  -->
    <slot name="before-nav-items">
      <!-- <div class="vertical-nav-items-shadow"/> -->
    </slot>
    <!--  -->
    <div class="py-5" />
    <slot
      name="nav-items position-relative"
      :update-is-vertical-nav-scrolled="updateIsVerticalNavScrolled"
    >
      <PerfectScrollbar
        :key="isAppRtl"
        class="nav-items pb-4"
        :options="{ wheelPropagation: false }"
        tag="ul"
        @ps-scroll-y="handleNavScroll"
      >
        <template v-for="(item, idx) in navItems" :key="idx">
          <v-divider v-if="item.divider" class="mt-10 mb-10" />
          <template v-if="!userHasRoutePermission(item.to.name)" />
          <template v-else>
            <Component
              :is="resolveNavItemComponent(item)"
              :class="item.permission"
              :item="item"
            />
            <template v-for="(child, index) in item.children" :key="index">
              <Component
                :is="resolveNavItemComponent(child)"
                :class="child.permission"
                :item="child"
              />
            </template>
          </template>
        </template>
        <template v-if="authUserType === 'organizer'">
          <v-divider class="mt-3 mb-4" />
          <div style="max-width: 230px">
            <v-btn
              block
              class="justify-space-between border logoutbtn"
              color="#667178"
              :disabled="isLoggingOut"
              :loading="isLoggingOut"
              variant="text"
              @click="handleLogout"
            >
              <LogoutIcon />
              <span class="nav-item-title ms-3 text-base"> تسجيل الخروج </span>
            </v-btn>
          </div>
        </template>
      </PerfectScrollbar>
      <div v-if="authUserType != 'organizer'" class="nav-link py-3 px-2 pe-6">
        <v-btn
          block
          class="rounded-pill justify-space-between"
          color="#667178"
          :disabled="isLoggingOut"
          variant="text"
          @click="handleLogout"
        >
          <LogoutIcon />
          <span class="nav-item-title ms-3 text-base"> تسجيل الخروج </span>
        </v-btn>
      </div>
    </slot>
  </Component>
</template>

<style lang="scss">
@use "@configured-variables" as variables;
@use "@layouts/styles/mixins";

// .v-theme--dark .app-logo {
//   img {
//     filter: brightness(0) invert(1);
//   }
// }

.app-logo {
  img {
    filter: unset;
    max-inline-size: 300px;
  }
}

// 👉 Vertical Nav
.layout-vertical-nav {
  position: fixed;
  z-index: variables.$layout-vertical-nav-z-index;
  display: flex;
  flex-direction: column;
  block-size: 100%;
  inline-size: variables.$layout-vertical-nav-width;
  inset-block-start: 0;
  inset-inline-start: 0;
  transition: transform 0.25s ease-in-out, inline-size 0.25s ease-in-out,
    box-shadow 0.25s ease-in-out;
  will-change: transform, inline-size;
  // border-radius: 40px 0 0 40px;

  .logoutbtn {
    width: 230px !important;
    max-width: 230px !important;
    height: 60px;
    border-radius: 16px 0 0 16px;
  }
  .nav-header {
    display: flex;
    align-items: center;
    // margin-block-end: 1rem;
    min-block-size: 95px !important;
    border: solid 2px rgba(var(--v-theme-background));
    border-width: 0 0 2px 2px;
    .header-action {
      // position: absolute;
      cursor: pointer;
      // inset-block-start: 50%;
      // inset-inline-start: 10px;
      // transform: translateY(-50%);
    }
  }

  .app-title-wrapper {
    margin-inline-end: auto;
  }

  .nav-items {
    block-size: 100%;

    // ℹ️ We no loner needs this overflow styles as perfect scrollbar applies it
    // overflow-x: hidden;

    // // ℹ️ We used `overflow-y` instead of `overflow` to mitigate overflow x. Revert back if any issue found.
    // overflow-y: auto;
  }

  .nav-item-title {
    overflow: hidden;
    margin-inline-end: auto;
    text-overflow: ellipsis;
    white-space: nowrap;
    line-height: 1.6;
  }

  // 👉 Collapsed
  .layout-vertical-nav-collapsed & {
    &:not(.hovered) {
      inline-size: variables.$layout-vertical-nav-collapsed-width;
    }
  }

  // 👉 Overlay nav
  &.overlay-nav {
    &:not(.visible) {
      transform: translateX(-#{variables.$layout-vertical-nav-width});

      @include mixins.rtl {
        transform: translateX(variables.$layout-vertical-nav-width);
      }
    }
  }
}
</style>
