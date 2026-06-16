<template>
  <div class="custom-switch d-flex gap-3 py-2 px-3 rounded-lg">
    <input
      :id="_key"
      type="checkbox"
      class="d-none"
      :value="switchValue"
      v-model="switchValue"
    />
    <label :for="_key" class="d-block track">
      <span class="d-block thumb"> </span>
    </label>
    <span>{{ switchValue ? "نشط" : "غير نشط" }}</span>
  </div>
</template>

<script setup>
import { watch } from "vue";

const props = defineProps(["_key"]);
const model = defineModel();
const switchValue = ref(model.value === "active");
watch(switchValue, (value) => {
  model.value = value ? "active" : "inactive";
});
</script>

<style scoped>
.custom-switch {
  border: solid 1px #ddd;
  width: max-content;
}
.custom-switch:has(input:checked) {
  border-color: #89ffbc;
  background-color: #eefcf4;
}
.track {
  height: 20px !important;
  width: 38px;
  border-radius: 9999px !important;
  background-color: #bec2c5 !important; /* gray when false */
  cursor: pointer;
}
.thumb {
  background-color: white !important;
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
  background-color: #1e874c !important; /* success color when true */
}
</style>
