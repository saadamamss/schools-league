<script setup lang="ts">
  defineProps<{
    user: Record<string, any>
    noShadow?: boolean
    noPadding?: boolean
    transparent?: boolean
    size?: string
  }>()

  const fallbackAvatar = 'https://placehold.co/400'
</script>

<template>
  <div
    class="user-widget"
    :class="{
      'no-shadow': noShadow,
      'no-padding': noPadding,
      transparent: transparent,
    }"
  >
    <div class="d-flex gap-3 align-center">
      <div>
        <span class="avatar-wrapper d-flex justify-center align-center">
          <!-- <img :src="fallbackAvatar" alt="User avatar" /> -->
          <img
            alt="User avatar"
            class="avatar-img border rounded-circle"
            :src="user?.personal_image || user?.avatar_image || fallbackAvatar"
            :style="{ width: size === 'small' ? '2rem' : '2.5rem' }"
            @error="(e: any) => { if (e?.target) e.target.src = fallbackAvatar }"
          >
        </span>
      </div>
      <div>
        <h2 class="user-name text-subtitle-2 text-black whitespace-wrap">
          {{ user?.firstname }} {{ user?.lastname }}
        </h2>
        <p class="user-job mb-0 text-body-2">
          {{ user?.job }}
        </p>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.user-widget {
  background-color: rgb(var(--v-theme-surface));
  padding: 1rem;
  border-radius: 8px;

  &.transparent {
    background-color: transparent;
  }

  &.no-shadow {
    box-shadow: none;
  }

  &.no-padding {
    padding: 0;
  }

  .avatar-wrapper {
    width: 2.5rem;
    height: 2.5rem;
  }

  .avatar-img {
    width: 2.5rem;
    height: 2.5rem;
  }

  .user-name {
    color: #000;
  }

  .user-job {
    color: #75797c;
  }
}
</style>
