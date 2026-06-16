<template>
  <VDialog max-width="650px" v-model="isOpen">
    <VCard>
      <VCardTitle class="text-h6 pa-4 text-center">
        <Trashfordialog />
      </VCardTitle>
      <VCardText class="pa-4">
        <h3 class="text-primary text-center mb-2">{{ props.title }}</h3>
        <p class="text-center">{{ props.subtitle }}</p>
      </VCardText>
      <VCardActions
        class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
      >
        <VBtn
          variant="outlined"
          class="text-black rounded-xl bg-white px-6"
          @click="emit('cancel')"
        >
          {{ props.cancelText }}
        </VBtn>
        <VBtn
          color="primary"
          variant="flat"
          class="rounded-xl px-8"
          @click="emit('action')"
        >
          {{ props.actionText }}
        </VBtn>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
<script setup>
import Trashfordialog from "./icons/trashfordialog.vue";

const props = defineProps(["title", "subtitle", "cancelText", "actionText"]);
const isOpen = defineModel();
const emit = defineEmits(["cancel", "action"]);
</script>
