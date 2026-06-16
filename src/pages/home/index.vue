<template>
  <div>
    <Suspense>
      <component :is="homeView" :user="user.user"></component>
    </Suspense>
  </div>
</template>

<script setup>
import { useAuthStore } from "@/stores/Auth";
import { computed } from "vue";

const { user } = useAuthStore();

const homeView = computed(() => {
  if (user.user?.user_type?.code === "organizer") {
    return defineAsyncComponent(() => import("@/components/Organizer.vue"));
  }
  return defineAsyncComponent(() => import("@/components/Admin.vue"));
});
</script>
