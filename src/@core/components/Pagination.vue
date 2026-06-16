<template>
  <div
    class="d-flex flex-column flex-md-row align-md-center justify-md-space-between ga-2 mt-6"
  >
    <div class="d-flex align-center justify-space-between ga-3">
      <v-select
        v-model="perPageModel"
        :items="[5, 12, 25, 50, 100]"
        min-width="100"
        max-width="120"
        bg-color="#F8F9F9"
        rounded="lg"
        variant="solo"
        class="v-select-no-shadow"
        @update:model-value="$emit('update:perPage', $event)"
      />
      <span class="text-sm">
        تم عرض من <span>{{ startItem }}</span> إلى <span>{{ endItem }}</span> من
        أصل
        <span>{{ totalItems }}</span>
      </span>
    </div>
    <v-pagination
      v-model="currentPageModel"
      :length="totalPages"
      :total-visible="5"
      rounded="circle"
      variant="flat"
      @update:model-value="$emit('update:page', $event)"
    />
  </div>
</template>

<script setup>
import { computed, ref, watch } from "vue";

const props = defineProps({
  totalItems: {
    type: Number,
    required: true,
    default: 0,
  },
  perPage: {
    type: Number,
    required: true,
    default: 10,
  },
  page: {
    type: Number,
    required: true,
    default: 1,
  },
});

const emit = defineEmits(["update:page", "update:perPage"]);

// Create local models for v-model binding
const perPageModel = ref(props.perPage);
const currentPageModel = ref(props.page);

// Watch for prop changes to update local models
watch(
  () => props.perPage,
  (newVal) => {
    perPageModel.value = newVal;
  }
);

watch(
  () => props.page,
  (newVal) => {
    currentPageModel.value = newVal;
  }
);

// Computed properties
const totalPages = computed(
  () => Math.ceil(props.totalItems / props.perPage) || 1
);

const startItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return (props.page - 1) * props.perPage + 1;
});

const endItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return Math.min(props.page * props.perPage, props.totalItems);
});
</script>

<style lang="scss" scoped>
.v-select-no-shadow {
  :deep(.v-field) {
    box-shadow: none !important;
  }
}
</style>
