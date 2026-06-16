<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const isPasswordVisible = ref(false);

const formState = reactive({
  password: null,
  password_confirmation: null,
});

const auth = useAuthStore();
const router = useRouter();
const route = useRoute();

const isSubmitting = ref(false);
const isCreatingNewPassword = computed(() => route.query.new == 1);
const code = computed(() => route.query.code || null);
const phoneNumber = computed(() => route.query.phone || null);

const updatePassword = async () => {
  try {
    isSubmitting.value = true;
    await auth.updatePassword({
      phone: phoneNumber.value,
      code: code.value,
      ...formState,
    });

    toast.success("تم تحديث كلمة المرور بنجاح", {
      rtl: true,
      hideProgressBar: true,
    });

    router.replace({
      name: "login",
    });
  } catch (error) {
    console.error(error);

    if (error?.response?.message) {
      toast.error(error.response.message, { rtl: true, hideProgressBar: true });
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
      <h1 class="text-md-h3 text-h4 font-weight-bold">كلمة مرور جديدة</h1>
      <p class="text-base">أدخل كلمة مرور قوية وجديدة لتأمين حسابك.</p>
    </VCardTitle>
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-0">
      <VForm @submit.prevent="updatePassword">
        <VRow>
          <VCol cols="12" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              كلمة المرور الجديدة
            </label>
            <VTextField
              v-model="formState.password"
              variant="outlined"
              bg-color="white"
              density="comfortable"
              placeholder="أدخل كلمة المرور"
              rounded="lg"
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              :append-inner-icon="
                isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
              "
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>
          <VCol cols="12" class="mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              تأكيد كلمة المرور الجديدة
            </label>
            <VTextField
              v-model="formState.password_confirmation"
              variant="outlined"
              bg-color="white"
              density="comfortable"
              placeholder="تأكيد كلمة المرور"
              rounded="lg"
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              :append-inner-icon="
                isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
              "
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>
          <VCol cols="12 mt-4 d-flex justify-end">
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="54"
              class="w-100"
              max-width="200"
              :class="{ 'px-10': isCreatingNewPassword }"
            >
              <span v-if="isCreatingNewPassword">تأكيد</span>
              <span v-else>تغيير كلمة المرور</span>
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>

<!-- <style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";

.text-danger {
  color: red;
}

.auth-wrapper {
  background-size: cover;
}
</style> -->
