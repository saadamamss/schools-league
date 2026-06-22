<template>
  <VDialog v-model="isOpen" max-width="650px">
    <VCard>
      <VCardTitle class="text-h6 pa-4 text-center">
        <Success />
      </VCardTitle>
      <VCardText class="pa-4">
        <h3 class="text-primary text-center mb-2">{{ props.title }}</h3>
        <p class="text-center">{{ props.subtitle }}</p>
      </VCardText>
      <VCardActions
        class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
      >
        <VBtn
          class="text-black rounded-xl bg-white px-6"
          variant="outlined"
          @click="emit('cancel')"
        >
          {{ props.cancelText }}
        </VBtn>
        <VBtn
          class="rounded-xl px-8"
          color="primary"
          variant="flat"
          @click="emit('action')"
        >
          {{ props.actionText }}
        </VBtn>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
<script setup lang="ts">
  import Success from '@/components/icons/success.vue'
  const props = defineProps<{
    title: string
    subtitle: string
    cancelText: string
    actionText: string
  }>()
  const isOpen = defineModel<boolean>({ required: true })
  const emit = defineEmits<{ cancel: []; action: [] }>()
</script>
