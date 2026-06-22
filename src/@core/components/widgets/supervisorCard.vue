<template>
  <v-card class="Supervisor-card rounded-lg" elevation="0">
    <v-card-text
      class="pa-4 pb-0"
      style="cursor: pointer"
      @click="$router.push('/sites/company-details')"
    >
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <h2 class="text-h6 font-weight-medium">المشرف</h2>
        </div>
      </div>

      <div class="d-flex align-center justify-space-between border-t">
        <UserWidget
          class="flex-grow-1"
          no-shadow
          :user="{ name: 'محمد أحمد', job: 'مشرف الموقع' }"
        />

        <div
          class="company-info pa-2 d-flex align-center justify-space-between bg-light-gray-1 rounded-lg"
        >
          <div class="d-flex align-center gap-4 rounded-lg">
            <div class="d-flex justify-space-between align-center">
              <div class="d-flex align-start ga-2 ms-0">
                <svg
                  fill="none"
                  height="18"
                  viewBox="0 0 16 18"
                  width="16"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                    fill="#667178"
                    opacity="0.4"
                  />
                  <path
                    clip-rule="evenodd"
                    d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                    fill="#667178"
                    fill-rule="evenodd"
                  />
                  <path
                    d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                    fill="#667178"
                  />
                </svg>

                <div>
                  <div class="text-body-2 text-medium-emphasis">
                    رقم التصريح
                  </div>
                  <div class="text-body-2 font-weight-medium">{{ siteId }}</div>
                </div>
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
                    fill="#667178"
                  />
                  <path
                    d="M11.6182 14.6033C10.706 14.4193 9.80617 14.1141 8.7041 13.4821L10.6617 11.7778C11.3764 11.1556 11.7338 10.8444 12.1726 10.7987C12.6114 10.753 13.0253 10.9836 13.8531 11.445L14.8974 12.0272C15.36 12.285 15.4814 12.7849 15.126 13.2209C14.2852 14.2526 12.9673 14.8754 11.6182 14.6033Z"
                    fill="#667178"
                  />
                </g>
                <path
                  clip-rule="evenodd"
                  d="M4.21053 0.857753C4.58832 0.935833 4.88919 1.18966 5.07611 1.52501L5.67159 2.59331C5.89093 2.98678 6.07551 3.3179 6.19605 3.60575C6.32392 3.91109 6.39987 4.2122 6.36515 4.54511C6.33043 4.87801 6.19399 5.15697 6.00588 5.42937C5.82854 5.68615 5.57961 5.97207 5.28381 6.31182L3.79857 8.01784C4.97647 9.94851 6.71576 11.6886 8.64842 12.8676L10.3544 11.3824C10.6941 11.0866 10.9801 10.8377 11.2369 10.6604C11.5093 10.4722 11.7882 10.3358 12.1211 10.3011C12.454 10.2664 12.7551 10.3423 13.0605 10.4702C13.3483 10.5907 13.6795 10.7753 14.0729 10.9946L15.1412 11.5901C15.4765 11.777 15.7304 12.0779 15.8085 12.4557C15.8873 12.8374 15.771 13.2211 15.5139 13.5365C14.5813 14.6807 13.0871 15.4093 11.5197 15.0931C10.5562 14.8987 9.60555 14.5749 8.45575 13.9155C6.14549 12.5906 4.07411 10.518 2.75072 8.21051C2.09131 7.06071 1.7675 6.11004 1.57314 5.14653C1.25694 3.57905 1.9855 2.08487 3.12974 1.15226C3.44509 0.895234 3.82879 0.778853 4.21053 0.857753ZM9.57981 13.3826C10.3574 13.7658 11.0337 13.9749 11.7174 14.1128C12.8481 14.3409 13.9896 13.8239 14.7388 12.9047C14.8372 12.784 14.8378 12.6998 14.8291 12.6581C14.8197 12.6125 14.7815 12.5345 14.6543 12.4636L13.61 11.8814C13.1862 11.6452 12.9041 11.4888 12.6742 11.3926C12.4559 11.3012 12.3307 11.2846 12.2248 11.2957C12.1189 11.3067 11.9998 11.3488 11.8051 11.4832C11.6 11.6248 11.3563 11.8361 10.9903 12.1546L9.57981 13.3826ZM3.28359 7.08644L4.51158 5.67588C4.83013 5.30997 5.04139 5.06621 5.18303 4.8611C5.31749 4.66641 5.3595 4.54728 5.37054 4.44139C5.38159 4.3355 5.36506 4.21026 5.27367 3.99202C5.17738 3.76211 5.02097 3.47999 4.78476 3.05623L4.20264 2.01189C4.13172 1.88466 4.05373 1.84648 4.00813 1.83705C3.96645 1.82844 3.88221 1.82905 3.76152 1.92741C2.84233 2.67659 2.32531 3.81809 2.55339 4.94879C2.69131 5.63251 2.90044 6.30883 3.28359 7.08644Z"
                  fill="#667178"
                  fill-rule="evenodd"
                />
              </svg>

              <div>
                <div class="text-body-2 text-medium-emphasis">رقم المفوض</div>
                <div class="text-body-2 font-weight-medium">
                  {{ authorizationNumber }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-card-text>

    <!-- Location Correction Modal -->
    <v-dialog v-model="dialog" class="location-dialog" width="500">
      <v-card class="rounded-lg">
        <v-card-text class="pa-6">
          <div class="d-flex justify-space-between align-center mb-6">
            <h2 class="text-h6">إضافة موقع جديد</h2>
            <v-btn icon variant="text" @click="dialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
          <p class="text-subtitle-1 text-medium-emphasis mb-6">
            حدد تفاصيل الموقع الجديد
          </p>
          <v-form @submit.prevent="handleSubmit">
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="siteType"
                class="rounded-lg"
                density="comfortable"
                hide-details
                :items="siteTypes"
                placeholder="اختر نوع الموقع"
                variant="outlined"
              />
            </div>
            <div class="d-flex justify-end mt-6">
              <v-btn
                class="me-4"
                color="grey-darken-1"
                variant="text"
                @click="dialog = false"
              >
                إلغاء
              </v-btn>
              <v-btn
                class="text-white"
                color="#667178"
                :loading="loading"
                type="submit"
              >
                التالي
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script setup lang="ts">
  import { ref } from 'vue'
  import UserWidget from './UserWidget.vue'

  defineProps({
    siteId: {
      type: String,
      required: true,
    },
    authorizationNumber: {
      type: String,
      required: true,
    },
  })

  const dialog = ref(false)
  const loading = ref(false)
  const siteType = ref(null)
  const siteTypes = [
    { title: 'موقع سكني', value: 'residential' },
    { title: 'موقع تجاري', value: 'commercial' },
    { title: 'موقع صناعي', value: 'industrial' },
  ]

  const handleSubmit = async () => {
    loading.value = true
    // Add your submit logic here
    await new Promise(resolve => setTimeout(resolve, 1000))
    loading.value = false
    dialog.value = false
  }
</script>

<style lang="scss" scoped>
.Supervisor-card {
  // background-color: rgb(250, 250, 250) !important;
  // border: 1px solid rgba(182, 146, 101, 0.12);
  transition: all 0.3s ease;
  background-color: rgb(var(--v-theme-surface)) !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }

  .text-h6 {
    font-size: 1.1rem !important;
    line-height: 1.5;
  }

  .company-info {
    padding-top: 10px;
    border-top: 1px solid rgba(var(--v-theme-on-surface), 0.08);
  }
}

.location-dialog {
  :deep(.v-card) {
    background-color: rgb(var(--v-theme-surface));
  }

  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(var(--v-theme-background));
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
  }
}
</style>
