<script setup>
import Logo from "@/@core/components/icons/logo.vue";
import { useAuthStore } from "@/stores/Auth";
import { emailValidator, requiredValidator } from "@validators";
import { toast } from "vue3-toastify";
import Cookies from "js-cookie";
const formState = reactive({
  email: null,
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const submitEmail = async () => {
  try {
    isSubmitting.value = true;
    const { data } = await auth.resetPassword(formState.email);

    if (data.status?.code === 200) {
      const expiresInMs = 300 * 1000; // 300 seconds in milliseconds
      const expiryTime = new Date().getTime() + expiresInMs;

      Cookies.set("reset-otp", formState.email, {
        expires: 300 / 86400,
      });

      localStorage.setItem("reset-otp-expiry", expiryTime.toString());
      //
      toast.success("إفحص بريدك الإلكترونى!", {
        rtl: true,
        hideProgressBar: true,
      });
      setTimeout(() => {
        router.replace({ name: "otp" });
      }, 1000);
    }
  } catch (error) {
    console.error(error);

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
  <VCard rounded="xl" class="px-10 py-4">
    <VCardTitle style="flex: 0" class="px-0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">هل نسيت كلمة المرور؟</h1>
      <p class="text-sm">
        أدخل بريدك الإلكتروني المرتبط بحسابك، وسنرسل إليك رمز تحقق لإعادة تعيين
        كلمة المرور.
      </p>
    </VCardTitle>
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-0">
      <VForm ref="form" @submit.prevent="submitEmail">
        <VRow>
          <VCol cols="12" class="py-1 mb-5">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              البريد الإلكتروني
            </label>
            <VTextField
              v-model="formState.email"
              type="email"
              bg-color="white"
              density="comfortable"
              placeholder="ادخل البريد الإلكتروني"
              rounded="lg"
              :rules="[(v) => !!v || 'البريد الإلكتروني مطلوب']"
            />
          </VCol>

          <VCol cols="12" class="py-1 d-flex justify-space-between py-4 mt-3">
            <VBtn
              rounded="pill"
              :disabled="isSubmitting"
              size="large"
              height="54"
              class="px-10"
              color="light-gray"
              style="max-width: 280px"
              @click="$router.replace('/auth/login')"
              width="180"
            >
              إلغاء
            </VBtn>
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="54"
              color="primary"
              width="180"
            >
              إرسال رمز التحقق
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>

<style lang="scss"></style>
