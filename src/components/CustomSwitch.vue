<template>
  <div class="custom-switch d-flex gap-3 py-2 px-3 rounded-lg">
    <input
      :id="_key"
      v-model="switchValue"
      class="d-none"
      type="checkbox"
      :value="switchValue"
    >
    <label class="d-block track" :for="_key">
      <span class="d-block thumb" />
    </label>
    <span>{{ switchValue ? "نشط" : "غير نشط" }}</span>
  </div>
</template>

<script setup lang="ts">
  import { watch } from 'vue'

  // eslint-disable-next-line vue/prop-name-casing
  const props = defineProps(['_key'])
  const model = defineModel()
  const switchValue = ref(model.value === 'active')
  watch(switchValue, value => {
    model.value = value ? 'active' : 'inactive'
  })
</script>

<style scoped>
.custom-switch {
  border: solid 1px rgba(var(--v-theme-on-surface), 0.23);
  width: max-content;
}
.custom-switch:has(input:checked) {
  border-color: rgb(var(--v-theme-success));
  background-color: rgba(var(--v-theme-success), 0.1);
}
.track {
  height: 20px !important;
  width: 38px;
  border-radius: 9999px !important;
  background-color: rgba(var(--v-theme-on-surface), 0.3) !important;
  cursor: pointer;
}
.thumb {
  background-color: rgb(var(--v-theme-surface)) !important;
  width: 14px !important;
  height: 14px !important;
  position: relative;
  top: 3px;
  right: 2px;
  box-shadow: 0px 2px 4px -1px rgba(0, 0, 0, 0.2),
    0px 4px 5px 0px rgba(0, 0, 0, 0.14), 0px 1px 10px 0px rgba(0, 0, 0, 0.12);
  border-radius: 50%;
  transition: right 0.2s ease-in-out;
}
.custom-switch:has(input:checked) .thumb {
  right: calc(100% - 18px);
}
.custom-switch:has(input:checked) .track {
  background-color: rgb(var(--v-theme-success)) !important;
}
</style>
