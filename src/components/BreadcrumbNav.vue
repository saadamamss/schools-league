<!-- BreadcrumbNav.vue -->
<template>
  <v-breadcrumbs :items="items" class="pa-0 mb-4 custom-breadcrumbs" divider="">
    <template v-slot:prepend>
      <svg
        width="20"
        height="20"
        viewBox="0 0 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M5.62516 16.0419H14.3752C15.2956 16.0419 16.0418 15.2957 16.0418 14.3752V8.12523L10.0002 3.95856L3.9585 8.12523V14.3752C3.9585 15.2957 4.70469 16.0419 5.62516 16.0419Z"
          stroke="#25343E"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
        <path
          d="M8.12465 13.1244C8.12465 12.2039 8.87085 11.4577 9.79132 11.4577H10.208C11.1285 11.4577 11.8747 12.2039 11.8747 13.1244V16.0411H8.12465V13.1244Z"
          stroke="#25343E"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </template>
    <template v-slot:divider>
      <v-icon size="18" class="text-grey-darken-1">mdi-chevron-left</v-icon>
    </template>
    <template v-slot:title="{ item }">
      <router-link
        v-if="!item.disabled"
        :to="item.href"
        class="text-decoration-none text-grey-darken-1 text-13"
      >
        {{ item.title }}
      </router-link>
      <span v-else class="text-grey-darken-1 text-13">{{ item.title }}</span>
    </template>
  </v-breadcrumbs>
</template>

<script setup>
defineProps({
  items: {
    type: Array,
    required: true,
    default: () => [],
  },
});
</script>

<style lang="scss" scoped>
.custom-breadcrumbs {
  :deep(.v-breadcrumbs-item--disabled) {
    opacity: 1;
  }

  :deep(.v-breadcrumbs__item) {
    color: rgba(0, 0, 0, 0.6);
    font-size: 13px;
    padding: 0 8px;
  }

  :deep(.v-breadcrumbs-item--disabled) {
    color: rgba(0, 0, 0, 0.6);
  }
}

.text-13 {
  font-size: 13px;
}
</style>
