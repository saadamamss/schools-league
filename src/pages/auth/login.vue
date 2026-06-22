<script setup lang="ts">
  import { useRouter } from 'vue-router'
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { requiredValidator } from '@validators'
  import { reactive, ref } from 'vue'
  const isPasswordVisible = ref(false)
  const router = useRouter()
  const form = ref<any>(null)

  const formState = reactive({
    email: '',
    password: '',
  })

  const auth = useAuthStore()
  const appStore = useAppStore()

  const isSubmitting = ref(false)

  const loginUser = async () => {
    const { valid } = await form.value?.validate() ?? { valid: false }

    if (!valid) return

    try {
      isSubmitting.value = true
      const { token, user } = await auth.login(formState)

      if (token) {
        await router.push('/')
      } else {
        throw new Error('بيانات تسجيل الدخول غير صحيحة')
      }
    } catch (error: any) {
      if (error.response?.status === 422) {
        appStore.showSnackbar({
          message: error.response?.message || 'بيانات الدخول غير صحيحة',
          color: 'error',
        })
        return
      }

      if (error?.response?.data?.status?.message) {
        appStore.showSnackbar({
          message: error.response.data.status.message,
          color: 'error',
        })
      } else if (error?.message) {
        appStore.showSnackbar({
          message: error.message,
          color: 'error',
        })
      } else {
        appStore.showSnackbar({
          message: 'حدث خطأ أثناء تسجيل الدخول',
          color: 'error',
        })
      }
    } finally {
      isSubmitting.value = false
    }
  }
</script>

<template>
  <VCard class="px-10 py-4" rounded="xl">
    <VCardTitle class="px-0" style="flex: 0">
      <h1 class="text-md-h3 text-h4 font-weight-bold">تسجيل الدخول!</h1>
      <p class="text-basea">أدخل بياناتك لتسجيل الدخول إلى حسابك.</p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-0" style="flex: 0">
      <VForm ref="form" @submit.prevent="loginUser">
        <VRow>
          <VCol class="py-1 mb-5" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              البريد الإلكتروني
            </label>
            <VTextField
              v-model="formState.email"
              density="comfortable"
              placeholder="ادخل البريد الإلكتروني"
              rounded="lg"
              :rules="[(v) => !!v || 'البريد الإلكتروني مطلوب']"
              type="email"
            />
          </VCol>

          <VCol class="py-1 mb-5" cols="12">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              كلمة المرور
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
              :rules="[(v) => !!v || 'كلمة المرور مطلوبة']"
              :type="isPasswordVisible ? 'text' : 'password'"
              variant="outlined"
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
              color="primary"
              height="54"
              :loading="isSubmitting"
              rounded="pill"
              size="large"
              type="submit"
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
