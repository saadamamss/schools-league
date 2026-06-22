<template>
  <div
    ref="mapContainer"
    class="map-container"
    :style="{ height }"
  />
  <div v-if="!hasValidCoords" class="map-empty">
    <p>الإحداثيات غير متوفرة</p>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
  import L from 'leaflet'
  import 'leaflet/dist/leaflet.css'

  const markerIcon = L.divIcon({
    html: `<svg xmlns="http://www.w3.org/2000/svg" width="25" height="41" viewBox="0 0 25 41">
      <path d="M12.5 0C5.6 0 0 5.6 0 12.5C0 21.9 12.5 41 12.5 41S25 21.9 25 12.5C25 5.6 19.4 0 12.5 0Z" fill="#1976D2"/>
      <circle cx="12.5" cy="12.5" r="5" fill="white"/>
    </svg>`,
    className: '',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
  })

  const props = defineProps({
    latitude: {
      type: [Number, String],
      required: true,
    },
    longitude: {
      type: [Number, String],
      required: true,
    },
    height: {
      type: String,
      default: '400px',
    },
    zoom: {
      type: Number,
      default: 15,
    },
    readonly: {
      type: Boolean,
      default: true,
    },
  })

  const mapContainer = ref<HTMLDivElement | null>(null)
  let mapInstance: L.Map | null = null

  const lat = computed(() => {
    const v = typeof props.latitude === 'string' ? parseFloat(props.latitude) : props.latitude
    return isNaN(v) ? null : v
  })
  const lng = computed(() => {
    const v = typeof props.longitude === 'string' ? parseFloat(props.longitude) : props.longitude
    return isNaN(v) ? null : v
  })
  const hasValidCoords = computed(() => lat.value !== null && lng.value !== null)

  function initMap () {
    if (!mapContainer.value || !hasValidCoords.value) return
    mapInstance = L.map(mapContainer.value, {
      center: [lat.value!, lng.value!],
      zoom: props.zoom,
      zoomControl: !props.readonly,
      dragging: !props.readonly,
      scrollWheelZoom: !props.readonly,
      doubleClickZoom: !props.readonly,
      touchZoom: !props.readonly,
      keyboard: !props.readonly,
    })
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    }).addTo(mapInstance)
    L.marker([lat.value!, lng.value!], { icon: markerIcon }).addTo(mapInstance)
  }

  function destroyMap () {
    if (mapInstance) {
      mapInstance.remove()
      mapInstance = null
    }
  }

  onMounted(() => {
    initMap()
  })

  onUnmounted(() => {
    destroyMap()
  })

  watch([lat, lng], () => {
    destroyMap()
    initMap()
  })
</script>

<style scoped>
.map-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  position: relative;
  z-index: 0;
}
.map-empty {
  padding: 20px;
  text-align: center;
  color: #999;
  background: #f5f5f5;
  border-radius: 8px;
}
</style>
