<template>
  <div class="upload-file-wrapper">
    <div v-if="showArrow" class="arrow-title mb-2">
      <v-icon color="grey-darken-1" size="20">mdi-arrow-down</v-icon>
    </div>
    <h3 v-else class="text-subtitle-1 font-weight-medium mb-2">
      {{ title }}{{ required ? "*" : "" }}
    </h3>
    <v-card
      class="upload-card"
      :class="{
        'circle-card': isImageOnly || isProfilePic,
        'has-preview': filesValue.length > 0 && (isImageOnly || isProfilePic),
      }"
      variant="outlined"
      @click="triggerFileInput"
    >
      <div class="upload-area" :class="{ 'has-file': filesValue.length > 0 }">
        <v-file-input
          ref="fileInput"
          :accept="accept"
          class="upload-input"
          density="compact"
          hide-details
          :loading="loading"
          :model-value="filesValue"
          :multiple="multiple"
          :rules="required ? fileRules : undefined"
          variant="plain"
          @update:model-value="handleFileChange"
        />

        <!-- Files Grid Preview -->
        <div v-if="filesValue.length > 0" class="files-grid">
          <v-row>
            <v-col
              v-for="(file, index) in filesValue"
              :key="file.name"
              class="file-item"
              cols="12"
              md="6"
              sm="12"
            >
              <!-- Image Preview -->
              <div
                v-if="isImage(file)"
                class="image-preview"
                :class="{ 'circle-card': isProfilePic }"
              >
                <img
                  v-if="previewUrls[file.name]"
                  :alt="file.name"
                  :src="previewUrls[file.name]"
                >
                <div class="image-overlay">
                  <div class="file-info text-center">
                    <div class="text-subtitle-2 text-white">
                      {{ file.name }}
                    </div>
                    <div class="text-caption text-white">
                      {{ formatFileSize(file.size) }}
                    </div>
                  </div>
                  <v-btn
                    color="error"
                    density="compact"
                    icon
                    variant="text"
                    @click.stop="removeFile(index)"
                  >
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                </div>
              </div>

              <!-- Document Preview -->
              <div v-else class="file-preview">
                <div class="d-flex align-center justify-space-between pa-4">
                  <div class="d-flex align-center">
                    <v-icon
                      class="ml-2"
                      color="primary"
                      size="24"
                    >mdi-file-document-outline</v-icon>
                    <div>
                      <div class="text-body-2">{{ file.name }}</div>
                      <div class="text-caption text-grey">
                        {{ formatFileSize(file.size) }}
                      </div>
                    </div>
                  </div>
                  <v-btn
                    color="error"
                    density="compact"
                    icon
                    variant="text"
                    @click.stop="removeFile(index)"
                  >
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                </div>
              </div>
            </v-col>
          </v-row>
        </div>

        <!-- Upload Placeholder -->
        <div v-else class="upload-placeholder">
          <svg
            fill="none"
            height="16"
            viewBox="0 0 18 16"
            width="18"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M9.6252 9.66683C9.6252 10.012 9.34538 10.2918 9.0002 10.2918C8.65503 10.2918 8.3752 10.012 8.3752 9.66683V2.61522L8.37015 2.62118C8.19458 2.82838 8.02267 3.04735 7.86191 3.25212L7.82465 3.29958C7.66446 3.50353 7.49593 3.71779 7.36505 3.85245C7.12446 4.09997 6.72877 4.10559 6.48125 3.865C6.23373 3.62442 6.22811 3.22873 6.46869 2.98121C6.54261 2.90516 6.66397 2.75365 6.84163 2.52746L6.88082 2.47754C7.03921 2.27576 7.22504 2.03901 7.41648 1.81309C7.62165 1.57096 7.8513 1.31946 8.08086 1.12394C8.1959 1.02596 8.32556 0.929323 8.46506 0.854675C8.59959 0.782679 8.78497 0.708496 9.0002 0.708496C9.21544 0.708496 9.40082 0.782679 9.53535 0.854675C9.67484 0.929323 9.80451 1.02596 9.91955 1.12394C10.1491 1.31946 10.3788 1.57096 10.5839 1.81309C10.7754 2.039 10.9611 2.27569 11.1195 2.47747L11.1588 2.52746C11.3364 2.75365 11.4578 2.90516 11.5317 2.98121C11.7723 3.22873 11.7667 3.62442 11.5192 3.86501C11.2716 4.10559 10.8759 4.09997 10.6354 3.85245C10.5045 3.71779 10.3359 3.50353 10.1758 3.29958L10.1385 3.25211C9.97773 3.04735 9.80582 2.82837 9.63025 2.62118L9.6252 2.61522V9.66683Z"
              fill="#75797C"
            />
            <path
              d="M17.0895 9.87483C17.2044 9.54933 17.0337 9.19233 16.7082 9.07745C16.3827 8.96257 16.0257 9.13331 15.9108 9.45881L15.7159 10.0109C15.332 11.0988 15.0604 11.8655 14.7816 12.4403C14.5097 13.0007 14.2568 13.3212 13.9413 13.5444C13.6259 13.7676 13.2395 13.8994 12.6205 13.9692C11.9857 14.0409 11.1723 14.0418 10.0187 14.0418H7.98164C6.82799 14.0418 6.0146 14.0409 5.37984 13.9692C4.76086 13.8994 4.37447 13.7676 4.059 13.5444C3.74353 13.3212 3.49069 13.0007 3.21879 12.4403C2.93995 11.8655 2.66837 11.0988 2.28441 10.0109L2.08954 9.45881C1.97466 9.13331 1.61766 8.96257 1.29216 9.07745C0.966658 9.19234 0.795919 9.54934 0.910801 9.87484L1.11812 10.4623C1.48678 11.5068 1.78096 12.3403 2.09416 12.9859C2.41781 13.653 2.78722 14.1758 3.33704 14.5648C3.88685 14.9538 4.50283 15.1282 5.23962 15.2114C5.95261 15.2918 6.83652 15.2918 7.94422 15.2918H10.0561C11.1638 15.2918 12.0477 15.2918 12.7607 15.2114C13.4975 15.1282 14.1135 14.9538 14.6633 14.5648C15.2131 14.1758 15.5825 13.653 15.9062 12.9859C16.2194 12.3403 16.5136 11.5068 16.8822 10.4622L17.0895 9.87483Z"
              fill="#75797C"
            />
          </svg>

          <div class="text-grey-darken-1 text-body-2 mt-2 text-center">
            {{
              isImageOnly || isProfilePic
                ? " "
                : multiple
                  ? `اسحب وأفلت أو اختر الملفات (${maxFiles} كحد أقصى)`
                  : "اسحب وأفلت أو اختر الملف الذي تريد تحميله"
            }}
          </div>
          <div v-if="!isImageOnly" class="text-caption text-grey-darken-1">
            الحد الأقصى للحجم 5 ميجا بايت
          </div>
        </div>
      </div>
    </v-card>
  </div>
</template>

<script setup lang="ts">
  import { computed, ref, watch } from 'vue'

  const props = defineProps({
    modelValue: {
      type: Array as () => File[],
      default: () => [],
    },
    title: {
      type: String,
      required: true,
    },
    required: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    isImageOnly: {
      type: Boolean,
      default: false,
    },
    isProfilePic: {
      type: Boolean,
      default: false,
    },
    accept: {
      type: String,
      default: 'image/*,.pdf',
    },
    showArrow: {
      type: Boolean,
      default: false,
    },
    multiple: {
      type: Boolean,
      default: false,
    },
    maxFiles: {
      type: Number,
      default: 5,
    },
  })

  const emit = defineEmits(['update:modelValue', 'change'])

  const filesValue = ref<File[]>([])
  const fileInput = ref<any>(null)
  const previewUrls = ref<{ [key: string]: string }>({})

  const fileRules = [
    (v: File[] | null) =>
      props.required ? (v && v.length > 0) || 'هذا الحقل مطلوب' : true,
    (v: File[] | null) => {
      if (!v) return true
      return (
        v.every(file => file.size <= 5 * 1024 * 1024) ||
        'حجم الملف يجب أن لا يتجاوز 5 ميجا بايت'
      )
    },
  ]

  const isImage = (file: File) => file.type.startsWith('image/')

  const updatePreviews = async (files: File[]) => {
    const previews: { [key: string]: string } = {}
    for (const file of files) {
      if (isImage(file)) {
        try {
          const result = await readFileAsDataURL(file)
          previews[file.name] = result
        } catch {
          // silently fail
        }
      }
    }
    previewUrls.value = previews
  }

  const readFileAsDataURL = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.onload = () => resolve(reader.result as string)
      reader.onerror = reject
      reader.readAsDataURL(file)
    })
  }

  watch(
    () => props.modelValue,
    newValue => {
      if (Array.isArray(newValue)) {
        filesValue.value = newValue
        updatePreviews(newValue)
      } else {
        filesValue.value = []
        previewUrls.value = {}
      }
    },
    { immediate: true }
  )

  const handleFileChange = async (event: File | File[]) => {
    const files: File[] = Array.isArray(event) ? event : event ? [event] : []

    if (props.multiple) {
      const totalFiles = [...filesValue.value, ...files]
      if (totalFiles.length > props.maxFiles) {
        // Handle max files limit
        return
      }
      filesValue.value = totalFiles
    } else {
      filesValue.value = files.slice(0, 1)
    }

    await updatePreviews(filesValue.value)
    emit('update:modelValue', filesValue.value)
    emit('change', filesValue.value)
  }

  const removeFile = (index: number) => {
    const newFiles = [...filesValue.value]
    const removedFile = newFiles.splice(index, 1)[0]
    filesValue.value = newFiles
    delete previewUrls.value[removedFile.name]
    emit('update:modelValue', newFiles)
  }

  const triggerFileInput = () => {
    if (
      (!props.multiple && filesValue.value.length === 0) ||
      (props.multiple && filesValue.value.length < props.maxFiles)
    ) {
      fileInput.value?.$el.querySelector('input')?.click()
    }
  }

  const formatFileSize = (bytes: number): string => {
    if (!bytes || bytes === 0) return '0 B'
    const k = 1024
    const sizes = ['B', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(1))} ${sizes[i]}`
  }
</script>

<style scoped lang="scss">
.upload-file-wrapper {
  .arrow-title {
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .upload-card {
    border: 1px dashed rgba(0, 0, 0, 0.12);
    padding: 1rem;
    transition: all 0.3s ease;
    // background-color: #fafafa;
    border-radius: 8px;
    cursor: pointer;

    &.circle-card {
      width: 88px;
      height: 88px;
      border-radius: 50% !important;
      padding: 0;
      overflow: hidden;
      // border: 2px dashed rgba(0, 0, 0, 0.12);
      display: flex;
      align-items: center;
      justify-content: center;

      .upload-area {
        border-radius: 50%;
        width: 88px;
        height: 88px;
      }

      &.has-preview {
        border-style: solid;
        border-color: rgb(var(--v-theme-primary));
      }
    }

    &:hover {
      border-color: rgb(var(--v-theme-primary));
      background-color: rgba(var(--v-theme-primary), 0.04);
    }

    .upload-area {
      position: relative;
      min-height: 150px !important;
      display: flex;
      align-items: center;
      justify-content: center;

      &.has-file {
        min-height: auto !important;
        padding: 0;
      }
    }

    .files-grid {
      width: 100%;
      padding: 0.5rem;

      .file-item {
        position: relative;
      }
    }

    .image-preview {
      width: 100%;
      aspect-ratio: 1;
      position: relative;
      overflow: hidden;
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: rgb(var(--v-theme-background));
      border-radius: 8px;

      &.circle-card {
        border-radius: 50%;
        width: 88px;
        height: 88px;
      }

      img {
        width: 100%;
        height: 100%;
        object-fit: contain;
        display: block;
        padding: 4px;
      }

      .image-overlay {
        position: absolute;
        inset: 0;
        background: rgba(0, 0, 0, 0.5);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: flex-end;
        opacity: 0;
        transition: opacity 0.3s ease;

        .file-info {
          padding: 8px 1rem;
          width: 100%;
          font-size: 12px;
          text-align: center;
          background: linear-gradient(to top, rgba(0, 0, 0, 0.7), transparent);

          .text-subtitle-2 {
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 100%;
            margin-bottom: 4px;
            font-size: 0.875rem;
          }
        }

        .v-btn {
          position: absolute;
          top: 8px;
          left: 50%;
          transform: translateX(-50%);
          background: rgba(0, 0, 0, 0.3);
          z-index: 1;
          width: 24px;
          height: 24px;
          min-width: 24px;
          padding: 0;

          :deep(.v-icon) {
            font-size: 16px;
          }

          &:hover {
            background: rgba(0, 0, 0, 0.5);
          }
        }
      }

      &:hover .image-overlay {
        opacity: 1;
      }
    }

    .file-preview {
      width: 100%;
      background-color: rgb(var(--v-theme-surface));
      border-radius: 8px;
      border: 1px solid rgba(0, 0, 0, 0.12);

      .v-btn {
        opacity: 0;
        transition: opacity 0.3s ease;
      }

      &:hover {
        .v-btn {
          opacity: 1;
        }
      }
    }

    :deep(.v-file-input) {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      opacity: 0;
      pointer-events: none;

      .v-field {
        display: none;
      }

      .v-field__field {
        display: none;
      }
    }

    .upload-placeholder {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 2rem;
      pointer-events: none;

      .text-body-2 {
        max-width: 80%;
        margin: 0.5rem auto;
      }
    }
  }
}
</style>
