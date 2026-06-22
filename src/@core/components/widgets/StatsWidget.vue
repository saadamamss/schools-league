<script setup lang="ts">
  defineProps({
    title: {
      type: String,
      default: '',
    },
    number: {
      type: Number,
      default: 0,
    },
  })
</script>

<template>
  <VCard class="rounded-lg widget-card" elevation="0">
    <div class="back">
      <img alt="" src="/imgs/widget.png">
    </div>
    <v-card-text class="px-1 py-2 pb-0 card-content">
      <div class="d-flex flex-column gap-4 pa-4">
        <div class="d-flex flex-column">
          <div class="d-flex justify-start align-start">
            <slot name="icon" />
          </div>
          <div>
            <h2 class="text-h5 text-lg-h4 font-weight-bold text-primary">
              {{ number }}
            </h2>
          </div>
          <div>
            <span class="text-lg title">{{ title }}</span>
          </div>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
<style scoped>
.widget-card {
  position: relative;
  overflow: hidden !important;
  background-color: rgb(var(--v-theme-surface));
  /* padding-block: 12px; */
}
.title {
  color: rgb(var(--v-theme-on-surface));
}
.back {
  position: absolute;
  width: 100%;
  height: 100%;
  left: -10px;
  top: 0px;
  overflow: hidden;
}
.back img {
  width: 100%;
  object-fit: cover;
}
.card-content {
  position: relative;
  z-index: 1;
}
</style>
