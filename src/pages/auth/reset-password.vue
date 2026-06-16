<script setup>
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "@/stores/Auth";
import {
  requiredValidator,
  passwordValidator,
  confirmPasswordValidator,
} from "@validators";
import { toast } from "vue3-toastify";
import Cookies from "js-cookie";
const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const email = Cookies.get("reset-otp"); // route.query.email;
const otp = Cookies.get("reset-otp-code"); // route.query.code;

if (!email || !otp) {
  router.replace({ name: "forgot-password" });
}

const formState = reactive({
  email,
  otp,
  password: "",
  password_confirmation: "",
});

const isSubmitting = ref(false);
const showPassword = ref(false);
const showConfirmPassword = ref(false);

const submitNewPassword = async () => {
  try {
    isSubmitting.value = true;
    const response = await auth.updatePassword(formState);

    if (response.data?.status?.success) {
      toast.success("تم تغيير كلمة المرور بنجاح", {
        rtl: true,
        hideProgressBar: true,
      });
      setTimeout(() => {
        router.replace({ name: "login" });
        Cookies.remove("reset-otp");
        Cookies.remove("reset-otp-code");
      }, 1000);
    } else {
      throw new Error(response.data?.status?.message);
    }
  } catch (error) {
    console.error(error);
    if (error?.response?.data?.status?.code === 400) {
      toast.error("كود التحقق غير صحيح ؟", {
        rtl: true,
        hideProgressBar: true,
      });
      return;
    }
    if (error?.response?.data?.status?.message) {
      toast.error(error.response.data?.status.message, {
        rtl: true,
        hideProgressBar: true,
      });
      return;
    }

    toast.error("خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <VCard class="w-100 app-logo py-4 px-10" rounded="xl">
    <VCardTitle style="flex: 0" class="px-0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">كلمة مرور جديدة</h1>
      <p class="text-base">أدخل كلمة مرور قوية وجديدة لتأمين حسابك.</p>
    </VCardTitle>
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-0">
      <VForm @submit.prevent="submitNewPassword">
        <VRow>
          <VCol cols="12" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              كلمة المرور
            </label>
            <VTextField
              v-model="formState.password"
              :type="showPassword ? 'text' : 'password'"
              bg-color="white"
              density="comfortable"
              placeholder="أنشئ كلمة مرور قوية"
              rounded="lg"
              :rules="[requiredValidator, passwordValidator]"
              dir="rtl"
              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
              @click:append-inner="showPassword = !showPassword"
            />
          </VCol>

          <VCol cols="12" class="py-1 mb-10">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              تأكيد كلمة المرور
            </label>
            <VTextField
              v-model="formState.password_confirmation"
              :type="showConfirmPassword ? 'text' : 'password'"
              bg-color="white"
              density="comfortable"
              placeholder="أعد كتابة كلمة المرور"
              rounded="lg"
              :rules="[
                requiredValidator,
                (val) => confirmPasswordValidator(val, formState.password),
              ]"
              dir="rtl"
              :append-inner-icon="
                showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye'
              "
              @click:append-inner="showConfirmPassword = !showConfirmPassword"
            />
          </VCol>

          <VCol cols="12" class="py-1 d-flex justify-end">
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="50"
              class="px-10"
            >
              تحديث كلمة المرور
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>

<style lang="scss" scoped>
.auth-wrapper {
  background-size: cover;
}
</style>
