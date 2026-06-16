<template>
  <router-link :to="link" class="text-decoration-none">
    <v-card elevation="0" class="rounded-lg border border-grey-lighten-2 h-100">
      <v-card-text class="pa-4">
        <div class="d-flex flex-column h-100">
          <!-- Header -->
          <div class="d-flex justify-space-between align-center mb-4">
            <div class="d-flex align-center">
              <v-avatar color="grey-lighten-4" size="48">
                <v-icon color="primary" size="24"
                  >mdi-clipboard-check-outline</v-icon
                >
              </v-avatar>
              <div class="ms-3">
                <h3 class="text-subtitle-1 font-weight-bold mb-1">
                  {{ authorizationNumber }}
                </h3>
                <p class="text-caption text-grey mb-0">#{{ siteId }}</p>
              </div>
            </div>
          </div>

          <!-- Status -->
          <div class="d-flex align-center justify-space-between mt-auto">
            <v-chip
              :color="status === 'PENDING' ? 'warning' : 'success'"
              size="small"
            >
              {{ status === "PENDING" ? "قيد المراجعة" : "مكتمل" }}
            </v-chip>
            <span class="text-caption text-grey">{{ createdAt }}</span>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </router-link>
</template>

<script setup lang="ts">
import { defineProps } from "vue";

const props = defineProps({
  siteId: {
    type: [String, Number],
    required: true,
  },
  authorizationNumber: {
    type: String,
    required: true,
  },
  status: {
    type: String,
    default: "PENDING",
  },
  createdAt: {
    type: String,
    required: true,
  },
  link: {
    type: String,
    required: true,
  },
});
</script>

<style lang="scss" scoped>
.v-card {
  transition: all 0.3s ease;
  cursor: pointer;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 4px 25px 0 rgba(0, 0, 0, 0.1) !important;
  }
}
</style>
