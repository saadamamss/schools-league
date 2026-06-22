<template>
  <v-card class="site-card rounded-lg" elevation="0">
    <v-card-text class="pa-4">
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <div
            class="font-weight-medium d-flex align-center ga-1"
            style="font-size: 0.9rem; max-width: 170px"
          >
            <span v-if="status == 'active'">
              <svg
                fill="none"
                height="16"
                viewBox="0 0 15 16"
                width="15"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M0.147461 8.00004C0.147461 11.958 3.35613 15.1667 7.31413 15.1667C11.2721 15.1667 14.4808 11.958 14.4808 8.00004C14.4808 4.04204 11.2721 0.833374 7.31413 0.833374C3.35613 0.833374 0.147461 4.04204 0.147461 8.00004Z"
                  fill="#65B669"
                  opacity="0.4"
                />
                <path
                  d="M7.31411 9.33331C7.13744 9.33331 6.96812 9.2633 6.84279 9.13797L4.50945 6.80463C4.24945 6.54397 4.24945 6.12199 4.50945 5.86199C4.77012 5.60132 5.1921 5.60132 5.4521 5.86199L7.28878 7.69864L12.8188 1.55398C13.0648 1.28065 13.4868 1.25797 13.7601 1.50463C14.0341 1.75063 14.0561 2.17264 13.8095 2.44598L7.80945 9.11264C7.68679 9.24864 7.51409 9.32798 7.33143 9.33331C7.32543 9.33331 7.31944 9.33331 7.31411 9.33331Z"
                  fill="#65B669"
                />
              </svg>
            </span>
            <span v-else class="flex-shrink-0">
              <svg
                fill="none"
                height="16"
                viewBox="0 0 16 16"
                width="16"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M0.833008 8.00008C0.833008 11.9581 4.04163 15.1667 7.99967 15.1667C11.9577 15.1667 15.1663 11.9581 15.1663 8.00008C15.1663 4.04204 11.9577 0.833414 7.99967 0.833414C4.04163 0.833414 0.833008 4.04204 0.833008 8.00008Z"
                  fill="#F53D6B"
                  opacity="0.4"
                />
                <path
                  clip-rule="evenodd"
                  d="M10.4711 5.52865C10.7314 5.78901 10.7314 6.21112 10.4711 6.47146L8.94238 8.00004L10.4711 9.52862C10.7314 9.78896 10.7314 10.2111 10.4711 10.4714C10.2108 10.7318 9.78864 10.7318 9.52829 10.4715L7.99954 8.94282L6.47106 10.4712C6.21071 10.7315 5.7886 10.7315 5.52825 10.4712C5.26791 10.2108 5.26793 9.7887 5.52829 9.52836L7.0567 8.00004L5.52829 6.47172C5.26793 6.21138 5.26791 5.78927 5.52825 5.52891C5.7886 5.26856 6.21071 5.26854 6.47106 5.52888L7.99954 7.05726L9.52829 5.52862C9.78864 5.26828 10.2108 5.26829 10.4711 5.52865Z"
                  fill="#F53D6B"
                  fill-rule="evenodd"
                />
              </svg>
            </span>
            <span class="mt-n2 text-truncate" style="max-width: 200px">{{
              name
            }}</span>
          </div>

          <!-- open dialog have map take lat and long latitude and longitude -->
          <button @click="openMapDialog">
            <svg
              fill="none"
              height="32"
              viewBox="0 0 33 32"
              width="33"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M0.666992 16C0.666992 7.16344 7.83044 0 16.667 0C25.5035 0 32.667 7.16344 32.667 16C32.667 24.8366 25.5035 32 16.667 32C7.83044 32 0.666992 24.8366 0.666992 16Z"
                fill="#F9F9F9"
              />
              <path
                d="M9.5 15.2839C9.5 11.7397 12.4839 8.83337 16.002 8.83337C19.2879 8.83337 22.1078 11.3688 22.4657 14.5912C22.4878 14.791 22.4989 14.8908 22.4376 14.9365C22.3763 14.9822 22.2764 14.9398 22.0768 14.8551C21.5379 14.6264 20.9488 14.5 20.3366 14.5C19.8496 14.5 19.3773 14.58 18.9333 14.7278C18.7882 14.7761 18.7156 14.8002 18.6659 14.775C18.6162 14.7497 18.5922 14.6743 18.5442 14.5236C18.2013 13.4467 17.1928 12.6667 16.002 12.6667C14.5288 12.6667 13.3345 13.8606 13.3345 15.3334C13.3345 16.676 14.3271 17.7869 15.6186 17.9727C15.774 17.9951 15.8517 18.0062 15.8848 18.0507C15.9179 18.0953 15.9066 18.1704 15.884 18.3208C15.8519 18.5343 15.8352 18.7529 15.8352 18.9753C15.8352 20.264 16.3878 21.3008 17.0601 22.0825C17.1254 22.1584 17.1924 22.2326 17.2606 22.3049C17.3688 22.4196 17.4229 22.4769 17.415 22.5393C17.4072 22.6017 17.343 22.6423 17.2145 22.7236C17.103 22.7942 16.9918 22.8633 16.8812 22.931L16.8737 22.9356L16.866 22.9399C16.6024 23.0887 16.3047 23.1667 16.002 23.1667C15.6992 23.1667 15.4015 23.0887 15.1379 22.9399L15.1291 22.935L15.1205 22.9296C13.8135 22.1208 12.4175 21.1152 11.3452 19.8684C10.2681 18.6161 9.5 17.0989 9.5 15.2839Z"
                fill="#B69265"
                opacity="0.4"
              />
              <path
                clip-rule="evenodd"
                d="M20.333 15.5C18.4395 15.5 16.833 17.062 16.833 18.9753C16.833 19.9589 17.2506 20.7732 17.8157 21.4305C18.3762 22.0824 19.0991 22.601 19.7608 23.0107L19.7694 23.016L19.7782 23.021C19.9475 23.1166 20.1387 23.1667 20.333 23.1667C20.5273 23.1667 20.7185 23.1166 20.8878 23.021L20.8955 23.0166L20.903 23.0121C21.5683 22.6049 22.2914 22.0845 22.8517 21.4306C23.4163 20.7718 23.833 19.9558 23.833 18.9753C23.833 17.062 22.2265 15.5 20.333 15.5ZM20.327 17.6667C19.594 17.6667 18.9997 18.2636 18.9997 19C18.9997 19.7364 19.594 20.3333 20.327 20.3333H20.339C21.0721 20.3333 21.6663 19.7364 21.6663 19C21.6663 18.2636 21.0721 17.6667 20.339 17.6667H20.327Z"
                fill="#B69265"
                fill-rule="evenodd"
              />
            </svg>
          </button>
          <!-- <div class="d-flex align-center ga-2 flex-shrink-0">
            <v-btn
              :to="`/sites/housing-details?siteId=${siteId}`"
              size="small"
              variant="tonal"
              class="text-none font-weight-medium px-2 me-2 bg-primary-100 rounded-pill"
            >
              <span class="me-2"> عرض </span>
              <svg
                width="17"
                height="16"
                viewBox="0 0 17 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M2.66602 7.99992C2.66602 7.63173 2.96449 7.33325 3.33268 7.33325L13.9994 7.33325C14.3675 7.33325 14.666 7.63173 14.666 7.99992C14.666 8.36811 14.3675 8.66659 13.9994 8.66659L3.33268 8.66659C2.96449 8.66659 2.66602 8.36811 2.66602 7.99992Z"
                  fill="#B69265"
                />
                <path
                  d="M4.01792 7.99988C4.08012 8.10306 4.21052 8.28847 4.3531 8.44984C4.63693 8.77106 5.02808 9.13798 5.43642 9.49251C5.84144 9.84416 6.24841 10.1708 6.55533 10.4103C6.70844 10.5298 6.97265 10.7294 7.0614 10.7964C7.35783 11.0148 7.42114 11.4321 7.20282 11.7285C6.98448 12.025 6.56714 12.0883 6.27068 11.87L6.2682 11.8681C6.17195 11.7954 5.89287 11.5846 5.73503 11.4614C5.41695 11.2132 4.99059 10.8712 4.56228 10.4993C4.13728 10.1303 3.6951 9.71881 3.35393 9.33269C3.18401 9.14039 3.02506 8.93868 2.90497 8.73948C2.79591 8.55858 2.66603 8.29535 2.66602 7.9999C2.66603 7.70446 2.79591 7.44118 2.90497 7.26028C3.02506 7.06107 3.18401 6.85936 3.35393 6.66707C3.6951 6.28095 4.13728 5.86944 4.56228 5.50044C4.99059 5.12857 5.41695 4.78656 5.73503 4.53832C5.89287 4.41515 6.17177 4.2045 6.26802 4.1318L6.27068 4.12979C6.56714 3.91145 6.98448 3.97479 7.20281 4.27125C7.42114 4.5677 7.35783 4.985 7.0614 5.20335C6.97265 5.27039 6.70844 5.46998 6.55533 5.58946C6.24841 5.82898 5.84144 6.15559 5.43642 6.50725C5.02808 6.86177 4.63693 7.2287 4.3531 7.54992C4.21052 7.71129 4.08012 7.8967 4.01792 7.99988Z"
                  fill="#B69265"
                />
              </svg>
            </v-btn>
          </div> -->
        </div>
      </div>

      <div class="d-flex align-center mb-2 mt-n4">
        <svg
          fill="none"
          height="17"
          viewBox="0 0 17 17"
          width="17"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M4.87994 1.45131C5.28679 1.36194 5.77226 1.33325 6.33464 1.33325L10.3346 1.33325C10.897 1.33325 11.3825 1.36194 11.7893 1.45131C12.2019 1.54194 12.5648 1.70164 12.8489 1.9857C13.1329 2.26976 13.2926 2.6327 13.3832 3.04522C13.4726 3.45208 13.5013 3.93754 13.5013 4.49992L13.5013 15.1666L3.16797 15.1666L3.16797 4.49992C3.16797 3.93754 3.19665 3.45208 3.28603 3.04522C3.37665 2.6327 3.53635 2.26976 3.82042 1.9857C4.10448 1.70164 4.46742 1.54194 4.87994 1.45131Z"
            fill="#B69265"
            opacity="0.4"
          />
        </svg>

        <span class="card-gray-text">{{ companiesCount }} شركه</span>
      </div>

      <div class="site-info mt-6 d-flex align-center justify-space-between">
        <div class="d-flex justify-space-between align-center">
          <div class="d-flex align-start ga-2">
            <svg
              fill="none"
              height="18"
              viewBox="0 0 16 18"
              width="16"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                fill="#B69265"
                opacity="0.4"
              />
            </svg>

            <div>
              <div class="text-body-2 text-medium-emphasis">رقم البريد</div>
              <div class="text-body-2 font-weight-medium">{{ licenseNo }}</div>
            </div>
          </div>
        </div>
        <div class="d-flex justify-space-between align-center">
          <div class="d-flex align-start ga-2">
            <svg
              fill="none"
              height="16"
              viewBox="0 0 17 16"
              width="17"
              xmlns="http://www.w3.org/2000/svg"
            >
              <g opacity="0.4">
                <path
                  d="M2.06292 5.04751C2.24692 5.95969 2.55209 6.85958 3.1841 7.96164L4.88834 6.00405C5.51059 5.28929 5.82174 4.9319 5.86749 4.4931C5.91325 4.0543 5.68255 3.64042 5.22115 2.81265L4.63902 1.76831C4.38118 1.30572 3.88132 1.18429 3.44528 1.53969C2.41356 2.38058 1.79077 3.69843 2.06292 5.04751Z"
                  fill="#B69265"
                />
              </g>
            </svg>

            <div>
              <div class="text-body-2 text-medium-emphasis">رقم المفوض</div>
              <div class="text-body-2 font-weight-medium">
                {{ delegatePhone }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-card-text>
  </v-card>

  <!-- Map Dialog -->
  <v-dialog v-model="mapDialog" max-width="800">
    <v-card>
      <v-card-title class="d-flex justify-space-between align-center pa-4">
        <span>موقع {{ name }}</span>
        <v-btn icon @click="mapDialog = false">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>
      <v-card-text class="pa-0">
        <MapDisplay
          height="500px"
          :latitude="latitude"
          :longitude="longitude"
          :readonly="true"
          @error="handleMapError"
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
  import { ref } from 'vue'
  import MapDisplay from '@/components/maps/MapDisplay.vue'

  const props = defineProps({
    siteId: {
      type: [String, Number],
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    licenseNo: {
      type: String,
      required: true,
    },
    delegatePhone: {
      type: String,
      required: true,
    },
    companiesCount: {
      type: Number,
      default: 0,
    },
    locationType: {
      type: Object,
      default: () => ({
        title: '',
        description: '',
      }),
    },
    status: {
      type: String,
      default: 'active',
    },
    latitude: {
      type: Number,
      required: true,
    },
    longitude: {
      type: Number,
      required: true,
    },
  })

  const emit = defineEmits(['update'])

  // Dialog state
  const mapDialog = ref(false)

  // Open map dialog
  const openMapDialog = () => {
    mapDialog.value = true
  }

  // Handle map loading errors
  const handleMapError = (error: any) => {
    console.warn('Map error:', error)
  }
</script>

<style lang="scss" scoped>
.site-card {
  background-color: rgb(var(--v-theme-surface));
  transition: all 0.3s ease;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  .site-info {
    background-color: rgb(var(--v-theme-background));
    border-radius: 8px;
    padding: 1rem;
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }
}

.ga-1 {
  gap: 0.25rem;
}

.ga-2 {
  gap: 0.5rem;
}
</style>
