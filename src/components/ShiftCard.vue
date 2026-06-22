<template>
  <VCard class="shift-card rounded-lg">
    <VCardText>
      <!-- Header -->
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center">
          <div class="rounded-circle bg-primary-lighten-4 p-2 me-3">
            <svg
              fill="none"
              height="1.5rem"
              viewBox="0 0 24 24"
              width="1.5rem"
              xmlns="http://www.w3.org/2000/svg"
            >
              <circle
                cx="12"
                cy="12"
                r="10"
                stroke="#667178"
                stroke-width="2"
              />
              <polyline
                points="12 6 12 12 16 14"
                stroke="#667178"
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
              />
            </svg>
          </div>
          <div>
            <h3 class="text-h6 mb-1">{{ shift.name }}</h3>
            <p class="text-body-2 text-medium-emphasis mb-0">
              {{ shift.startTime }} - {{ shift.endTime }}
            </p>
          </div>
        </div>

        <!-- Actions -->
        <div class="d-flex align-center ga-2">
          <VBtn
            color="primary"
            icon
            size="small"
            variant="text"
            @click="$emit('edit', shift)"
          >
            <VIcon icon="mdi-pencil-outline" size="22" />
          </VBtn>

          <VBtn
            color="error"
            icon
            size="small"
            variant="text"
            @click="$emit('delete', shift.id)"
          >
            <VIcon icon="mdi-delete" size="22" />
          </VBtn>
        </div>
      </div>
    </VCardText>
  </VCard>
</template>

<script setup lang="ts">
  defineProps({
    shift: {
      type: Object,
      required: true,
    },
  })

  defineEmits(['edit', 'delete'])
</script>

<style lang="scss" scoped>
.shift-card {
  transition: transform 0.2s, box-shadow 0.2s;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 4px 25px 0 rgba(var(--v-shadow-key-umbra-color), 0.1);
  }
}

.bg-primary-lighten-4 {
  background-color: rgba(182, 146, 101, 0.1);
}
</style>
