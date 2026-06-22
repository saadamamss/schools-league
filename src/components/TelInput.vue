<template>
  <vue-tel-input
    v-model="phone"
    :preferred-countries="['sa', 'us', 'gb']"
    @on-input="onInput"
  />
</template>

<script setup lang="ts">
  import VueTelInput from 'vue-tel-input'
  import 'vue-tel-input/vue-tel-input.css'

  const phone = ref('')
  const phoneObject = ref<{
    formattedNumber: string
    number: string
    valid: boolean
    country: Record<string, any>
  } | null>(null)

  function onInput (formattedNumber: string, { number, valid, country }: { number: string; valid: boolean; country: Record<string, any> }) {
    phoneObject.value = {
      formattedNumber,
      number,
      valid,
      country,
    }
  }
</script>
