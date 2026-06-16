<template>
  <v-card elevation="1" class="rounded-lg">
    <v-card-text class="pa-4">
      <!-- Header with Stats -->
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center">
          <v-chip
            color="primary"
            text-color="white"
            size="small"
            class="font-weight-medium"
          >
            نوع الملاحظة
          </v-chip>
        </div>
        <div class="text-caption text-medium-emphasis">
          {{ formatDate(created_at) }}
        </div>
      </div>

      <!-- Title -->
      <div class="mb-2">
        <div class="d-flex justify-space-between align-center">
          <div class="text-subtitle-1 font-weight-bold">{{ title }}</div>
          <div class="d-flex align-center gap-2">
            <v-btn
              icon
              variant="text"
              color="primary"
              size="small"
              @click.stop="$emit('edit')"
            >
              <v-icon>mdi-pencil</v-icon>
            </v-btn>
            <v-btn
              icon
              variant="text"
              color="error"
              size="small"
              @click.stop="handleDelete"
            >
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </div>
        </div>
      </div>

      <!-- Stats -->
      <div class="d-flex align-center mb-2">
        <v-icon size="small" color="primary" class="me-2"
          >mdi-file-document-outline</v-icon
        >
        <span class="text-body-2">{{ total_reports }} ملاحظة</span>
      </div>

      <!-- Last Update -->
      <div class="d-flex align-center">
        <v-icon size="small" color="primary" class="me-2"
          >mdi-clock-outline</v-icon
        >
        <span class="text-body-2">آخر تحديث: {{ formatDate(updated_at) }}</span>
      </div>
    </v-card-text>
  </v-card>

  <!-- Delete Confirmation Dialog -->
  <v-dialog v-model="deleteDialog" max-width="400px">
    <v-card class="pa-4">
      <v-card-title class="text-h5 mb-3">تأكيد الحذف</v-card-title>
      <v-card-text>
        هل أنت متأكد من حذف نوع الملاحظة "{{ title }}"؟
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          color="grey-darken-1"
          variant="text"
          @click="deleteDialog = false"
        >
          إلغاء
        </v-btn>
        <v-btn color="error" @click="confirmDelete"> حذف </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { defineProps, ref } from "vue";

const deleteDialog = ref(false);

const props = defineProps({
  id: {
    type: [Number, String],
    required: true,
  },
  title: {
    type: String,
    required: true,
  },
  total_reports: {
    type: Number,
    default: 0,
  },
  created_at: {
    type: String,
    required: true,
  },
  updated_at: {
    type: String,
    required: true,
  },
});

const formatDate = (date) => {
  if (!date) return "";
  return new Date(date).toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

// Function to handle delete button click
const handleDelete = () => {
  deleteDialog.value = true;
};

// Function to confirm deletion
const confirmDelete = () => {
  deleteDialog.value = false;
  // Emit delete event to parent
  emit("delete");
};

const emit = defineEmits(["edit", "delete"]);
</script>

<style scoped>
.v-card {
  transition: transform 0.2s ease-in-out;
}

.v-card:hover {
  transform: translateY(-2px);
}
</style>
