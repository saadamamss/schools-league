<template>
  <v-container fluid class="profile-container">
    <v-skeleton-loader type="card-avatar" v-if="!user"></v-skeleton-loader>
    <v-card v-else>
      <v-card-text>
        <v-row>
          <!-- Profile Column -->
          <v-col cols="12" md="4" class="text-center">
            <v-avatar size="200" color="primary" class="mb-4">
              <v-img
                v-if="user.profile_image"
                :src="user.profile_image"
                size="100"
              />

              <span class="text-h2">
                {{ user?.first_name?.[0] }}
              </span>
            </v-avatar>

            <h2 class="text-h5">{{ user.full_name }}</h2>
            <div class="d-flex justify-center mt-2 ga-3">
              <v-chip color="primary" class="mr-2">
                {{ user.user_type.name.ar }}
              </v-chip>
              <v-chip :color="user.is_active ? 'primary' : 'error'">
                {{ user.is_active ? "نشط" : "غير نشط" }}
              </v-chip>
            </div>

            <v-card class="mt-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">معلومات الاتصال</div>
                <v-divider class="mb-3"></v-divider>

                <v-list density="compact">
                  <v-list-item>
                    <template v-slot:prepend>
                      <v-icon>mdi-email</v-icon>
                    </template>
                    <v-list-item-title>{{ user.email }}</v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template v-slot:prepend>
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
            <v-card variant="outlined" class="mb-6">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">المعلومات الشخصية</div>
                <v-divider class="mb-3"></v-divider>

                <v-row>
                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="user.first_name"
                      label="الاسم الأول"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="user.last_name"
                      label="اسم العائلة"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="user.sa_id"
                      label="رقم الهوية"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="user.gender === 'male' ? 'ذكر' : 'أنثى'"
                      label="الجنس"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="formatDate(user.created_at)"
                      label="تاريخ الإنشاء"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-text-field
                      :model-value="
                        user.today_check_in
                          ? formatDate(user.today_check_in)
                          : 'لم يسجل دخول بعد'
                      "
                      label="آخر تسجيل دخول"
                      readonly
                      variant="outlined"
                      density="comfortable"
                    ></v-text-field>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Change Password Form -->
            <v-card variant="outlined">
              <v-card-title class="d-flex align-center">
                <v-icon icon="mdi-lock-reset" class="mr-2"></v-icon>
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
                        :type="showCurrentPassword ? 'text' : 'password'"
                        label="كلمة المرور الحالية"
                        variant="outlined"
                        density="comfortable"
                        required
                        @click:append="
                          showCurrentPassword = !showCurrentPassword
                        "
                        :error-messages="errors.currentPassword"
                      ></v-text-field>
                    </v-col>

                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="passwordForm.newPassword"
                        :append-inner-icon="
                          showNewPassword ? 'mdi-eye' : 'mdi-eye-off'
                        "
                        :type="showNewPassword ? 'text' : 'password'"
                        label="كلمة المرور الجديدة"
                        variant="outlined"
                        density="comfortable"
                        required
                        @click:append="showNewPassword = !showNewPassword"
                        :error-messages="errors.newPassword"
                      ></v-text-field>
                    </v-col>

                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model="passwordForm.confirmPassword"
                        :append-inner-icon="
                          showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'
                        "
                        :type="showConfirmPassword ? 'text' : 'password'"
                        label="تأكيد كلمة المرور"
                        variant="outlined"
                        density="comfortable"
                        required
                        @click:append="
                          showConfirmPassword = !showConfirmPassword
                        "
                        :error-messages="errors.confirmPassword"
                      ></v-text-field>
                    </v-col>

                    <v-col cols="12">
                      <v-alert
                        v-if="passwordMessage.text"
                        :type="passwordMessage.type"
                        variant="tonal"
                        class="mb-4"
                      >
                        {{ passwordMessage.text }}
                      </v-alert>

                      <div class="d-flex justify-end">
                        <v-btn
                          type="submit"
                          color="primary"
                          :loading="isChangePassLoading"
                          :disabled="isChangePassLoading"
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

<script setup>
import { onMounted, ref } from "vue";
import { useAuthStore } from "@/stores/Auth";
import { computed } from "vue";
import axiosIns from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import { formatDate } from "@/@core/utils/formatters";
const appStore = useAppStore();
const authStore = useAuthStore();
const user = ref(null);
const fetchuserProfile = async () => {
  try {
    const response = await axiosIns.get("auth/profile");
    if (response.data) {
      user.value = response.data.data;
    }
  } catch (error) {
    appStore.showSnackbar({
      message:
        error?.response?.data?.status?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
  }
};

const passwordForm = ref({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const showCurrentPassword = ref(false);
const showNewPassword = ref(false);
const showConfirmPassword = ref(false);

const errors = ref({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const passwordMessage = ref({
  text: "",
  type: "",
});

const formatDateTime = (dateTimeString) => {
  if (!dateTimeString) return "";
  const options = {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  };
  return new Date(dateTimeString).toLocaleDateString("ar-SA", options);
};

const validateForm = () => {
  let isValid = true;
  errors.value = {
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  };

  if (!passwordForm.value.currentPassword) {
    errors.value.currentPassword = "يجب إدخال كلمة المرور الحالية";
    isValid = false;
  }

  if (!passwordForm.value.newPassword) {
    errors.value.newPassword = "يجب إدخال كلمة المرور الجديدة";
    isValid = false;
  } else if (passwordForm.value.newPassword.length < 8) {
    errors.value.newPassword = "يجب أن تتكون كلمة المرور من 8 أحرف على الأقل";
    isValid = false;
  }

  if (!passwordForm.value.confirmPassword) {
    errors.value.confirmPassword = "يجب تأكيد كلمة المرور";
    isValid = false;
  } else if (
    passwordForm.value.newPassword !== passwordForm.value.confirmPassword
  ) {
    errors.value.confirmPassword = "كلمة المرور غير متطابقة";
    isValid = false;
  }

  return isValid;
};

const isChangePassLoading = ref(false);
const handleChangePassword = async () => {
  if (!validateForm()) return;
  try {
    isChangePassLoading.value = true;
    const response = await axiosIns.post("auth/change-password", {
      current_password: passwordForm.value.currentPassword,
      new_password: passwordForm.value.newPassword,
      new_password_confirmation: passwordForm.value.confirmPassword,
    });
    if (response.status === 200) {
      appStore.showSnackbar({
        message: response.data?.status?.message,
        color: "primary",
      });

      passwordForm.value = {
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      };
    }
  } catch (error) {
    console.log(error.response);

    appStore.showSnackbar({
      message: error.response?.data?.status?.message,
      color: "error",
    });
  } finally {
    isChangePassLoading.value = false;
  }
};

onMounted(() => {
  fetchuserProfile();
});
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
