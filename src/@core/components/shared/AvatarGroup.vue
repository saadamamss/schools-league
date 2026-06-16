<script setup>
const props = defineProps({
  avatars: {
    type: Array,
    default: () => [],
  },
  size: {
    type: String,
    default: "45",
  },
  maxAvatars: {
    type: Number,
    default: 4,
  },
});

const fallbackAvatar = "https://placehold.co/400";

const visibleItems = computed(() => {
  if (props.avatars?.length > props.maxAvatars) {
    return props.avatars.slice(0, props.maxAvatars);
  }

  return props.avatars;
});
</script>

<template>
  <div class="d-flex pa-2 rounded-pill bg-background align-center gap-2">
    <template v-for="(item, index) in visibleItems" :key="index">
      <v-avatar
        :size="size"
        style="border: 4px solid #fff"
        :class="{ 'ms-n3': index > 0 }"
      >
        <v-tooltip activator="parent" location="top">{{ item.name }}</v-tooltip>
        <v-img
          :alt="item.name"
          :src="item.avatar || fallbackAvatar"
          @error="$event.target.src = fallbackAvatar"
        ></v-img>
      </v-avatar>
    </template>
    <v-avatar
      v-if="avatars?.length > maxAvatars"
      :size="size"
      color="gray"
      style="border: 4px solid #fff"
      class="ms-n3"
    >
      <span class="text-primary">
        <span>{{ avatars?.length - maxAvatars }}</span>
        <span>+</span>
      </span>
    </v-avatar>

    <slot name="additionData"></slot>
  </div>
</template>
