<script setup lang="ts">
  const props = defineProps<{
    confirmationMsg: string
    isDialogVisible: boolean
  }>()

  const emit = defineEmits<{
    (e: 'update:isDialogVisible', val: boolean): void
    (e: 'confirm', val: boolean): void
  }>()

  const updateModelValue = (val: boolean) => {
    emit('update:isDialogVisible', val)
  }

  const onConfirmation = () => {
    emit('confirm', true)
    updateModelValue(false)
  }

  const onCancel = () => {
    emit('confirm', false)
    emit('update:isDialogVisible', false)
  }
</script>

<template>
  <!-- 👉 Confirm Dialog -->
  <VDialog
    max-width="500"
    :model-value="props.isDialogVisible"
    @update:model-value="updateModelValue"
  >
    <VCard class="text-center px-10 py-6">
      <VCardText>
        <VBtn
          class="mb-4"
          color="warning"
          icon
          style="width: 88px; height: 88px; pointer-events: none;"
          variant="outlined"
        >
          <span class="text-5xl">!</span>
        </VBtn>

        <h6 class="text-lg font-weight-medium">
          {{ props.confirmationMsg }}
        </h6>
      </VCardText>

      <VCardActions class="align-center justify-center gap-2">
        <VBtn
          variant="elevated"
          @click="onConfirmation"
        >
          Confirm
        </VBtn>

        <VBtn
          color="secondary"
          variant="tonal"
          @click="onCancel"
        >
          Cancel
        </VBtn>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
