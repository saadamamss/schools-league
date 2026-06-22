<script setup lang="ts">
  import { useRoute, useRouter } from 'vue-router'
  import { useAuthStore } from '@/stores/auth'
  import {
    confirmPasswordValidator,
    passwordValidator,
    requiredValidator,
  } from '@validators'
  import { useAppStore } from '@/stores/app'
  import Cookies from 'js-cookie'
  const route = useRoute()
  const router = useRouter()
  const auth = useAuthStore()
  const appStore = useAppStore()

  const email = Cookies.get('reset-otp') // route.query.email;
  const otp = Cookies.get('reset-otp-code') // route.query.code;

  if (!email || !otp) {
    router.replace({ name: 'forgot-password' })
  }

  const formState = reactive({
    email,
    otp,
    password: '',
    passwordConfirmation: '',
  })

  const isSubmitting = ref(false)
  const showPassword = ref(false)
  const showConfirmPassword = ref(false)

  const submitNewPassword = async () => {
    try {
      isSubmitting.value = true
      const response = await auth.updatePassword(formState)

      if (response.data?.status?.success) {
        appStore.showSnackbar({ message: 'تم تغيير كلمة المرور بنجاح', color: 'success' })
        setTimeout(() => {
          router.replace({ name: 'login' })
          Cookies.remove('reset-otp')
          Cookies.remove('reset-otp-code')
        }, 1000)
      } else {
        throw new Error(response.data?.status?.message)
      }
    } catch (error: any) {
      if (error?.response?.data?.status?.code === 400) {
        appStore.showSnackbar({ message: 'كود التحقق غير صحيح ؟', color: 'error' })
        return
      }
      if (error?.response?.data?.status?.message) {
        appStore.showSnackbar({ message: error.response.data?.status.message, color: 'error' })
        return
      }

      appStore.showSnackbar({ message: 'خطأ غير متوقع!', color: 'error' })
    } finally {
      isSubmitting.value = false
    }
  }
</script>

<template>
  <VCard class="w-100 app-logo py-4 px-10" rounded="xl">
    <VCardTitle class="px-0" style="flex: 0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">كلمة مرور جديدة</h1>
      <p class="text-base">أدخل كلمة مرور قوية وجديدة لتأمين حسابك.</p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-0" style="flex: 0">
      <VForm @submit.prevent="submitNewPassword">
        <VRow>
          <VCol class="py-1 mb-4" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              كلمة المرور
            </label>
            <VTextField
              v-model="formState.password"
              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
              bg-color="white"
              density="comfortable"
              dir="rtl"
              placeholder="أنشئ كلمة مرور قوية"
              rounded="lg"
              :rules="[requiredValidator, passwordValidator]"
              :type="showPassword ? 'text' : 'password'"
              @click:append-inner="showPassword = !showPassword"
            />
          </VCol>

          <VCol class="py-1 mb-10" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-dark">
              تأكيد كلمة المرور
            </label>
            <VTextField
              v-model="formState.passwordConfirmation"
              :append-inner-icon="
                showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye'
              "
              bg-color="white"
              density="comfortable"
              dir="rtl"
              placeholder="أعد كتابة كلمة المرور"
              rounded="lg"
              :rules="[
                requiredValidator,
                (val) => confirmPasswordValidator(val, formState.password),
              ]"
              :type="showConfirmPassword ? 'text' : 'password'"
              @click:append-inner="showConfirmPassword = !showConfirmPassword"
            />
          </VCol>

          <VCol class="py-1 d-flex justify-end" cols="12">
            <VBtn
              class="px-10"
              height="50"
              :loading="isSubmitting"
              rounded="pill"
              size="large"
              type="submit"
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
