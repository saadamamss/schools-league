<template>
  <VContainer v-if="hasError" class="text-center py-10">
    <VAlert class="mx-auto" max-width="500" type="error" variant="tonal">
      <template #title>حدث خطأ</template>
      {{ errorMessage }}
    </VAlert>
    <VBtn class="mt-4" color="primary" @click="recover">إعادة المحاولة</VBtn>
  </VContainer>
  <slot v-else />
</template>

<script setup lang="ts">
  import { onErrorCaptured, ref } from 'vue'

  const props = defineProps({
    errorMessage: { type: String, default: 'حدث خطأ غير متوقع' },
  })

  const hasError = ref(false)

  onErrorCaptured(err => {
    console.error('ErrorBoundary caught:', err)
    hasError.value = true
    return false
  })

  const recover = () => {
    hasError.value = false
  }
</script>
