<template>
  <VContainer v-if="hasError" class="text-center py-10">
    <VAlert type="error" variant="tonal" class="mx-auto" max-width="500">
      <template #title>حدث خطأ</template>
      {{ errorMessage }}
    </VAlert>
    <VBtn class="mt-4" color="primary" @click="recover">إعادة المحاولة</VBtn>
  </VContainer>
  <slot v-else />
</template>

<script setup>
import { ref, onErrorCaptured } from 'vue'

const props = defineProps({
  errorMessage: { type: String, default: 'حدث خطأ غير متوقع' },
})

const hasError = ref(false)

onErrorCaptured(() => {
  hasError.value = true
  return false
})

const recover = () => {
  hasError.value = false
}
</script>
