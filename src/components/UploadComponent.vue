<template>
  <v-card-text class="py-2" outlined>
    <!-- Title -->
    <v-card-title class="text-h6 justify-center">
      {{ title }}
    </v-card-title>
    <!-- Uploaded File Info -->
    <v-alert
      v-if="uploadedFile && !fileUploadProgress"
      type=""
      class="mb-6 border rounded-lg"
    >
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2 align-center">
          <div>
            <Csvfileicon />
          </div>
          <div>
            <span class="text-lg">{{ uploadedFile.name }}</span>
            <p class="text-error mb-0 text-sm" v-if="uploadError?.message">
              {{ uploadError.message }}
            </p>
            <p class="mb-0 text-secondary text-sm" v-else>
              {{ formatFileSize(uploadedFile.size) }}
            </p>
          </div>
        </div>
        <div class="d-flex gap-2">
          <v-btn
            :icon="Repeaticon"
            small
            text
            color="#fff"
            size="small"
            v-if="uploadError?.message"
          >
          </v-btn>
          <v-btn
            small
            text
            @click="removeFile"
            color="#fff"
            size="small"
            :icon="Trash"
          >
          </v-btn>
        </div>
      </div>
    </v-alert>
    <!-- Drag & Drop Zone -->
    <v-card
      v-if="!fileUploadProgress && !uploadedFile"
      @click="$refs.fileInput.click()"
      @dragover.prevent="dragOver = true"
      @dragleave="dragOver = false"
      @drop.prevent="handleDrop"
      :class="['drop-zone', { dragover: dragOver }]"
      outlined
      class="text-center bg-primary_2 py-6 mb-6"
    >
      <Uploadfile />
      <p class="mt-4 mb-2">اسحب وأفلِت أو اختر الملف الذي تريد تحميله</p>
      <p class="text-secondary text-xs">
        يدعم الامتدادات .xlsx, .csv والحد الأقصى للحجم 5 ميجا بايت.
      </p>
      <v-btn color="#fff" class="mt-2 rounded-xl">استعراض الملفات</v-btn>
      <input
        type="file"
        ref="fileInput"
        @change="handleFileSelect"
        accept=".csv"
        hidden
      />
    </v-card>

    <v-alert type="" class="mb-6 border rounded-lg" v-if="fileUploadProgress">
      <div class="d-flex flex-column">
        <div class="d-flex gap-2 align-center justify-space-between mb-3">
          <span class="text-sm"> جارى التحميل ...</span>
          <span>{{ uploadProgress }}%</span>
        </div>
        <div style="flex: 1">
          <div class="bg-white rounded-pill" style="height: 10px; width: 100%">
            <div
              class="bg-primary rounded-pill h-100"
              :style="{ width: `${uploadProgress}%` }"
            ></div>
          </div>
          <div style="flex: 1" class="mt-1 text-secondary">
            <p>
              {{ progressSize }} من
              {{ formatFileSize(uploadedFile.size) }}
            </p>
          </div>
        </div>
      </div>
    </v-alert>

    <v-card-text class="rounded-xl py-2" style="background-color: #f8f5ef">
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2">
          <Inforound />
          <p class="text-sm mb-0">
            يرجى التأكد من تنسيق الأعمدة حسب النموذج المطلوب
          </p>
        </div>
        <v-btn
          color="#fff"
          size="small"
          class="text-black bg-white px-5 text-sm"
          variant="text"
          rounded="xl"
          >{{ btnText }}
        </v-btn>
      </div>
    </v-card-text>
  </v-card-text>
</template>

<script setup>
import Repeaticon from "@/components/icons/repeaticon.vue";
import Trash from "@/components/icons/trash.vue";
import Success from "@/components/icons/success.vue";
import Uploadfile from "@/components/icons/uploadfile.vue";
import Csvfileicon from "./icons/csvfileicon.vue";
import Inforound from "./icons/inforound.vue";

const emit = defineEmits(["file-selected"]);
const props = defineProps(["title", "uploadError", "btnText"]);

const dragOver = ref(false);
const uploadedFile = ref(null);
const previewData = ref([]);
const fileUploadProgress = ref(false);
const uploadProgress = ref(0);
const progressSize = ref(0);
const previewHeaders = ref([]);

function formatFileSize(bytes) {
  if (bytes === 0) return "0 Bytes";

  const k = 1024;
  const sizes = ["بايت", "كيلوبايت", "ميجابايت", "جيجابايت", "تيرابايت"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return sizes[i] + " " + parseFloat((bytes / Math.pow(k, i)).toFixed(2));
}
const handleprogress = () => {
  fileUploadProgress.value = true;
  let width = 0;
  const pergInterval = setInterval(frame, 10);
  function frame() {
    if (width == 100) {
      clearInterval(pergInterval);
    } else {
      width++;
      uploadProgress.value = width;
      progressSize.value = formatFileSize(
        (uploadedFile.value.size * width) / 100
      );
    }
  }
};

async function handleFileSelect(event) {
  uploadedFile.value = event.target.files[0];
  handleprogress();
  await new Promise((resolve) => {
    setTimeout(() => {
      resolve(true);
      fileUploadProgress.value = false;
    }, 1010);
  });
  //
  //
  dragOver.value = false;
  if (uploadedFile.value) {
    emit("file-selected", uploadedFile.value);
  }
}
function handleDrop(event) {
  const files = event.dataTransfer.files;
  if (files.length && files[0].name.endsWith(".csv")) {
    uploadedFile.value = files[0];
  }
  dragOver.value = false;
}
function removeFile() {
  uploadedFile.value = null;
  previewData.value = [];
  fileInput.value.value = "";
}
</script>

<style scoped>
.drop-zone {
  border: 2px dashed #ddd;
  cursor: pointer;
  transition: background 0.3s;
}
.dragover {
  background: rgba(25, 118, 210, 0.1);
}
</style>
