<script setup lang="ts">
  interface AvatarItem {
    name: string
    avatar?: string
    [key: string]: any
  }

  const props = withDefaults(defineProps<{
    avatars?: AvatarItem[]
    size?: string
    maxAvatars?: number
  }>(), {
    avatars: () => [],
    size: '45',
    maxAvatars: 4,
  })

  const fallbackAvatar = 'https://placehold.co/400'

  const visibleItems = computed(() => {
    if (props.avatars?.length > props.maxAvatars) {
      return props.avatars.slice(0, props.maxAvatars)
    }

    return props.avatars
  })
</script>

<template>
  <div class="d-flex pa-2 rounded-pill bg-background align-center gap-2">
    <template v-for="(item, index) in visibleItems" :key="index">
      <v-avatar
        :class="{ 'ms-n3': index > 0 }"
        :size="size"
        style="border: 4px solid rgb(var(--v-theme-surface))"
      >
        <v-tooltip activator="parent" location="top">{{ item.name }}</v-tooltip>
        <v-img
          :alt="item.name"
          :src="item.avatar || fallbackAvatar"
          @error="(e: any) => { if (e?.target) e.target.src = fallbackAvatar }"
        />
      </v-avatar>
    </template>
    <v-avatar
      v-if="avatars?.length > maxAvatars"
      class="ms-n3"
      color="gray"
      :size="size"
      style="border: 4px solid rgb(var(--v-theme-surface))"
    >
      <span class="text-primary">
        <span>{{ avatars?.length - maxAvatars }}</span>
        <span>+</span>
      </span>
    </v-avatar>

    <slot name="additionData" />
  </div>
</template>
