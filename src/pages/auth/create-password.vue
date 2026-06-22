<script setup lang="ts">
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { requiredValidator } from '@validators'

  const isPasswordVisible = ref(false)

  const formState = reactive({
    password: null,
    passwordConfirmation: null,
  })

  const auth = useAuthStore()
  const router = useRouter()
  const appStore = useAppStore()
  const route = useRoute()

  const isSubmitting = ref(false)
  const isCreatingNewPassword = computed(() => route.query.new === '1')
  const code = computed(() => route.query.code || null)
  const phoneNumber = computed(() => route.query.phone || null)

  const updatePassword = async () => {
    try {
      isSubmitting.value = true
      await auth.updatePassword({
        phone: phoneNumber.value,
        code: code.value,
        ...formState,
      })

      appStore.showSnackbar({ message: 'تم تحديث كلمة المرور بنجاح', color: 'success' })

      router.replace({
        name: 'login',
      })
    } catch (error: any) {
      if (error?.response?.message) {
        appStore.showSnackbar({ message: error.response.message, color: 'error' })
        return
      }

      appStore.showSnackbar({ message: 'خطأ غير متوقع!', color: 'error' })
    } finally {
      isSubmitting.value = false
    }
  }
</script>

<template>
  <VCard class="px-10 py-4" rounded="xl">
    <VCardTitle class="px-0" style="flex: 0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">كلمة مرور جديدة</h1>
      <p class="text-base">أدخل كلمة مرور قوية وجديدة لتأمين حسابك.</p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-0" style="flex: 0">
      <VForm @submit.prevent="updatePassword">
        <VRow>
          <VCol class="py-1 mb-4" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              كلمة المرور الجديدة
            </label>
            <VTextField
              v-model="formState.password"
              :append-inner-icon="
                isPasswordVisible ? 'mdi-eye-off' : 'mdi-eye'
              "
              bg-color="white"
              density="comfortable"
              placeholder="أدخل كلمة المرور"
              rounded="lg"
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              variant="outlined"
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>
          <VCol class="mb-4" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              تأكيد كلمة المرور الجديدة
            </label>
            <VTextField
              v-model="formState.passwordConfirmation"
              :append-inner-icon="
                isPasswordVisible ? 'mdi-eye-off' : 'mdi-eye'
              "
              bg-color="white"
              density="comfortable"
              placeholder="تأكيد كلمة المرور"
              rounded="lg"
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              variant="outlined"
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>
          <VCol cols="12 mt-4 d-flex justify-end">
            <VBtn
              class="w-100"
              :class="{ 'px-10': isCreatingNewPassword }"
              height="54"
              :loading="isSubmitting"
              max-width="200"
              rounded="pill"
              size="large"
              type="submit"
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
