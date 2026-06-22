<script setup lang="ts">
  import Logo from '@/@core/components/icons/logo.vue'
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { emailValidator, requiredValidator } from '@validators'
  import Cookies from 'js-cookie'
  const formState = reactive({
    email: '',
  })

  const auth = useAuthStore()
  const router = useRouter()
  const appStore = useAppStore()

  const isSubmitting = ref(false)

  const submitEmail = async () => {
    try {
      isSubmitting.value = true
      const { data } = await auth.resetPassword(formState.email)

      if (data.status?.code === 200) {
        const expiresInMs = 300 * 1000 // 300 seconds in milliseconds
        const expiryTime = new Date().getTime() + expiresInMs

        Cookies.set('reset-otp', formState.email, {
          expires: 300 / 86400,
        })

        localStorage.setItem('reset-otp-expiry', expiryTime.toString())
        //
        appStore.showSnackbar({ message: 'إفحص بريدك الإلكترونى!', color: 'success' })
        setTimeout(() => {
          router.replace({ name: 'otp' })
        }, 1000)
      }
    } catch (error: any) {
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
  <VCard class="px-10 py-4" rounded="xl">
    <VCardTitle class="px-0" style="flex: 0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">هل نسيت كلمة المرور؟</h1>
      <p class="text-sm">
        أدخل بريدك الإلكتروني المرتبط بحسابك، وسنرسل إليك رمز تحقق لإعادة تعيين
        كلمة المرور.
      </p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-0" style="flex: 0">
      <VForm ref="form" @submit.prevent="submitEmail">
        <VRow>
          <VCol class="py-1 mb-5" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              البريد الإلكتروني
            </label>
            <VTextField
              v-model="formState.email"
              bg-color="white"
              density="comfortable"
              placeholder="ادخل البريد الإلكتروني"
              rounded="lg"
              :rules="[(v) => !!v || 'البريد الإلكتروني مطلوب']"
              type="email"
            />
          </VCol>

          <VCol class="py-1 d-flex justify-space-between py-4 mt-3" cols="12">
            <VBtn
              class="px-10"
              color="light-gray"
              :disabled="isSubmitting"
              height="54"
              rounded="pill"
              size="large"
              style="max-width: 280px"
              width="180"
              @click="$router.replace('/auth/login')"
            >
              إلغاء
            </VBtn>
            <VBtn
              color="primary"
              height="54"
              :loading="isSubmitting"
              rounded="pill"
              size="large"
              type="submit"
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
