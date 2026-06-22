<template>
  <v-card class="note-item rounded-lg pa-4 bg-white" elevation="0">
    <div class="d-flex justify-space-between align-center mb-2 border-b pb-2">
      <div class="d-flex align-center">
        <svg
          class="me-2 text-medium-emphasis"
          fill="none"
          height="24"
          viewBox="0 0 24 24"
          width="24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <rect fill="currentColor" height="24" rx="12" width="24" />
          <path
            d="M13.5698 6.84606C13.7527 6.66317 13.8441 6.57173 13.8137 6.47257C13.7834 6.37341 13.6623 6.34999 13.4201 6.30317C13.2099 6.26254 12.9952 6.23456 12.7768 6.22004C11.9372 6.16423 11.0613 6.16434 10.2234 6.22004C7.59757 6.39459 5.51418 8.5142 5.34288 11.1648C5.31084 11.6607 5.31084 12.1737 5.34288 12.6695C5.40666 13.6565 5.8372 14.5468 6.31579 15.2692C6.45915 15.5461 6.38446 15.9613 6.11632 16.4695L6.10397 16.4928C6.01387 16.6635 5.92568 16.8305 5.87481 16.9729C5.8209 17.1238 5.76518 17.3634 5.90414 17.6003C6.03183 17.818 6.23583 17.8978 6.40072 17.9297C6.53762 17.9562 6.70549 17.9602 6.86298 17.9639L6.88927 17.9646C7.70562 17.9844 8.28088 17.7487 8.73653 17.4127C8.75607 17.3983 8.77429 17.3849 8.79131 17.3724C8.86004 17.3218 8.8944 17.2966 8.93907 17.291C8.98375 17.2855 9.02301 17.3015 9.10155 17.3335C9.14045 17.3493 9.18419 17.3673 9.23367 17.3877C9.54497 17.5159 9.89856 17.5927 10.2234 17.6143C11.0613 17.67 11.9372 17.6701 12.7768 17.6143C15.4026 17.4398 17.486 15.3202 17.6573 12.6695C17.6894 12.1737 17.6894 11.6607 17.6573 11.1648C17.6195 10.5796 17.4884 10.0202 17.2789 9.5019C17.2004 9.30767 17.1611 9.21056 17.0703 9.19209C16.9795 9.17363 16.902 9.25114 16.747 9.40616L15.4957 10.6575C15.1505 11.0026 14.7427 11.1538 14.4165 11.2371C14.2538 11.2786 14.0937 11.3074 13.9606 11.3304L13.8876 11.3429C13.7795 11.3614 13.6919 11.3764 13.6053 11.3949L13.5697 11.4026C13.4046 11.438 13.1984 11.4823 13.0194 11.4937C12.8118 11.5069 12.4242 11.4929 12.1079 11.1766C11.7916 10.8602 11.7776 10.4727 11.7908 10.2651C11.8022 10.0861 11.8464 9.8799 11.8819 9.71477L11.8895 9.67919C11.908 9.59262 11.9231 9.50498 11.9416 9.39686L11.9541 9.32384C11.9771 9.19074 12.0059 9.0307 12.0474 8.868C12.1307 8.54179 12.2819 8.13395 12.627 7.78883L13.5698 6.84606Z"
            fill="white"
            opacity="0.4"
          />
        </svg>
        <span class="text-h6 font-weight-medium text-primary">{{
          note.id
        }}</span>
      </div>
      <!-- <div class="status-chip" :class="'status-' + note.status.toLowerCase()">
        {{ getStatusText(note.status) }}
      </div> -->
    </div>

    <div class="note-content mb-0">
      <p class="text-body-1 text-medium-emphasis description-text">
        {{ note.description }}
      </p>
    </div>

    <div class="d-flex align-center gap-2 mb-3">
      <div class="d-flex align-center flex-wrap text-medium-emphasis">
        <span class="text-body-2">{{
          note.report_by?.fullname || note.sector
        }}</span>
        <span class="text-body-2 mx-2">-</span>
        <span class="text-body-2 location-text">{{ note.location }}</span>
      </div>
    </div>

    <div class="d-flex align-center justify-space-between text-grey">
      <div class="text-disabled" style="font-size: 12px">
        {{ note.time }} | {{ formatDate(note.date) }}
      </div>
      <!-- <div
        v-if="note.attachments && note.attachments.length"
        class="d-flex align-center gap-2"
      >
        <v-icon size="small" color="primary">mdi-paperclip</v-icon>
        <span class="text-body-2">{{ note.attachments.length }}</span>
      </div> -->
    </div>
  </v-card>
</template>

<script setup lang="ts">

  const props = defineProps<{
    note: Record<string, any>
  }>()

  const formatDate = (date: string) => {
    if (!date) return ''
    const [year, month, day] = date.split('-')
    return `${day}/${month}/${year}`
  }

  const getStatusText = (status: string) => {
    const statusMap: Record<string, string> = {
      APPROVED: 'معتمد',
      PENDING: 'قيد الانتظار',
      REJECTED: 'مرفوض',
    }
    return statusMap[status] || status
  }
</script>

<style lang="scss" scoped>
.note-item {
  border: 1px solid rgba(var(--v-theme-on-surface), 0.08);
  transition: all 0.3s ease;
  min-height: 200px;
  display: flex;
  flex-direction: column;

  &:hover {
    border-color: rgba(var(--v-theme-on-surface), 0.23);
    transform: translateY(-2px);
  }
}

.note-content {
  flex-grow: 1;
}

.description-text {
  color: rgb(var(--v-theme-on-surface));
  white-space: normal;
  overflow-wrap: break-word;
  word-wrap: break-word;
  line-height: 1.5;
}

.location-text {
  white-space: normal;
  overflow-wrap: break-word;
  word-wrap: break-word;
}

.text-primary {
  color: rgba(var(--v-theme-on-surface), 0.6) !important;
}

.status-chip {
  padding: 4px 16px;
  border-radius: 20px;
  font-size: 0.875rem;
  display: inline-block;
  text-align: center;
  min-width: 80px;
}

.status-approved {
  background-color: rgba(var(--v-theme-success), 0.12);
  color: rgb(var(--v-theme-success));
}

.status-pending {
  background-color: rgba(var(--v-theme-warning), 0.12);
  color: rgb(var(--v-theme-warning));
}

.status-rejected {
  background-color: rgba(var(--v-theme-error), 0.12);
  color: rgb(var(--v-theme-error));
}
</style>
