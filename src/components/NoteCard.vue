<template>
  <v-card elevation="0" class="note-card rounded-lg pa-4">
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
          color="#667178"
          text-color="white"
          size="small"
          class="font-weight-medium"
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
            :src="getAvatarUrl(note.reportBy)"
            :alt="getFullName(note.reportBy)"
          ></v-img>
        </v-avatar>
        <span class="text-body-2">{{ getFullName(note.reportBy) }}</span>
      </div>
      <div class="d-flex align-center gap-2">
        <v-btn icon variant="text" size="small" class="eye-btn">
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

<script setup>
import { defineProps } from "vue";

const props = defineProps({
  note: {
    type: Object,
    required: true,
  },
});

const getStatusClass = (status) => {
  switch (status) {
    case "CLOSED":
      return "مغلق";
    case "PENDING":
      return "مفتوح";
    case "REJECTED":
      return "مرفوض";
    default:
      return "";
  }
};

const getStatusText = (status) => {
  return getStatusClass(status);
};

const formatDate = (date) => {
  return new Date(date).toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

const getFullName = (user) => {
  return `${user.firstname} ${user.lastname}`;
};

const getAvatarUrl = (user) => {
  return user.avatar || "https://placehold.co/400";
};
</script>

<style lang="scss" scoped>
.note-card {
  border: 1px solid #eee;
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
