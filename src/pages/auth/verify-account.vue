<script setup lang="ts">
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { requiredValidator } from '@validators'
  import VOtpInput from 'vue3-otp-input'
  import { computed, onMounted } from 'vue'
  import Cookies from 'js-cookie'

  import { useRoute, useRouter } from 'vue-router'
  const router = useRouter()
  const auth = useAuthStore()
  const appStore = useAppStore()

  const email = Cookies.get('verif-mail') || ''

  if (!email) {
    router.replace({ name: 'forgot-password' })
  }

  const formState = reactive({
    code: '',
  })

  const isSubmitting = ref(false)
  const otpInput = ref<any>(null)

  const submitOTP = async () => {
    try {
      isSubmitting.value = true
      const response = await auth.verifyAccount({ email, otp: formState.code })

      if (response.token) {
        appStore.showSnackbar({ message: 'تم التحقق بنجاح!', color: 'success' })
        setTimeout(() => {
          router.replace('/')
          localStorage.removeItem('verif-mail-expiry')
        }, 1000)
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

  const handleOnComplete = (value: string) => {
    formState.code = value
    submitOTP()
  }

  const handleOnChange = (value: string) => {
    formState.code = value
  }

  const remainingTime = ref(0)
  let countdownInterval: ReturnType<typeof setInterval> | null = null

  // State
  const expiryTime = ref(0)
  // Computed properties for display
  const minutes = computed(() => Math.floor(remainingTime.value / 60))
  const seconds = computed(() => remainingTime.value % 60)
  const isCount = ref(false)
  // Initialize the countdown
  const initCountdown = () => {
    // Get stored expiry time or set new one
    const storedExpiry = localStorage.getItem('verif-mail-expiry')

    if (storedExpiry) {
      expiryTime.value = parseInt(storedExpiry)
    } else {
      const newExpiry = Date.now()
      expiryTime.value = newExpiry
      localStorage.setItem('verif-mail-expiry', newExpiry.toString())
    }

    startCountdown()
  }

  // Start the countdown timer
  const startCountdown = () => {
    if (countdownInterval !== null) clearInterval(countdownInterval)

    countdownInterval = setInterval(() => {
      const now = Date.now()
      remainingTime.value = Math.max(
        0,
        Math.floor((expiryTime.value - now) / 1000)
      )
      isCount.value = true

      if (remainingTime.value <= 0) {
        if (countdownInterval !== null) clearInterval(countdownInterval)
      }
    }, 1000)
  }

  // Resend OTP function
  const resendOTP = async () => {
    try {
      // Call your API to resend OTP
      const response = await auth.resendVerificationOTP(email)
      if (response.data?.status?.success) {
        // Reset the expiration
        const newExpiry = Date.now() + 300000 // 5 minutes in ms
        expiryTime.value = newExpiry
        localStorage.setItem('verif-mail-expiry', newExpiry.toString())

        // Set cookie again
        Cookies.set('verif-mail', response.data.email, { expires: 300 / 86400 })

        // Restart countdown
        startCountdown()
        appStore.showSnackbar({ message: 'تم إرسال رمز التحقق مرة أخرى', color: 'success' })
      }
    } catch (error: any) {
      appStore.showSnackbar({ message: 'حدث خطأ أثناء إعادة إرسال رمز التحقق', color: 'error' })
    }
  }

  const buttonText = computed(() => {
    return remainingTime.value > 0
      ? `إعادة الإرسال خلال ${minutes.value} دقيقة ${seconds.value} ثانية`
      : 'إعادة إرسال الرمز'
  })
  // Lifecycle hooks
  onMounted(() => {
    initCountdown()
  })

  onBeforeUnmount(() => {
    if (countdownInterval !== null) clearInterval(countdownInterval)
  })
</script>

<template>
  <VCard class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12" rounded="xl">
    <VCardTitle>
      <h1 class="text-md-h3 text-h4 font-weight-bold">
        تأكيد البريد الالكتروني
      </h1>
      <p class="text-base">
        تم إرسال رمز التحقق إلى بريدك الإلكتروني {{ $route.query.email }} ، يرجى
        إدخال الرمز لتأكيد العملية
      </p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-4" style="flex: 0">
      <VForm class="" @submit.prevent="submitOTP">
        <VRow>
          <VCol class="py-1" cols="12">
            <div class="mb-4" dir="ltr">
              <VOtpInput
                ref="otpInput"
                v-model="formState.code"
                :disabled="isSubmitting"
                :input-classes="'otp-input'"
                input-type="number"
                :length="6"
                :num-inputs="6"
                :rules="[requiredValidator]"
                separator=" "
                should-auto-focus
                @on-change="handleOnChange"
                @on-complete="handleOnComplete"
              />
            </div>
          </VCol>
          <VCol class="py-1 my-5" cols="12">
            <p class="text-center font-weight-bold mb-0">لم يصلك رمز التحقق؟</p>
            <div class="d-flex justify-center">
              <VBtn
                :disabled="!isCount || remainingTime > 0"
                variant="text"
                @click="resendOTP"
              >
                {{ buttonText }}
              </VBtn>
            </div>
          </VCol>

          <VCol class="py-1 d-flex justify-space-between" cols="12">
            <VBtn
              class="px-10"
              color="light-gray"
              :disabled="isSubmitting"
              height="54"
              rounded="pill"
              size="large"
              style="max-width: 280px"
              @click="$router.replace('/auth/login')"
            >
              إلغاء
            </VBtn>
            <VBtn
              class="px-10"
              height="54"
              :loading="isSubmitting"
              rounded="pill"
              size="large"
              style="max-width: 280px"
              type="submit"
            >
              تأكيد البريد الإلكترونى
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>

<style lang="scss" scoped>
:deep(.vue-otp-input) {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
}

:deep(.otp-input) {
  width: 50px !important;
  height: 50px !important;
  padding: 0.5rem;
  border-radius: 8px;
  box-shadow: 0 0 5px 0px rgba($color: #000000, $alpha: 0.1);
  border: solid 1px rgba(var(--v-theme-on-surface), 0.08);
  background-color: rgb(var(--v-theme-surface));
  font-size: 1.4rem;
  text-align: center;

  &:focus {
    outline: none;
    border-color: rgb(var(--v-theme-primary));
  }
  @media (max-width: 786px) {
    width: 40px !important;
    height: 40px !important;
    font-size: 1.1rem;
  }
}

.auth-wrapper {
  background-size: cover;
}
.otp-input-container {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
}
</style>
