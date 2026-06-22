<template>
  <div class="location-picker">
    <iframe
      class="map-container rounded-lg"
      loading="lazy"
      referrerpolicy="no-referrer-when-downgrade"
      :src="embedUrl"
    />
    <VRow class="mt-2">
      <VCol cols="6">
        <VTextField
          label="خط العرض"
          :model-value="latStr"
          placeholder="24.7136"
          type="number"
          @update:model-value="onLatChange"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          label="خط الطول"
          :model-value="lngStr"
          placeholder="46.6753"
          type="number"
          @update:model-value="onLngChange"
        />
      </VCol>
    </VRow>
  </div>
</template>

<script setup lang="ts">
  import { computed, ref } from 'vue'

  const props = defineProps({
    modelValue: {
      type: Object,
      default: () => ({ lat: 24.7136, lng: 46.6753 }),
    },
    isDraggable: {
      type: Boolean,
      default: true,
    },
    isClickable: {
      type: Boolean,
      default: true,
    },
  })

  const emit = defineEmits(['update:modelValue'])

  const latStr = ref(String(props.modelValue.lat))
  const lngStr = ref(String(props.modelValue.lng))

  const embedUrl = computed(() => {
    const lat = parseFloat(latStr.value)
    const lng = parseFloat(lngStr.value)
    if (isNaN(lat) || isNaN(lng)) return ''
    return `https://www.openstreetmap.org/export/embed.html?bbox=${lng - 0.01},${lat - 0.01},${lng + 0.01},${lat + 0.01}&layer=mapnik&marker=${lat},${lng}`
  })

  const onLatChange = (val: string) => {
    latStr.value = val
    const lat = parseFloat(val)
    const lng = parseFloat(lngStr.value)
    if (!isNaN(lat) && !isNaN(lng)) {
      emit('update:modelValue', { lat, lng })
    }
  }

  const onLngChange = (val: string) => {
    lngStr.value = val
    const lat = parseFloat(latStr.value)
    const lng = parseFloat(val)
    if (!isNaN(lat) && !isNaN(lng)) {
      emit('update:modelValue', { lat, lng })
    }
  }
</script>

<style scoped>
.map-container {
  width: 100%;
  height: 400px;
  background-color: rgb(var(--v-theme-background));
  position: relative;
}

.location-picker :deep(.v-field) {
  border-radius: 8px;
  background-color: rgb(var(--v-theme-background));
}
</style>
