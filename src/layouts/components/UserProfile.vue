<script setup>
import { useSettingsStore } from "@/stores/Settings";
import { useAuthStore } from "@/stores/Auth";
import { useRouter } from "vue-router";
import axios from "@/plugins/axios";

const user = ref({});
const settingsListStore = useSettingsStore();
const authStore = useAuthStore();
const router = useRouter();
const isLoggingOut = ref(false);
const isMenu = ref(false);

const logout = async (e) => {
  e.preventDefault();

  isLoggingOut.value = true;
  try {
    // Clear auth store state
    await authStore.logout();
    // Navigate to login page
    router.push("/auth/login");
    // Show success message
    settingsListStore.alertColor = "success";
    settingsListStore.alertMessage = "تم تسجيل الخروج بنجاح";
    settingsListStore.isAlertShow = true;
    setTimeout(() => {
      settingsListStore.isAlertShow = false;
      settingsListStore.alertMessage = "";
    }, 3000);
  } catch (error) {
    console.error("Logout error:", error);
    settingsListStore.alertColor = "error";
    settingsListStore.alertMessage = "حدث خطأ أثناء تسجيل الخروج";
    settingsListStore.isAlertShow = true;
  } finally {
    isLoggingOut.value = false;
  }
};

onMounted(() => {
  user.value = authStore.user.user;
  console.log(user.value);
});
</script>

<template>
  <div>
    <v-btn
      height="48"
      variant="flat"
      color="background"
      elevation="0"
      class="btn-user-profile px-3"
    >
      <div class="d-flex align-center ga-3">
        <v-avatar size="35" color="avatar">
          <v-img
            v-if="user?.profile_image"
            width="30"
            height="30"
            src="https://placehold.co/400"
            class="avatar-img"
          />
          <span v-else>
            {{ user?.first_name?.[0] }}
          </span>
        </v-avatar>

        <span class="text-black text-sm">
          {{ user?.first_name }} {{ user?.last_name }}
        </span>

        <span class="ms-2">
          <svg
            width="9"
            height="6"
            viewBox="0 0 8 5"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M7.25 0.75L4 4.25L0.75 0.75"
              stroke="#667178"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </span>
      </div>

      <v-menu activator="parent">
        <v-list>
          <!-- <v-list-item :to="{ name: 'home' }">
            <v-list-item-title class="text-sm"
              >الصفحة الرئيسية</v-list-item-title
            >
          </v-list-item> -->
          <v-list-item :to="{ name: 'profile' }">
            <v-list-item-title class="text-sm">
              <VIcon class="me-2" icon="tabler-user" size="21" />
              حسابي
            </v-list-item-title>
          </v-list-item>
          <v-list-item @click="logout" :disabled="isLoggingOut">
            <v-list-item-title>
              <VIcon class="me-2" icon="tabler-logout" size="21" />
              <span v-if="!isLoggingOut" class="text-sm">تسجيل الخروج</span>
              <VIcon
                v-else
                icon="mingcute:loading-line"
                class="fixed loading"
                size="32"
              />
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </v-btn>
  </div>
</template>

<style>
.avatar-container {
  position: relative;
  border: 1px solid #f8f7fa;
  border-radius: 50%;
  block-size: 35px;
  inline-size: 35px;
}

.avatar-img {
  display: inline-block;
  block-size: 100%;
  inline-size: 100%;
  object-fit: contain;
}

.v-btn.btn-user-profile {
  --v-btn-height: 40px;
  border-radius: 10px;
}
</style>
