<template>
  <div class="location-picker">
    <div ref="mapContainer" class="map-container rounded-lg"></div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { loadGoogleMapsScript } from "@/plugins/google-maps";

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({ lat: 24.7136, lng: 46.6753 }), // Default to Riyadh coordinates
  },
  isDraggable: {
    type: Boolean,
    default: true,
  },
  isClickable: {
    type: Boolean,
    default: true,
  },
});

const emit = defineEmits(["update:modelValue"]);

const mapContainer = ref(null);
let map = null;
let marker = null;

const initMap = async () => {
  try {
    const maps = await loadGoogleMapsScript();

    // Create the map instance
    map = new maps.Map(mapContainer.value, {
      center: {
        lat: parseFloat(props.modelValue.lat),
        lng: parseFloat(props.modelValue.lng),
      },
      zoom: 15,
      mapTypeControl: true,
      streetViewControl: false,
      fullscreenControl: false,
      mapTypeId: maps.MapTypeId.ROADMAP,
      styles: [
        {
          featureType: "poi",
          elementType: "labels",
          stylers: [{ visibility: "off" }],
        },
      ],
    });

    // Create the marker
    marker = new maps.Marker({
      position: {
        lat: parseFloat(props.modelValue.lat),
        lng: parseFloat(props.modelValue.lng),
      },
      map: map,
      draggable: props.isDraggable,
      animation: maps.Animation.DROP,
    });

    // Add search box
    const input = document.createElement("input");
    input.className = "location-search-input";
    input.placeholder = "ابحث عن موقع...";
    map.controls[maps.ControlPosition.TOP_RIGHT].push(input);

    const searchBox = new maps.places.SearchBox(input);

    // Listen for search box events
    searchBox.addListener("places_changed", () => {
      const places = searchBox.getPlaces();
      if (places.length === 0) return;

      const place = places[0];
      if (!place.geometry || !place.geometry.location) return;

      // Update map and marker
      map.setCenter(place.geometry.location);
      marker.setPosition(place.geometry.location);
      updateCoordinates(place.geometry.location);
    });

    // Add click event to map
    map.addListener("click", (e) => {
      if (props.isClickable) {
        const position = e.latLng;
        marker.setPosition(position);
        updateCoordinates(position);
      }
    });

    // Add dragend event to marker
    marker.addListener("dragend", () => {
      const position = marker.getPosition();
      updateCoordinates(position);
    });
  } catch (error) {
    console.error("Failed to load Google Maps:", error);
  }
};

const updateCoordinates = (position) => {
  emit("update:modelValue", { lat: position.lat(), lng: position.lng() });
};

watch(
  () => props.modelValue,
  (newValue) => {
    if (map && marker && newValue) {
      const position = {
        lat: parseFloat(newValue.lat),
        lng: parseFloat(newValue.lng),
      };
      marker.setPosition(position);
      map.setCenter(position);
    }
  },
  { deep: true }
);

onMounted(() => {
  initMap();
});
</script>

<style scoped>
.map-container {
  width: 100%;
  height: 400px;
  background-color: #f5f5f5;
  position: relative;
}

.location-search-input {
  margin: 10px;
  padding: 8px 12px;
  border-radius: 8px;
  border: 1px solid #ddd;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  font-size: 14px;
  width: 250px;
  direction: rtl;
}

.location-picker :deep(.v-field) {
  border-radius: 8px;
  background-color: rgb(250, 250, 250);
}
</style>
