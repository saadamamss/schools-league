<template>
  <v-container class="profile-container" fluid>
    <v-skeleton-loader v-if="!user" type="card-avatar" />
    <v-card v-else>
      <v-card-text>
        <v-row>
          <!-- Profile Column -->
          <v-col class="text-center" cols="12" md="4">
            <v-avatar class="mb-4" color="primary" size="200">
              <v-img
                v-if="user.profileImage"
                size="100"
                :src="user.profileImage"
              />

              <span class="text-h2">
                {{ user?.firstName?.[0] || user?.fullName?.[0] }}
              </span>
            </v-avatar>

            <h2 class="text-h5">{{ user.fullName }}</h2>
            <div class="d-flex justify-center mt-2 ga-3">
              <v-chip class="mr-2" color="primary">
                {{ user.userType?.name?.ar }}
              </v-chip>
              <v-chip :color="user.isActive ? 'primary' : 'error'">
                {{ user.isActive ? "نشط" : "غير نشط" }}
              </v-chip>
            </div>

            <v-card class="mt-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">معلومات الاتصال</div>
                <v-divider class="mb-3" />

                <v-list density="compact">
                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-email</v-icon>
                    </template>
                    <v-list-item-title>{{ user.email }}</v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-phone</v-icon>
                    </template>
                    <v-list-item-title>{{ user.phone }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Information Column -->
          <v-col cols="12" md="8">
            <v-card class="mb-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">المعلومات الشخصية</div>
                <v-divider class="mb-3" />

                <v-row>
                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="الاسم الأول"
                      :model-value="user.firstName"
                      readonly
                      variant="outlined"
                    />
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="اسم العائلة"
                      :model-value="user.lastName"
                      readonly
                      variant="outlined"
                    />
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="رقم الهوية"
                      :model-value="user.saId"
                      readonly
                      variant="outlined"
                    />
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="الجنس"
                      :model-value="user.gender === 'male' ? 'ذكر' : 'أنثى'"
                      readonly
                      variant="outlined"
                    />
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="تاريخ الإنشاء"
                      :model-value="formatDate(user.createdAt)"
                      readonly
                      variant="outlined"
                    />
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      density="comfortable"
                      label="آخر تسجيل دخول"
                      :model-value="
                        user.todayCheckIn
                          ? formatDate(user.todayCheckIn)
                          : 'لم يسجل دخول بعد'
                      "
                      readonly
                      variant="outlined"
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Change Password Form -->
            <v-card variant="outlined">
              <v-card-title class="d-flex align-center">
                <v-icon class="mr-2" icon="mdi-lock-reset" />
                <span>تغيير كلمة المرور</span>
              </v-card-title>

              <v-card-text>
                <v-form @submit.prevent="handleChangePassword">
                  <v-row>
                    <v-col cols="12">
                      <v-text-field
                        v-model="passwordForm.currentPassword"
                        :append-inner-icon="
                          showCurrentPassword ? 'mdi-eye' : 'mdi-eye-off'
                        "
                        density="comfortable"
                        :error-messages="errors.currentPassword"
                        label="كلمة المرور الحالية"
                        required
                        :type="showCurrentPassword ? 'text' : 'password'"
                        variant="outlined"
                        @click:append="
                          showCurrentPassword = !showCurrentPassword
                        "
                      />
                    </v-col>

                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="passwordForm.newPassword"
                        :append-inner-icon="
                          showNewPassword ? 'mdi-eye' : 'mdi-eye-off'
                        "
                        density="comfortable"
                        :error-messages="errors.newPassword"
                        label="كلمة المرور الجديدة"
                        required
                        :type="showNewPassword ? 'text' : 'password'"
                        variant="outlined"
                        @click:append="showNewPassword = !showNewPassword"
                      />
                    </v-col>

                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="passwordForm.confirmPassword"
                        :append-inner-icon="
                          showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'
                        "
                        density="comfortable"
                        :error-messages="errors.confirmPassword"
                        label="تأكيد كلمة المرور"
                        required
                        :type="showConfirmPassword ? 'text' : 'password'"
                        variant="outlined"
                        @click:append="
                          showConfirmPassword = !showConfirmPassword
                        "
                      />
                    </v-col>

                    <v-col cols="12">
                      <v-alert
                        v-if="passwordMessage.text"
                        class="mb-4"
                        :type="passwordMessage.type"
                        variant="tonal"
                      >
                        {{ passwordMessage.text }}
                      </v-alert>

                      <div class="d-flex justify-end">
                        <v-btn
                          color="primary"
                          :disabled="isChangePassLoading"
                          :loading="isChangePassLoading"
                          type="submit"
                        >
                          حفظ التغييرات
                        </v-btn>
                      </div>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
  import { computed, onMounted, ref } from 'vue'
  import { useAuthStore } from '@/stores/auth'

  import { authApi } from '@/api'
  import { useAppStore } from '@/stores/app'
  import { formatDate } from '@/@core/utils/formatters'
  const appStore = useAppStore()
  const authStore = useAuthStore()
  const user = ref<Record<string, any> | null>(null)
  const fetchuserProfile = async () => {
    try {
      const response = await authApi.getProfile()
      if (response.data) {
        user.value = response.data.data
      }
    } catch (error: any) {
      appStore.showSnackbar({
        message:
          error?.response?.data?.status?.message || 'حدث خطأ أثناء جلب البيانات',
        color: 'error',
      })
    }
  }

  const passwordForm = ref({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  })

  const showCurrentPassword = ref(false)
  const showNewPassword = ref(false)
  const showConfirmPassword = ref(false)

  const errors = ref({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  })

  const passwordMessage = ref<{ text: string; type: 'warning' | 'success' | 'info' | 'error' }>({
    text: '',
    type: 'success',
  })

  const formatDateTime = (dateTimeString: string) => {
    if (!dateTimeString) return ''
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }
    return new Date(dateTimeString).toLocaleDateString('ar-SA', options)
  }

  const validateForm = () => {
    let isValid = true
    errors.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    }

    if (!passwordForm.value.currentPassword) {
      errors.value.currentPassword = 'يجب إدخال كلمة المرور الحالية'
      isValid = false
    }

    if (!passwordForm.value.newPassword) {
      errors.value.newPassword = 'يجب إدخال كلمة المرور الجديدة'
      isValid = false
    } else if (passwordForm.value.newPassword.length < 8) {
      errors.value.newPassword = 'يجب أن تتكون كلمة المرور من 8 أحرف على الأقل'
      isValid = false
    }

    if (!passwordForm.value.confirmPassword) {
      errors.value.confirmPassword = 'يجب تأكيد كلمة المرور'
      isValid = false
    } else if (
      passwordForm.value.newPassword !== passwordForm.value.confirmPassword
    ) {
      errors.value.confirmPassword = 'كلمة المرور غير متطابقة'
      isValid = false
    }

    return isValid
  }

  const isChangePassLoading = ref(false)
  const handleChangePassword = async () => {
    if (!validateForm()) return
    try {
      isChangePassLoading.value = true
      const response = await authApi.changePassword({
        current_password: passwordForm.value.currentPassword,
        new_password: passwordForm.value.newPassword,
        new_password_confirmation: passwordForm.value.confirmPassword,
      })
      if (response.status === 200) {
        appStore.showSnackbar({
          message: response.data?.status?.message,
          color: 'primary',
        })

        passwordForm.value = {
          currentPassword: '',
          newPassword: '',
          confirmPassword: '',
        }
      }
    } catch (error: any) {
      appStore.showSnackbar({
        message: error.response?.data?.status?.message,
        color: 'error',
      })
    } finally {
      isChangePassLoading.value = false
    }
  }

  onMounted(() => {
    fetchuserProfile()
  })
</script>

<style scoped>
.v-avatar {
  border: 4px solid rgb(var(--v-theme-primary));
}

.text-h5 {
  font-weight: 500;
}

.v-list-item {
  padding-left: 0;
  padding-right: 0;
}
</style>
