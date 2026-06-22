<script setup lang="ts">
  import { useAuthStore } from '@/stores/auth'
  import { useAppStore } from '@/stores/app'
  import { requiredValidator } from '@validators'
  import { computed, onBeforeMount, reactive, ref, watch } from 'vue'
  import { useRouter } from 'vue-router'
  import {
    confirmPasswordValidator,
    emailValidator,
  } from '@/@core/utils/validators'
  import { useDashboardStore } from '@/stores/dashboard'

  import { useDebounceFn, watchDebounced } from '@vueuse/core'

  const dashboardStore = useDashboardStore()
  const isPasswordVisible = ref(false)
  const isConfirmPasswordVisible = ref(false)
  const form = ref<any>(null)

  const formState = reactive({
    saId: '',
    firstName: '',
    middleName: '',
    lastName: '',
    email: '',
    phone: '',
    password: '',
    passwordConfirmation: '',
    birthDate: '',
    gender: 'male',
    cityId: null as number | null,
    nationalityId: null as number | null,
    locationId: null as number | null,
  })

  // const saIdValidator = (value) => {
  //   if (!value) return "رقم الهوية مطلوب";
  //   if (!/^\d+$/.test(value)) return "يجب أن يحتوي رقم الهوية على أرقام فقط";
  //   if (value.length !== 10) return "يجب أن يتكون رقم الهوية من 10 أرقام";
  //   return true;
  // };

  const saIdValidator = (value: string) => {
    if (!value) return 'رقم الهوية مطلوب'

    // Remove any non-digit characters
    const cleanedId = value.replace(/\D/g, '')

    // Check if it matches the regex: starts with 1, 2, or 3, followed by 9 digits (total 10 digits)
    const saudiIdRegex = /^[1-3]\d{9}$/
    if (!saudiIdRegex.test(cleanedId)) {
      return 'يجب أن يبدأ رقم الهوية بـ 1 أو 2 أو 3 ويتكون من 10 أرقام'
    }

    return true // Valid
  }
  const ksaPhoneValidator = (value: string) => {
    // Remove all non-digit characters
    const cleaned = value.replace(/\D/g, '')

    const pattern = /^(05\d{8}|9665\d{8}|\+9665\d{8})$/

    if (!pattern.test(cleaned)) {
      return 'يجب إدخال رقم جوال سعودي صحيح (يبدأ بـ 05 أو +9665)'
    }

    return true
  }
  const cities = computed(() => dashboardStore.city)
  const locations = computed(() => dashboardStore.locations)
  const nationalities = computed(() => dashboardStore.nationalities)
  const locationsSearch = ref('')
  const auth = useAuthStore()
  const router = useRouter()
  const appStore = useAppStore()

  const isSubmitting = ref(false)

  const registerUser = async () => {
    const { valid } = await form.value?.validate() ?? { valid: false }

    if (!valid) return

    if (formState.password !== formState.passwordConfirmation) {
      appStore.showSnackbar({ message: 'كلمة المرور وتأكيدها غير متطابقين', color: 'error' })
      return
    }

    try {
      isSubmitting.value = true
      const response = await auth.register(formState)
      if (response?.status?.success) {
        appStore.showSnackbar({ message: 'تم التسجيل بنجاح', color: 'success' })
        setTimeout(() => {
          router.push({ name: 'VerifyAccount' })
        }, 1000)
      }
    } catch (error: any) {
      if (error.response?.status === 422) {
        const errors = error.response.data.errors
        Object.keys(errors).forEach(key => {
          appStore.showSnackbar({ message: errors[key][0], color: 'error' })
        })

        return
      }

      if (error?.response?.data?.status?.message) {
        appStore.showSnackbar({ message: error.response.data.status.message, color: 'error' })
      } else if (error?.message) {
        appStore.showSnackbar({ message: error.message, color: 'error' })
      } else {
        appStore.showSnackbar({ message: 'حدث خطأ أثناء التسجيل', color: 'error' })
      }
    } finally {
      isSubmitting.value = false
    }
  }

  const genderOPtions = [
    {
      title: 'ذكر',
      value: 'male',
    },
    {
      title: 'أنثى',
      value: 'female',
    },
  ]

  const itemProps = (item: Record<string, any>) => {
    return {
      disabled: !item.isActive,
      class: !item.isActive ? 'text-disabled' : '',
    }
  }

  const citiesLoading = ref(false)
  const searchCities = useDebounceFn(async value => {
    citiesLoading.value = true
    await dashboardStore.fetchCities(value)
    citiesLoading.value = false
  }, 500)

  const nationalitiesLoading = ref(false)
  const searchNationalities = useDebounceFn(async value => {
    nationalitiesLoading.value = true
    await dashboardStore.fetchNationalities(value)
    nationalitiesLoading.value = false
  }, 500)

  const loacationLoading = ref(false)
  const searchLocations = useDebounceFn(async value => {
    loacationLoading.value = true
    await dashboardStore.fetchLocations(locationsSearch.value, formState.cityId ?? undefined)
    loacationLoading.value = false
  }, 500)

  watchDebounced(
    () => formState.cityId,
    async () => {
      loacationLoading.value = true
      formState.locationId = null
      await dashboardStore.fetchLocations(locationsSearch.value, formState.cityId ?? undefined)
      loacationLoading.value = false
    },
    { deep: true, debounce: 300 }
  )
  onBeforeMount(() => {
    dashboardStore.fetchCities()
    dashboardStore.fetchNationalities()
  })
</script>

<template>
  <VCard class="px-10 py-4" rounded="xl" style="margin-block: 50px">
    <VCardTitle class="px-0" style="flex: 0">
      <h1 class="text-h4 font-weight-bold">تسجيل حساب جديد</h1>
      <p class="text-basea">أدخل بياناتك لإنشاء حساب جديد.</p>
    </VCardTitle>
    <v-divider />
    <VCardText class="px-0" style="flex: 0">
      <VForm ref="form" @submit.prevent="registerUser">
        <VRow>
          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              رقم الهوية
            </label>
            <VTextField
              v-model="formState.saId"
              density="comfortable"
              placeholder="ادخل رقم الهوية"
              rounded="lg"
              :rules="[saIdValidator]"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الأول
            </label>
            <VTextField
              v-model="formState.firstName"
              density="comfortable"
              placeholder="ادخل الاسم الأول"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>
          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الثانى
            </label>
            <VTextField
              v-model="formState.middleName"
              density="comfortable"
              placeholder="ادخل الاسم الثانى"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الأخير
            </label>
            <VTextField
              v-model="formState.lastName"
              density="comfortable"
              placeholder="ادخل الاسم الأخير"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              البريد الإلكترونى
            </label>
            <VTextField
              v-model="formState.email"
              density="comfortable"
              placeholder="البريد الإلكترونى"
              rounded="lg"
              :rules="[requiredValidator, emailValidator]"
            />
          </VCol>
          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              رقم الجوال
            </label>
            <VTextField
              v-model="formState.phone"
              density="comfortable"
              placeholder="ادخل رقم الجوال"
              rounded="lg"
              :rules="[requiredValidator, ksaPhoneValidator]"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
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
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              variant="outlined"
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              تأكيد كلمة المرور
            </label>
            <VTextField
              v-model="formState.passwordConfirmation"
              :append-inner-icon="
                isConfirmPasswordVisible ? 'mdi-eye-off' : 'mdi-eye'
              "
              bg-color="white"
              density="comfortable"
              placeholder="أعد إدخال كلمة المرور"
              rounded="lg"
              :rules="[
                requiredValidator,
                confirmPasswordValidator(
                  formState.password,
                  formState.passwordConfirmation
                ),
              ]"
              :type="isConfirmPasswordVisible ? 'text' : 'password'"
              variant="outlined"
              @click:append-inner="
                isConfirmPasswordVisible = !isConfirmPasswordVisible
              "
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              تاريخ الميلاد
            </label>
            <VTextField
              v-model="formState.birthDate"
              density="comfortable"
              placeholder="ادخل تاريخ الميلاد"
              rounded="lg"
              :rules="[requiredValidator]"
              type="date"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الجنس
            </label>
            <VSelect
              v-model="formState.gender"
              density="comfortable"
              :items="genderOPtions"
              placeholder="اختر الجنس"
              rounded="md"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              المدينة
            </label>

            <v-autocomplete
              v-model="formState.cityId"
              clearable
              item-title="name.ar"
              item-value="id"
              :items="cities"
              label="ادخل المدينة"
              :loading="citiesLoading"
              no-data-text="لا توجد بيانات!"
              placeholder="ادخل المدينة"
              rounded="pill"
              :rules="[requiredValidator]"
              @update:search="searchCities"
            />
          </VCol>

          <VCol class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الجنسية
            </label>
            <v-autocomplete
              v-model="formState.nationalityId"
              clearable
              item-title="name_ar"
              item-value="id"
              :items="nationalities"
              label="ادخل الجنسية"
              :loading="nationalitiesLoading"
              no-data-text="لا توجد بيانات!"
              placeholder="ادخل الجنسية"
              rounded="pill"
              @update:search="searchNationalities"
            />
          </VCol>

          <VCol v-if="formState.cityId" class="py-1 mb-4" cols="12" md="6">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الموقع
            </label>
            <v-autocomplete
              v-model="formState.locationId"
              v-model:search="locationsSearch"
              clearable
              item-title="name"
              item-value="id"
              :items="locations"
              label="ادخل الموقع"
              :loading="loacationLoading"
              no-data-text="لا توجد بيانات!"
              placeholder="ادخل الموقع"
              rounded="pill"
              @update:search="searchLocations"
            />
          </VCol>

          <VCol class="d-flex justify-end align-center mt-4" cols="12">
            <VBtn
              color="primary"
              height="54"
              :loading="isSubmitting"
              rounded="pill"
              size="large"
              type="submit"
              width="180"
            >
              تسجيل
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
    </VCardText>
  </VCard>
</template>
