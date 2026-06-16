<script setup>
import navItems from "@/navigation/vertical";
import { useThemeConfig } from "@core/composable/useThemeConfig";
import { useAppStore } from "@/stores/app";

// Components
import Footer from "@/layouts/components/Footer.vue";
import NavbarThemeSwitcher from "@/layouts/components/NavbarThemeSwitcher.vue";
import UserProfile from "@/layouts/components/UserProfile.vue";
import Notifications from "@/@core/components/Notifications";

// @layouts plugin
import { VerticalNavLayout } from "@layouts";
import ToggleTheme from "@/components/toggleTheme.vue";
import { onMounted } from "vue";
import { useAuthStore } from "@/stores/Auth";

const authStore = useAuthStore();
const appStore = useAppStore();

const { appRouteTransition, isLessThanOverlayNavBreakpoint } = useThemeConfig();
const { width: windowWidth } = useWindowSize();

const notifications = ref([
  {
    id: 1,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله",
  },
  {
    id: 2,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله نص وهمي يمكن استبداله",
  },
  {
    id: 3,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله",
  },
]);

const search = ref("");
const user = ref({});
onMounted(() => {
  user.value = authStore.user.user;
  console.log(user.value);
});
</script>

<template>
  <VerticalNavLayout :nav-items="navItems">
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center ga-3 text-dark">
        <VBtn
          v-if="isLessThanOverlayNavBreakpoint(windowWidth)"
          icon
          variant="text"
          color="default"
          class="ms-n3"
          size="small"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="tabler-menu-2" size="24" />
        </VBtn>
        <!-- search box -->
        <v-btn
          color="default"
          size="small"
          class="btn-icon d-sm-none"
          elevation="0"
        >
          <svg
            width="1.5rem"
            height="1.5rem"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19.25 19.25L15.5 15.5M4.75 11C4.75 7.54822 7.54822 4.75 11 4.75C14.4518 4.75 17.25 7.54822 17.25 11C17.25 14.4518 14.4518 17.25 11 17.25C7.54822 17.25 4.75 14.4518 4.75 11Z"
              stroke="#667178"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </v-btn>

        <div class="d-none d-sm-flex align-center gap-2">
          <div>
            <img
              v-if="user?.avatar"
              width="50"
              height="50"
              :src="user?.avatar"
              class="avatar-img"
              style="width: 50px; height: 50px"
            />
          </div>
          <div>
            <h2 class="text-h6">
              <b>مرحبًا بك {{ user?.full_name }} ! </b>
            </h2>
            <p class="text-sm text-sec-text">
              نظام إدارة الحضور — مؤشرات الأداء والإحصائيات
            </p>
          </div>
        </div>
        <!-- <span class="text-black font-weight-semibold">{{
          appStore.appPageTitle
        }}</span> -->
        <!-- <NavbarThemeSwitcher /> -->
        <VSpacer />
        <div class="d-flex gap-3">
          <ToggleTheme />
        </div>
        <!-- <div class="d-flex gap-3">
          <Notifications :notifications="[]" />
        </div> -->
        <div class="d-flex gap-3">
          <!-- <Notifications :notifications="notifications" /> -->
          <UserProfile />
        </div>
      </div>
    </template>

    <RouterView v-slot="{ Component }">
      <Transition :name="appRouteTransition" mode="out-in">
        <Component :is="Component" />
      </Transition>
    </RouterView>

    <!-- <template #footer>
      <Footer />
    </template> -->

    <!-- <TheCustomizer /> -->
  </VerticalNavLayout>
</template>
