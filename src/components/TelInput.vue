<template>
  <vue-tel-input
    v-model="phone"
    :preferredCountries="['sa', 'us', 'gb']"
    @onInput="onInput"
  ></vue-tel-input>
</template>

<script setup>
import VueTelInput from "vue-tel-input";
import "vue-tel-input/vue-tel-input.css";

const phone = ref("");
const phoneObject = ref(null);

function onInput(formattedNumber, { number, valid, country }) {
  phoneObject.value = {
    formattedNumber,
    number,
    valid,
    country,
  };
}
</script>
