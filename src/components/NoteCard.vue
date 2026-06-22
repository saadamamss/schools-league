<template>
  <v-card class="note-card rounded-lg pa-4" elevation="0">
    <div class="d-flex justify-space-between align-center mb-4">
      <div class="d-flex align-center gap-2">
        <v-icon color="#667178">mdi-map-marker</v-icon>
        <span class="text-body-2">{{ note.address }}</span>
      </div>
      <div class="status-chip" :class="'status-' + getStatusClass(note.status)">
        {{ getStatusText(note.status) }}
      </div>
    </div>

    <div class="mb-4">
      <div class="d-flex align-center gap-2 mb-2">
        <v-chip
          class="font-weight-medium"
          color="#667178"
          size="small"
          text-color="white"
        >
          {{ note.report_type.title }}
        </v-chip>
        <span class="text-caption text-grey">{{
          formatDate(note.created_at)
        }}</span>
      </div>
      <p class="text-body-1 mb-0">{{ note.description }}</p>
    </div>

    <div class="d-flex justify-space-between align-center">
      <div class="d-flex align-center gap-2">
        <v-avatar size="32">
          <v-img
            :alt="getFullName(note.reportBy)"
            :src="getAvatarUrl(note.reportBy)"
          />
        </v-avatar>
        <span class="text-body-2">{{ getFullName(note.reportBy) }}</span>
      </div>
      <div class="d-flex align-center gap-2">
        <v-btn class="eye-btn" icon size="small" variant="text">
          <v-icon>mdi-eye-outline</v-icon>
        </v-btn>
        <button class="escalate-btn py-2 rounded-pill">
          <v-icon>mdi-arrow-up-right</v-icon>
          <span class="ms-2">تصعيد</span>
        </button>
      </div>
    </div>
  </v-card>
</template>

<script setup lang="ts">

  const props = defineProps<{
    note: Record<string, any>
  }>()

  const getStatusClass = (status: string) => {
    switch (status) {
      case 'CLOSED':
        return 'مغلق'
      case 'PENDING':
        return 'مفتوح'
      case 'REJECTED':
        return 'مرفوض'
      default:
        return ''
    }
  }

  const getStatusText = (status: string) => {
    return getStatusClass(status)
  }

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('ar-SA', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    })
  }

  const getFullName = (user: Record<string, any>) => {
    return `${user.firstname} ${user.lastname}`
  }

  const getAvatarUrl = (user: Record<string, any>) => {
    return user.avatar || 'https://placehold.co/400'
  }
</script>

<style lang="scss" scoped>
.note-card {
  border: 1px solid rgba(var(--v-theme-on-surface), 0.08);
  transition: all 0.3s ease;

  &:hover {
    border-color: #667178;
    transform: translateY(-2px);
  }
}

.status-chip {
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 0.875rem;
  display: inline-block;
  text-align: center;
  min-width: 80px;
}

.status-مغلق {
  background-color: #e3f0db;
  color: #7ab55c;
}

.status-مفتوح {
  background-color: #fff8df;
  color: #d4b327;
}

.status-مرفوض {
  background-color: #ffebeb;
  color: #ff5252;
}

.eye-btn {
  color: #667178;
  border: 1px solid #fbf7f3;
  border-radius: 8px;
  &:hover {
    background-color: darken(#fbf7f3, 5%);
  }
}

.escalate-btn {
  color: #667178;
  background-color: #fbf7f3;
  border-radius: 8px;
  padding: 0 16px;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  &:hover {
    background-color: darken(#fbf7f3, 5%);
  }
}
</style>
