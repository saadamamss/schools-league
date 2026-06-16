<script setup>
import { injectionKeyIsVerticalNavHovered, useLayouts } from "@layouts";
import {
  VerticalNavGroup,
  VerticalNavLink,
  VerticalNavSectionTitle,
} from "@layouts/components";
import { config } from "@layouts/config";
import LogoutIcon from "@/components/icons/logouticon.vue";
import { PerfectScrollbar } from "vue3-perfect-scrollbar";
import { useAuthStore } from "@/stores/Auth";
import { toast } from "vue3-toastify";
import { authService } from "@/services/auth.service";
import { useRouter, useRoute } from "vue-router";
import { hasPermission } from "@core/utils/permissions";
import axios from "@/plugins/axios";
import Logo from "@/@core/components/icons/logo.vue";
import { userHasRoutePermission } from "@/utils/usersTypes";
// import { LOCAL_STORAGE_USER_KEY } from "@/config";

const props = defineProps({
  tag: {
    type: [String, null],
    required: false,
    default: "aside",
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
});
const logoutItem = {
  title: "تسجيل الخروج",
  icon: LogoutIcon,
};
const authUserType = computed(() => useAuthStore().user?.user?.user_type?.code);

const refNav = ref();
const { width: windowWidth } = useWindowSize();
const isHovered = useElementHover(refNav);

const auth = useAuthStore();
const router = useRouter();
const isLoggingOut = ref(false);

provide(injectionKeyIsVerticalNavHovered, isHovered);

const {
  isVerticalNavCollapsed: isCollapsed,
  isLessThanOverlayNavBreakpoint,
  isVerticalNavMini,
  isAppRtl,
} = useLayouts();

// const hideTitleAndIcon = isVerticalNavMini(windowWidth, isHovered);

const resolveNavItemComponent = (item) => {
  if ("heading" in item) return VerticalNavSectionTitle;
  if ("children" in item) return VerticalNavGroup;

  return VerticalNavLink;
};

const route = useRoute();
const user_permissions = reactive([]);

watch(
  () => route.name,
  () => {
    props.toggleIsOverlayNavActive(false);
  }
);

onMounted(() => {
  user_permissions.value = JSON.parse(localStorage.getItem("SAR_TOKEN"));
});

// const userNavItems = computed(() => {
//   return props.navItems
// })

const isVerticalNavScrolled = ref(false);
const updateIsVerticalNavScrolled = (val) =>
  (isVerticalNavScrolled.value = val);

const handleNavScroll = (evt) => {
  isVerticalNavScrolled.value = evt.target.scrollTop > 0;
};

const handleLogout = async () => {
  isLoggingOut.value = true;
  try {
    // await axios.post("/auth/logout");
    // First clear the auth store
    await auth.logout();
    // Then clear any remaining session data
    // authService.logout();
    // Finally redirect to login
    await router.push("/auth/login");
  } catch (error) {
    console.error("Logout error:", error);
    toast.error("حدث خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
  } finally {
    isLoggingOut.value = false;
  }
};
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
          to="/"
          class="w-100 app-logo text-primary-text pt-5 mb-2 d-flex align-center justify-center"
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
    <div class="py-5"></div>
    <slot
      name="nav-items position-relative"
      :update-is-vertical-nav-scrolled="updateIsVerticalNavScrolled"
    >
      <PerfectScrollbar
        :key="isAppRtl"
        tag="ul"
        class="nav-items pb-4"
        :options="{ wheelPropagation: false }"
        @ps-scroll-y="handleNavScroll"
      >
        <template v-for="(item, index) in navItems" :key="index">
          <v-divider v-if="item.divider" class="mt-10 mb-10"></v-divider>
          <template v-if="!userHasRoutePermission(item.to.name)"></template>
          <template v-else>
            <Component
              :is="resolveNavItemComponent(item)"
              :item="item"
              :class="item.permission"
            />
            <template v-for="(child, index) in item.children" :key="index">
              <Component
                :is="resolveNavItemComponent(child)"
                :item="child"
                :class="child.permission"
              />
            </template>
          </template>
        </template>
        <template v-if="authUserType === 'organizer'">
          <v-divider class="mt-3 mb-4"></v-divider>
          <div style="max-width: 230px">
            <v-btn
              @click="handleLogout"
              :loading="isLoggingOut"
              :disabled="isLoggingOut"
              block
              color="#667178"
              variant="link"
              class="justify-space-between border logoutbtn"
            >
              <LogoutIcon />
              <span class="nav-item-title ms-3 text-base"> تسجيل الخروج </span>
            </v-btn>
          </div>
        </template>
      </PerfectScrollbar>
      <div class="nav-link py-3 px-2 pe-6" v-if="authUserType != 'organizer'">
        <v-btn
          @click="handleLogout"
          :disabled="isLoggingOut"
          block
          color="#667178"
          variant="link"
          class="rounded-pill justify-space-between"
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
