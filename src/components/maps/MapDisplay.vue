<template>
  <div class="map-container" :style="{ height: height }">
    <div ref="mapRef" style="width: 100%; height: 100%"></div>
    <div v-if="isLoading" class="map-loading">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
      <p class="mt-2">جاري تحميل الخريطة...</p>
    </div>
    <div v-if="loadingError" class="map-error">
      <p>{{ loadingError }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { loadGoogleMapsScript } from "@/plugins/google-maps";

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
    default: "400px",
  },
  zoom: {
    type: Number,
    default: 15,
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:latitude", "update:longitude", "error"]);

const mapRef = ref(null);
const loadingError = ref(null);
const isLoading = ref(true);
let map = null;
let marker = null;

const initMap = () => {
  try {
    const lat =
      typeof props.latitude === "string"
        ? parseFloat(props.latitude)
        : props.latitude;
    const lng =
      typeof props.longitude === "string"
        ? parseFloat(props.longitude)
        : props.longitude;

    // Check for valid coordinates
    if (isNaN(lat) || isNaN(lng) || !isFinite(lat) || !isFinite(lng)) {
      throw new Error("Invalid coordinates");
    }

    const position = { lat, lng };

    map = new google.maps.Map(mapRef.value, {
      center: position,
      zoom: props.zoom,
      streetViewControl: false,
      mapTypeControl: false,
    });

    marker = new google.maps.Marker({
      position,
      map,
      animation: google.maps.Animation.DROP,
      draggable: !props.readonly,
    });

    if (!props.readonly) {
      // Handle map clicks
      map.addListener("click", (e) => {
        const newPosition = {
          lat: e.latLng.lat(),
          lng: e.latLng.lng(),
        };
        marker.setPosition(newPosition);
        emit("update:latitude", newPosition.lat);
        emit("update:longitude", newPosition.lng);
      });

      // Handle marker drag
      marker.addListener("dragend", () => {
        const position = marker.getPosition();
        emit("update:latitude", position.lat());
        emit("update:longitude", position.lng());
      });
    }
  } catch (error) {
    console.error("Error initializing map:", error);
    loadingError.value = "Could not initialize map. Please try again.";
    emit("error", error);
  }
};

const updateMarkerPosition = () => {
  if (!map || !marker) return;

  const lat =
    typeof props.latitude === "string"
      ? parseFloat(props.latitude)
      : props.latitude;
  const lng =
    typeof props.longitude === "string"
      ? parseFloat(props.longitude)
      : props.longitude;

  const position = { lat, lng };
  marker.setPosition(position);
  map.setCenter(position);
};

watch(
  () => [props.latitude, props.longitude],
  () => {
    updateMarkerPosition();
  }
);

onMounted(async () => {
  try {
    isLoading.value = true;
    await loadGoogleMapsScript();
    if (window.google && window.google.maps) {
      initMap();
      isLoading.value = false;
    } else {
      isLoading.value = false;
      loadingError.value =
        "Google Maps could not be loaded. Please check your internet connection.";
      emit("error", new Error("Google Maps not loaded"));
    }
  } catch (error) {
    isLoading.value = false;
    console.error("Error loading Google Maps:", error);
    loadingError.value = "Error loading Google Maps. Please try again later.";
    emit("error", error);
  }
});
</script>

<style scoped>
.map-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  position: relative;
}

.map-error {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgba(245, 245, 245, 0.9);
  color: #d32f2f;
  text-align: center;
  padding: 20px;
}

.map-loading {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-color: rgba(255, 255, 255, 0.8);
  z-index: 1;
}
</style>
