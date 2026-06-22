<script setup lang="ts">
  defineProps({
    title: {
      type: String,
      default: '',
    },
    number: {
      type: Number,
      default: 0,
    },
  })
</script>

<template>
  <VCard class="rounded-lg" elevation="0">
    <v-card-text class="px-1 py-0 pb-0">
      <div class="d-flex flex-column gap- pa-4">
        <div class="d-flex justify-space-between align-center">
          <div>
            <p class="text-md font-weight-bold text-primary mb-0">
              {{ number }}
            </p>
          </div>
          <div class="d-flex justify-center align-center">
            <slot name="icon" />
          </div>
        </div>
        <div class="d-flex align-center justify-space-between gap-2 mt-2">
          <span class="text-sm text-black font-weight-bold">{{ title }}</span>
          <span class="text-lg text-black">
            <slot name="description" />
          </span>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
