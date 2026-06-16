
<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";
import { ref, reactive, onBeforeMount } from "vue";
import { useRouter } from "vue-router";
const isPasswordVisible = ref(false);
const form = ref(null);

const formState = reactive({
  email: "",
  password: "",
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const loginUser = async () => {
  const { valid } = await form.value.validate();

  if (!valid) return;

  try {
    isSubmitting.value = true;
    const { token, user } = await auth.login(formState);

    if (token) {
      window.location.href = "/";
    } else {
      throw new Error("بيانات تسجيل الدخول غير صحيحة");
    }
  } catch (error) {
    console.error("Login error:", error);

    if (error.response?.status == 422) {
      toast.error(error.response?.message||"بيانات الدخول غير صحيحة", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      return;
    }

    if (error?.response?.data?.status?.message) {
      toast.error(error.response.data.status.message, {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    } else if (error?.message) {
      toast.error(error.message, {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    } else {
      toast.error("حدث خطأ أثناء تسجيل الدخول", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    }
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <VCard rounded="xl" class="px-10 py-4">
    <VCardTitle style="flex: 0" class="px-0">
     <h1 class="text-md-h3 text-h4 font-weight-bold">تسجيل الدخول!</h1>
      <p class="text-basea">أدخل بياناتك لتسجيل الدخول إلى حسابك.</p>
    </VCardTitle>
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-0">
      <VForm ref="form" @submit.prevent="loginUser">
        <VRow>
          <VCol cols="12" class="py-1 mb-5">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              البريد الإلكتروني
            </label>
            <VTextField
              v-model="formState.email"
              type="email"
              density="comfortable"
              placeholder="ادخل البريد الإلكتروني"
              rounded="lg"
              :rules="[(v) => !!v || 'البريد الإلكتروني مطلوب']"
            />
          </VCol>

          <VCol cols="12" class="py-1 mb-5">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              كلمة المرور
            </label>
            <VTextField
              v-model="formState.password"
              variant="outlined"
              bg-color="white"
              density="comfortable"
              placeholder="أدخل كلمة المرور"
              rounded="lg"
              :rules="[(v) => !!v || 'كلمة المرور مطلوبة']"
              :type="isPasswordVisible ? 'text' : 'password'"
              :append-inner-icon="
                isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
              "
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>
          <VCol class="d-flex justify-space-between align-center">
            <RouterLink
              class="text-decoration-underline text-grey-600"
              to="/auth/forgot-password"
            >
              نسيت كلمة المرور؟
            </RouterLink>
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="54"
              color="primary"
              width="180"
            >
              تسجيل الدخول
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>
