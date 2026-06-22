<script setup lang="ts">
  import { computed, ref } from 'vue'
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { useRouter } from 'vue-router'

  const authStore = useAuthStore()
  const user = computed(() => authStore.user)
  const appStore = useAppStore()
  const router = useRouter()
  const isLoggingOut = ref(false)
  const isMenu = ref(false)

  const logout = async (e: Event) => {
    e.preventDefault()

    isLoggingOut.value = true
    try {
      await authStore.logout()
      router.push('/auth/login')
      appStore.showSnackbar({
        message: 'تم تسجيل الخروج بنجاح',
        color: 'success',
      })
    } catch (error: unknown) {
      console.error('Logout error:', error)
      appStore.showSnackbar({
        message: 'حدث خطأ أثناء تسجيل الخروج',
        color: 'error',
      })
    } finally {
      isLoggingOut.value = false
    }
  }
</script>

<template>
  <div>
    <v-btn
      class="btn-user-profile px-3"
      color="background"
      elevation="0"
      height="48"
      variant="flat"
    >
      <div class="d-flex align-center ga-3">
        <v-avatar color="avatar" size="35">
          <v-img
            v-if="user?.profileImage"
            class="avatar-img"
            height="30"
            src="https://placehold.co/400"
            width="30"
          />
          <span v-else>
            {{ user?.firstName?.[0] }}
          </span>
        </v-avatar>

        <span class="text-black text-sm">
          {{ user?.firstName }} {{ user?.lastName }}
        </span>

        <span class="ms-2">
          <svg
            fill="none"
            height="6"
            viewBox="0 0 8 5"
            width="9"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M7.25 0.75L4 4.25L0.75 0.75"
              stroke="#667178"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="1.5"
            />
          </svg>
        </span>
      </div>

      <v-menu activator="parent">
        <v-list>
          <v-list-item :to="{ name: 'profile' }">
            <v-list-item-title class="text-sm">
              <VIcon class="me-2" icon="mdi-account" size="21" />حسابي
            </v-list-item-title>
          </v-list-item>
          <v-list-item :disabled="isLoggingOut" @click="logout">
            <v-list-item-title>
              <VIcon class="me-2" icon="mdi-logout" size="21" />
              <span v-if="!isLoggingOut" class="text-sm">تسجيل الخروج</span>
              <VIcon
                v-else
                class="fixed loading"
                icon="mdi-refresh"
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
