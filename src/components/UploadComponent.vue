<template>
  <v-card-text class="py-2" outlined>
    <!-- Title -->
    <v-card-title class="text-h6 justify-center">
      {{ title }}
    </v-card-title>
    <!-- Uploaded File Info -->
    <v-alert
      v-if="uploadedFile && !fileUploadProgress"
      class="mb-6 border rounded-lg"
    >
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2 align-center">
          <div>
            <Csvfileicon />
          </div>
          <div>
            <span class="text-lg">{{ uploadedFile.name }}</span>
            <p v-if="uploadError?.message" class="text-error mb-0 text-sm">
              {{ uploadError.message }}
            </p>
            <p v-else class="mb-0 text-secondary text-sm">
              {{ formatFileSize(uploadedFile?.size ?? 0) }}
            </p>
          </div>
        </div>
        <div class="d-flex gap-2">
          <v-btn
            v-if="uploadError?.message"
            color="#fff"
            :icon="Repeaticon"
            size="small"
            small
            text
          />
          <v-btn
            color="#fff"
            :icon="Trash"
            size="small"
            small
            text
            @click="removeFile"
          />
        </div>
      </div>
    </v-alert>
    <!-- Drag & Drop Zone -->
    <v-card
      v-if="!fileUploadProgress && !uploadedFile"
      class="text-center bg-primary_2 py-6 mb-6"
      :class="['drop-zone', { dragover: dragOver }]"
      outlined
      @click="fileInput?.click()"
      @dragleave="dragOver = false"
      @dragover.prevent="dragOver = true"
      @drop.prevent="handleDrop"
    >
      <Uploadfile />
      <p class="mt-4 mb-2">اسحب وأفلِت أو اختر الملف الذي تريد تحميله</p>
      <p class="text-secondary text-xs">
        يدعم الامتدادات .xlsx, .csv والحد الأقصى للحجم 5 ميجا بايت.
      </p>
      <v-btn class="mt-2 rounded-xl" color="#fff">استعراض الملفات</v-btn>
      <input
        ref="fileInput"
        accept=".csv"
        hidden
        type="file"
        @change="handleFileSelect"
      >
    </v-card>

    <v-alert v-if="fileUploadProgress" class="mb-6 border rounded-lg">
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
            />
          </div>
          <div class="mt-1 text-secondary" style="flex: 1">
            <p>
              {{ progressSize }} من
              {{ formatFileSize(uploadedFile?.size ?? 0) }}
            </p>
          </div>
        </div>
      </div>
    </v-alert>

    <v-card-text class="rounded-xl py-2" style="background-color: rgb(var(--v-theme-background))">
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2">
          <Inforound />
          <p class="text-sm mb-0">
            يرجى التأكد من تنسيق الأعمدة حسب النموذج المطلوب
          </p>
        </div>
        <v-btn
          class="text-black bg-white px-5 text-sm"
          color="#fff"
          rounded="xl"
          size="small"
          variant="text"
        >{{ btnText }}
        </v-btn>
      </div>
    </v-card-text>
  </v-card-text>
</template>

<script setup lang="ts">
  import Repeaticon from '@/components/icons/repeaticon.vue'
  import Trash from '@/components/icons/trash.vue'
  import Success from '@/components/icons/success.vue'
  import Uploadfile from '@/components/icons/uploadfile.vue'
  import Csvfileicon from './icons/csvfileicon.vue'
  import Inforound from './icons/inforound.vue'

  const emit = defineEmits<{ fileSelected: [file: File] }>()
  const props = defineProps<{
    title: string
    uploadError: { message: string } | null
    btnText: string
  }>()

  const dragOver = ref(false)
  const uploadedFile = ref<File | null>(null)
  const previewData = ref<any[]>([])
  const fileUploadProgress = ref(false)
  const uploadProgress = ref(0)
  const progressSize = ref('')
  const previewHeaders = ref<any[]>([])
  const fileInput = ref<HTMLInputElement | null>(null)

  function formatFileSize (bytes: number) {
    if (bytes === 0) return '0 Bytes'

    const k = 1024
    const sizes = ['بايت', 'كيلوبايت', 'ميجابايت', 'جيجابايت', 'تيرابايت']
    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return sizes[i] + ' ' + parseFloat((bytes / Math.pow(k, i)).toFixed(2))
  }
  const handleprogress = () => {
    fileUploadProgress.value = true
    let width = 0
    const pergInterval = setInterval(frame, 10)
    function frame () {
      if (width === 100) {
        clearInterval(pergInterval)
      } else {
        width++
        uploadProgress.value = width
        progressSize.value = formatFileSize(
          ((uploadedFile.value?.size ?? 0) * width) / 100
        )
      }
    }
  }

  async function handleFileSelect (event: Event) {
    const input = event.target as HTMLInputElement
    uploadedFile.value = input.files?.[0] ?? null
    handleprogress()
    await new Promise(resolve => {
      setTimeout(() => {
        resolve(true)
        fileUploadProgress.value = false
      }, 1010)
    })
    //
    //
    dragOver.value = false
    if (uploadedFile.value) {
      emit('fileSelected', uploadedFile.value)
    }
  }
  function handleDrop (event: DragEvent) {
    const files = event.dataTransfer?.files
    if (files?.length && files[0].name.endsWith('.csv')) {
      uploadedFile.value = files[0]
    }
    dragOver.value = false
  }
  function removeFile () {
    uploadedFile.value = null
    previewData.value = []
  }
</script>

<style scoped>
.drop-zone {
  border: 2px dashed rgba(var(--v-theme-on-surface), 0.23);
  cursor: pointer;
  transition: background 0.3s;
}
.dragover {
  background: rgba(25, 118, 210, 0.1);
}
</style>
