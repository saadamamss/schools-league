<script setup lang="ts">
  import { computed, ref } from 'vue'
  import { locationsApi } from '@/api'
  import { useAppStore } from '@/stores/app'

  const appStore = useAppStore()
  const isLoading = ref(false)

  interface CorrectionData {
    id: number
    created_by: Record<string, any>
    camp_number: string
    square_number: string
    contact_number: string
    updated_at: string
    status: string
    [key: string]: any
  }

  const props = withDefaults(defineProps<{
    data: CorrectionData
    loading?: boolean
    canUpdate?: boolean
  }>(), {
    loading: false,
    canUpdate: false,
  })

  const emit = defineEmits<{
    (e: 'refetch'): void
  }>()

  const handleApprove = async () => {
    try {
      isLoading.value = true
      const newStatus = props.data.status === 'rejected' ? 'pending' : 'approved'

      const response = await locationsApi.updateCorrection(props.data.id, {
        status: newStatus,
      })

      if (response.data.success === false) {
        throw new Error(response.data.message)
      }

      appStore.showSnackbar({
        message:
          newStatus === 'approved'
            ? 'تم اعتماد الموقع بنجاح'
            : 'تم التراجع عن الرفض بنجاح',
        color: 'success',
      })

      emit('refetch')
    } catch (error: any) {
      appStore.showSnackbar({
        message: error.message || 'حدث خطأ أثناء تحديث الموقع',
        color: 'error',
      })
    } finally {
      isLoading.value = false
    }
  }

  const handleReject = async () => {
    try {
      isLoading.value = true
      const newStatus = props.data.status === 'approved' ? 'pending' : 'rejected'

      const response = await locationsApi.updateCorrection(props.data.id, {
        status: newStatus,
      })

      if (response.data.success === false) {
        throw new Error(response.data.message)
      }

      appStore.showSnackbar({
        message:
          newStatus === 'rejected'
            ? 'تم رفض الموقع بنجاح'
            : 'تم التراجع عن الاعتماد بنجاح',
        color: 'success',
      })

      emit('refetch')
    } catch (error: any) {
      appStore.showSnackbar({
        message: error.message || 'حدث خطأ أثناء تحديث الموقع',
        color: 'error',
      })
    } finally {
      isLoading.value = false
    }
  }

  const formatDate = (dateString: string) => {
    if (!dateString) return 'غير محدد'

    return new Date(dateString).toLocaleDateString('ar', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    })
  }

  const getFullName = computed(() => {
    const { firstname = '', lastname = '' } = props.data.created_by || {}
    return `${firstname} ${lastname}`.trim() || 'غير محدد'
  })

  const statusText = computed(() => {
    const statusMap: Record<string, string> = {
      approved: 'معتمد',
      rejected: 'مرفوض',
      pending: 'قيد الانتظار',
    }
    return statusMap[props.data.status] || 'قيد الانتظار'
  })

  const statusTextClass = computed(() => {
    const classMap: Record<string, string> = {
      approved: 'text-success',
      rejected: 'text-error',
      pending: 'text-warning',
    }
    return classMap[props.data.status] || 'text-warning'
  })

  interface InfoItem {
    icon: string
    label: string
    value: string
    fullWidth?: boolean
    customIcon?: string
    subValue?: string
  }

  const infoItems = computed<InfoItem[]>(() => [
    {
      icon: 'mdi-account',
      label: 'المنشئ',
      value: getFullName.value,
      fullWidth: true,
    },
    {
      icon: 'mdi-tent',
      label: 'رقم المخيم',
      value: props.data.camp_number || 'غير محدد',
    },
    {
      icon: 'mdi-square-outline',
      label: 'رقم المربع',
      value: props.data.square_number || 'غير محدد',
    },
    {
      icon: 'mdi-phone',
      label: 'رقم الاتصال',
      value: props.data.contact_number || 'غير محدد',
    },
    {
      icon: 'custom-calendar',
      label: 'تاريخ التحديث',
      value: formatDate(props.data.updated_at),
      customIcon: `<svg width="23" height="24" viewBox="0 0 23 24" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path opacity="0.4" d="M20.6854 12.714V12.2385C20.6854 10.6372 20.6854 9.35368 20.6129 8.30957H3.16526C3.09277 9.35368 3.09277 10.6372 3.09277 12.2385V12.714C3.09277 16.7485 3.09277 18.7658 4.25214 20.0192C5.41151 21.2725 7.27749 21.2725 11.0094 21.2725H12.7687C16.5007 21.2725 18.3666 21.2725 19.526 20.0192C20.6854 18.7658 20.6854 16.7485 20.6854 12.714Z" fill="#667178"/>
      <path d="M7.25955 12.9392C7.25955 12.4278 7.6741 12.0133 8.18547 12.0133H8.19378C8.70516 12.0133 9.11971 12.4278 9.11971 12.9392C9.11971 13.4506 8.70516 13.8651 8.19378 13.8651H8.18547C7.6741 13.8651 7.25955 13.4506 7.25955 12.9392Z" fill="#667178"/>
      <path d="M10.9591 12.9392C10.9591 12.4278 11.3737 12.0133 11.885 12.0133H11.8933C12.4047 12.0133 12.8193 12.4278 12.8193 12.9392C12.8193 13.4506 12.4047 13.8651 11.8933 13.8651H11.885C11.3737 13.8651 10.9591 13.4506 10.9591 12.9392Z" fill="#667178"/>
      <path d="M14.6586 12.9392C14.6586 12.4278 15.0732 12.0133 15.5846 12.0133H15.5929C16.1043 12.0133 16.5188 12.4278 16.5188 12.9392C16.5188 13.4506 16.1043 13.8651 15.5929 13.8651H15.5846C15.0732 13.8651 14.6586 13.4506 14.6586 12.9392Z" fill="#667178"/>
      <path d="M7.25955 16.6429C7.25955 16.1315 7.6741 15.717 8.18547 15.717H8.19378C8.70516 15.717 9.11971 16.1315 9.11971 16.6429C9.11971 17.1543 8.70516 17.5688 8.19378 17.5688H8.18547C7.6741 17.5688 7.25955 17.1543 7.25955 16.6429Z" fill="#667178"/>
      <path d="M10.9591 16.6429C10.9591 16.1315 11.3737 15.717 11.885 15.717H11.8933C12.4047 15.717 12.8193 16.1315 12.8193 16.6429C12.8193 17.1543 12.4047 17.5688 11.8933 17.5688H11.885C11.3737 17.5688 10.9591 17.1543 10.9591 16.6429Z" fill="#667178"/>
      <path fill-rule="evenodd" clip-rule="evenodd" d="M6.33362 2.05957C6.71715 2.05957 7.02807 2.37048 7.02807 2.75401V3.09281C8.07998 2.98547 9.37439 2.98548 10.9533 2.9855H12.8251C14.404 2.98548 15.6984 2.98547 16.7503 3.09281V2.75401C16.7503 2.37048 17.0612 2.05957 17.4447 2.05957C17.8283 2.05957 18.1392 2.37048 18.1392 2.75401V3.34178C18.8843 3.55298 19.5128 3.89624 20.0359 4.46176C20.7562 5.24045 21.0764 6.22431 21.2297 7.45726C21.3799 8.66541 21.3799 10.2137 21.3799 12.1893V12.7632C21.3799 14.7388 21.3799 16.2871 21.2297 17.4952C21.0764 18.7282 20.7562 19.712 20.0359 20.4907C19.3085 21.2771 18.3774 21.6337 17.2121 21.8031C16.0843 21.967 14.6433 21.967 12.8251 21.967H10.9533C9.13506 21.967 7.6941 21.967 6.56629 21.8031C5.401 21.6337 4.46987 21.2771 3.74246 20.4907C3.02217 19.712 2.702 18.7282 2.54867 17.4952C2.39842 16.2871 2.39843 14.7388 2.39844 12.7632V12.1892C2.39843 10.2137 2.39842 8.66541 2.54867 7.45726C2.702 6.22431 3.02217 5.24045 3.74246 4.46176C4.26557 3.89624 4.89403 3.55298 5.63918 3.34178V2.75401C5.63918 2.37048 5.95009 2.05957 6.33362 2.05957ZM6.33362 5.30031C6.01619 5.30031 5.7485 5.08733 5.66566 4.79646C5.28852 4.94652 5.00217 5.14527 4.76204 5.40487C4.32477 5.8776 4.06559 6.52564 3.92863 7.61513H19.8497C19.7128 6.52564 19.4536 5.8776 19.0163 5.40487C18.7762 5.14527 18.4898 4.94653 18.1127 4.79647C18.0299 5.08733 17.7622 5.30031 17.4447 5.30031C17.0612 5.30031 16.7503 4.9894 16.7503 4.60587V4.48974C15.7679 4.37583 14.4981 4.37439 12.7688 4.37439H11.0095C9.28026 4.37439 8.01048 4.37583 7.02807 4.48974V4.60587C7.02807 4.9894 6.71715 5.30031 6.33362 5.30031ZM19.955 9.00402C19.9905 9.89002 19.991 10.9492 19.991 12.2385V12.714C19.991 14.7494 19.9898 16.2114 19.8514 17.3238C19.7149 18.4215 19.4554 19.0729 19.0163 19.5476C18.5844 20.0146 18.0028 20.2847 17.0123 20.4286C15.9959 20.5764 14.656 20.5781 12.7688 20.5781H11.0095C9.1224 20.5781 7.78247 20.5764 6.76607 20.4286C5.77555 20.2847 5.19401 20.0146 4.76204 19.5476C4.32296 19.0729 4.06345 18.4215 3.... (line truncated to 2000 chars)
    </svg>`,
    },
  ])
</script>

<template>
  <v-card class="site-card rounded-lg" elevation="0">
    <v-card-text class="pa-4">
      <!-- Status Header -->
      <div class="status-header mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <!-- Status Badge -->
          <div class="status-badge d-flex align-center gap-2">
            <div class="status-icon">
              <v-icon
                v-if="data.status === 'approved'"
                color="success"
                size="20"
              >
                mdi-check-circle
              </v-icon>
              <v-icon
                v-else-if="data.status === 'rejected'"
                color="error"
                size="20"
              >
                mdi-close-circle
              </v-icon>
              <v-icon v-else color="warning" size="20">
                mdi-clock-outline
              </v-icon>
            </div>
            <span class="status-text" :class="statusTextClass">
              {{ statusText }}
            </span>
            <span class="camp-number">{{ data.camp_number }}</span>
          </div>

          <!-- Action Buttons -->
          <div v-if="canUpdate" class="action-buttons d-flex gap-2">
            <!-- Show Approve button if status is rejected or pending -->
            <v-btn
              v-if="['rejected', 'pending'].includes(data.status)"
              class="text-none font-weight-medium px-4 rounded-pill"
              color="success"
              :loading="loading"
              size="small"
              variant="outlined"
              @click="handleApprove"
            >
              <span class="me-2">
                {{ data.status === "rejected" ? "تراجع عن الرفض" : "اعتماد" }}
              </span>
              <v-icon size="small">mdi-check</v-icon>
            </v-btn>

            <!-- Show Reject button if status is approved or pending -->
            <v-btn
              v-if="['approved', 'pending'].includes(data.status)"
              class="text-none font-weight-medium px-4 rounded-pill"
              color="error"
              :loading="loading"
              size="small"
              variant="outlined"
              @click="handleReject"
            >
              <span class="me-2">
                {{ data.status === "approved" ? "تراجع عن الاعتماد" : "رفض" }}
              </span>
              <v-icon size="small">mdi-close</v-icon>
            </v-btn>
          </div>
        </div>
      </div>

      <!-- Info Grid -->
      <div class="site-info">
        <div class="info-grid">
          <template v-for="(item, index) in infoItems" :key="`info-${index}`">
            <div class="info-item" :data-full-width="item.fullWidth">
              <div class="info-label">
                <v-icon v-if="!item.customIcon" color="primary" size="small">{{
                  item.icon
                }}</v-icon>
                <span
                  v-else
                  class="custom-icon"
                  v-html="item.customIcon"
                />
                <span>{{ item.label }}</span>
              </div>
              <div class="info-value">{{ item.value }}</div>
              <div v-if="item.subValue" class="info-sub-value">
                {{ item.subValue }}
              </div>
            </div>
          </template>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<style lang="scss" scoped>
.site-card {
  background-color: rgb(var(--v-theme-surface));
  transition: all 0.3s ease;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  .status-header {
    border-bottom: 1px solid rgba(var(--v-theme-on-surface), 0.08);
    padding-bottom: 1rem;
  }

  .status-badge {
    .status-icon {
      display: flex;
      align-items: center;
    }

    .status-text {
      font-weight: 500;
      font-size: 0.9rem;
    }

    .camp-number {
      font-weight: 500;
      color: rgba(var(--v-theme-on-surface), 0.6);
      margin-inline-start: 0.5rem;
      padding-inline-start: 0.5rem;
      border-inline-start: 2px solid rgba(var(--v-theme-on-surface), 0.08);
    }
  }

  .text-success {
    color: #65b669;
  }

  .text-error {
    color: #f53d6b;
  }

  .text-warning {
    color: #fb8c00;
  }

  .info-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    padding: 1rem;
    background-color: rgb(var(--v-theme-background));
    border-radius: 8px;
    margin-top: 1rem;
  }

  .info-item {
    &[data-full-width="true"] {
      grid-column: 1 / -1;
      border-bottom: 1px solid rgba(var(--v-theme-on-surface), 0.08);
      padding-bottom: 1rem;
      margin-bottom: 0.5rem;
    }

    .info-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: rgba(var(--v-theme-on-surface), 0.6);
      margin-bottom: 0.25rem;
      font-size: 0.875rem;
    }

    .info-value {
      font-weight: 500;
      color: rgba(var(--v-theme-on-surface), 0.87);
    }

    .info-sub-value {
      font-size: 0.75rem;
      color: rgba(var(--v-theme-on-surface), 0.6);
      margin-top: 0.25rem;
    }
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

.custom-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
}

.custom-icon svg {
  width: 20px;
  height: 20px;
}
</style>
