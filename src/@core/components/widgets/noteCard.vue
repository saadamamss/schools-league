<template>
  <v-card
    class="rounded-lg h-100 cursor-pointer"
    elevation="1"
    @click="navigateToDetails"
  >
    <v-card-text class="pa-4 h-100">
      <!-- Header with Status -->
      <!-- <div class="d-flex justify-space-between align-center mb-4"> -->
      <div class="d-flex align-center justify-space-between border-b pb-4">
        <div class="d-flex align-center gap-2">
          <svg
            fill="none"
            height="24"
            viewBox="0 0 24 24"
            width="24"
            xmlns="http://www.w3.org/2000/svg"
          >
            <rect fill="#667178" height="24" rx="12" width="24" />
            <path
              d="M13.5693 6.84557C13.7522 6.66268 13.8436 6.57124 13.8133 6.47208C13.7829 6.37292 13.6618 6.34951 13.4196 6.30268C13.2094 6.26205 12.9947 6.23407 12.7763 6.21955C11.9367 6.16374 11.0608 6.16385 10.2229 6.21955C7.59709 6.3941 5.5137 8.51372 5.3424 11.1643C5.31035 11.6602 5.31035 12.1732 5.3424 12.6691C5.40618 13.656 5.83671 14.5463 6.3153 15.2687C6.45866 15.5456 6.38397 15.9608 6.11583 16.469L6.10348 16.4924C6.01338 16.663 5.92519 16.83 5.87432 16.9724C5.82041 17.1233 5.76469 17.3629 5.90366 17.5998C6.03134 17.8175 6.23534 17.8973 6.40024 17.9292C6.53713 17.9557 6.705 17.9597 6.86249 17.9634L6.88878 17.9641C7.70514 17.9839 8.28039 17.7482 8.73605 17.4122C8.75558 17.3978 8.7738 17.3844 8.79082 17.3719C8.85955 17.3213 8.89391 17.2961 8.93858 17.2905C8.98326 17.285 9.02253 17.301 9.10106 17.333C9.13996 17.3488 9.1837 17.3668 9.23318 17.3872C9.54448 17.5154 9.89807 17.5922 10.2229 17.6138C11.0608 17.6695 11.9367 17.6697 12.7763 17.6138C15.4021 17.4393 17.4855 15.3197 17.6568 12.6691C17.6889 12.1732 17.6889 11.6602 17.6568 11.1643C17.619 10.5791 17.488 10.0197 17.2784 9.50141C17.1999 9.30718 17.1606 9.21007 17.0698 9.1916C16.979 9.17314 16.9015 9.25065 16.7465 9.40567L15.4952 10.657C15.15 11.0021 14.7422 11.1533 14.416 11.2366C14.2533 11.2781 14.0933 11.3069 13.9602 11.3299L13.8871 11.3424C13.779 11.3609 13.6914 11.3759 13.6048 11.3945L13.5692 11.4021C13.4041 11.4375 13.1979 11.4818 13.0189 11.4932C12.8113 11.5064 12.4237 11.4924 12.1074 11.1761C11.7911 10.8598 11.7771 10.4722 11.7903 10.2646C11.8017 10.0856 11.846 9.87941 11.8814 9.71428L11.8891 9.6787C11.9076 9.59214 11.9226 9.50449 11.9411 9.39637L11.9536 9.32335C11.9766 9.19026 12.0054 9.03021 12.0469 8.86751C12.1302 8.5413 12.2814 8.13346 12.6265 7.78834L13.5693 6.84557Z"
              fill="white"
              opacity="0.4"
            />
            <path
              d="M15.1389 6.49641L13.2369 8.39842C12.9359 8.69942 12.8636 9.12184 12.7927 9.53662C12.774 9.64546 12.7555 9.75378 12.733 9.85924C12.6481 10.256 12.6057 10.4543 12.7178 10.5664C12.8299 10.6785 13.0282 10.636 13.4249 10.5512C13.5304 10.5287 13.6387 10.5101 13.7476 10.4915C14.1623 10.4206 14.5848 10.3483 14.8858 10.0473L16.7878 8.14529C16.9794 7.95361 17.0753 7.85777 17.1328 7.75807C17.2891 7.48752 17.2891 7.15418 17.1328 6.88362C17.0753 6.78392 16.9794 6.68809 16.7878 6.49641C16.5961 6.30474 16.5003 6.20889 16.4006 6.15133C16.13 5.99513 15.7967 5.99513 15.5261 6.15133C15.4264 6.20889 15.3306 6.30473 15.1389 6.49641Z"
              fill="white"
            />
            <path
              d="M9.2 11.4967C8.88244 11.4967 8.625 11.7541 8.625 12.0717C8.625 12.3892 8.88244 12.6467 9.2 12.6467H9.20516C9.52272 12.6467 9.78016 12.3892 9.78016 12.0717C9.78016 11.7541 9.52272 11.4967 9.20516 11.4967H9.2Z"
              fill="white"
            />
            <path
              d="M11.4974 11.4967C11.1799 11.4967 10.9224 11.7541 10.9224 12.0717C10.9224 12.3892 11.1799 12.6467 11.4974 12.6467H11.5026C11.8201 12.6467 12.0776 12.3892 12.0776 12.0717C12.0776 11.7541 11.8201 11.4967 11.5026 11.4967H11.4974Z"
              fill="white"
            />
          </svg>

          {{ id }}
        </div>
        <div class="">
          <v-chip
            class="font-weight-medium"
            :color="getStatusColor(status)"
            size="small"
          >
            {{ getStatusText(status) }}
          </v-chip>
        </div>
      </div>
      <!-- <div class="text-caption text-medium-emphasis">
          {{ formatDate(report_date) }}
        </div> -->
      <!-- </div> -->

      <!-- Note Type -->
      <div class="mb-2">
        <div class="text-subtitle-1 font-weight-bold">{{ noteType }}</div>
      </div>

      <!-- Description -->
      <p class="text-body-2 mb-4">{{ description }}</p>

      <!-- Location -->
      <div class="d-flex align-center mb-2">
        <v-icon class="me-2" color="primary" size="small">mdi-map-marker</v-icon>
        <div class="d-flex flex-column flex-wrap">
          <span class="text-body-2 text-truncate" style="max-width: 250px">{{
            address
          }}</span>
          <div
            v-if="latitude && longitude"
            class="text-caption text-medium-emphasis"
          >
            {{ latitude }}, {{ longitude }}
          </div>
        </div>
      </div>

      <!-- Reporter -->
      <div class="d-flex align-center mb-2">
        <v-icon class="me-2" color="primary" size="small">mdi-account</v-icon>
        <div>
          <div class="d-flex align-center">
            <span class="text-body-2">
              {{ reportBy?.firstname }} {{ reportBy?.lastname }}</span>
          </div>
          <div class="text-caption text-medium-emphasis">
            <!-- {{ report_by.phone || report_by.email }} -->
          </div>
        </div>
      </div>

      <!-- Time -->
      <!-- <div class="d-flex align-center mt-2">
        <v-icon size="small" color="primary" class="me-2"
          >mdi-clock-outline</v-icon
        >
        <span class="text-body-2">{{ report_time }}</span>
      </div> -->
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
  import { useRouter } from 'vue-router'

  const router = useRouter()

  const props = withDefaults(defineProps<{
    noteId: number | string
    noteType: string
    status: string
    description: string
    reportDate: string
    reportTime: string
    address: string
    reportBy: Record<string, any> | null
    createdAt: string
    latitude?: number | null
    longitude?: number | null
    id?: number | null
  }>(), {
    reportBy: null,
    latitude: null,
    longitude: null,
    id: null,
  })

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'APPROVED':
        return 'success'
      case 'REJECTED':
        return 'error'
      case 'PENDING':
        return 'warning'
      default:
        return 'default'
    }
  }

  const getStatusText = (status: string) => {
    switch (status) {
      case 'APPROVED':
        return 'مقبولة'
      case 'REJECTED':
        return 'مرفوضة'
      case 'PENDING':
        return 'قيد المراجعة'
      default:
        return status
    }
  }

  const formatDate = (date: string) => {
    if (!date) return ''
    return new Date(date).toLocaleDateString('ar-SA', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    })
  }

  const getReporterType = (type: string) => {
    const types: Record<string, string> = {
      ADMIN: 'مدير',
      USER: 'مستخدم',
      SUPERVISOR: 'مشرف',
      INSPECTOR: 'مفتش',
      RESIDENT: 'مقيم',
      VISITOR: 'زائر',
      OTHER: 'آخر',
    }
    return types[type] || type
  }

  // Function to navigate to details page
  const navigateToDetails = () => {
    router.push(`/notes/details/${props.noteId}`)
  }
</script>

<style scoped>
.v-card {
  transition: transform 0.2s ease-in-out;
  cursor: pointer;
}

.v-card:hover {
  transform: translateY(-2px);
}
</style>
