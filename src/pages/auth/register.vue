<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";
import { ref, reactive, onBeforeMount, watch } from "vue";
import { useRouter } from "vue-router";
import {
  confirmPasswordValidator,
  emailValidator,
} from "@/@core/utils/validators";
import { useDashboardStore } from "@/stores/Dashboard";
import { computed } from "vue";
import { useDebounceFn, watchDebounced } from "@vueuse/core";

const dashboardStore = useDashboardStore();
const isPasswordVisible = ref(false);
const isConfirmPasswordVisible = ref(false);
const form = ref(null);

const formState = reactive({
  sa_id: "",
  first_name: "",
  middle_name: "",
  last_name: "",
  email: "",
  phone: "",
  password: "",
  password_confirmation: "",
  birth_date: "",
  gender: "male",
  city_id: null,
  nationality_id: null,
  location_id: null,
});

// const saIdValidator = (value) => {
//   if (!value) return "رقم الهوية مطلوب";
//   if (!/^\d+$/.test(value)) return "يجب أن يحتوي رقم الهوية على أرقام فقط";
//   if (value.length !== 10) return "يجب أن يتكون رقم الهوية من 10 أرقام";
//   return true;
// };

const saIdValidator = (value) => {
  if (!value) return "رقم الهوية مطلوب";

  // Remove any non-digit characters
  const cleanedId = value.replace(/\D/g, "");

  // Check if it matches the regex: starts with 1, 2, or 3, followed by 9 digits (total 10 digits)
  const saudiIdRegex = /^[1-3]\d{9}$/;
  if (!saudiIdRegex.test(cleanedId)) {
    return "يجب أن يبدأ رقم الهوية بـ 1 أو 2 أو 3 ويتكون من 10 أرقام";
  }

  return true; // Valid
};
const ksaPhoneValidator = (value) => {
  // Remove all non-digit characters
  const cleaned = value.replace(/\D/g, "");

  const pattern = /^(05\d{8}|9665\d{8}|\+9665\d{8})$/;

  if (!pattern.test(cleaned)) {
    return "يجب إدخال رقم جوال سعودي صحيح (يبدأ بـ 05 أو +9665)";
  }

  return true;
};
const cities = computed(() => dashboardStore.city);
const events = computed(() => dashboardStore.events);
const nationalities = computed(() => dashboardStore.nationlities);
const eventsSearch = ref("");
const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const registerUser = async () => {
  const { valid } = await form.value.validate();

  if (!valid) return;

  if (formState.password !== formState.password_confirmation) {
    toast.error("كلمة المرور وتأكيدها غير متطابقين", {
      rtl: true,
      hideProgressBar: true,
      position: "top-center",
    });
    return;
  }

  try {
    isSubmitting.value = true;
    const response = await auth.register(formState);
    if (response.data?.email && response.data?.expires_in) {
      toast.success("تم التسجيل بنجاح", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      setTimeout(() => {
        router.push({ name: "VerifyAccount" });
      }, 1000);
    }
  } catch (error) {
    console.error("Registration error:", error);

    if (error.response?.status == 422) {
      const errors = error.response.data.errors;
      Object.keys(errors).forEach((key) => {
        toast.error(errors[key][0], {
          rtl: true,
          hideProgressBar: true,
          position: "top-right",
        });
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
      toast.error("حدث خطأ أثناء التسجيل", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    }
  } finally {
    isSubmitting.value = false;
  }
};

const genderOPtions = [
  {
    title: "ذكر",
    value: "male",
  },
  {
    title: "أنثى",
    value: "female",
  },
];

const itemProps = (item) => {
  return {
    disabled: !item.is_active,
    class: !item.is_active ? "text-disabled" : "",
  };
};

const citiesLoading = ref(false);
const searchCities = useDebounceFn(async (value) => {
  citiesLoading.value = true;
  await dashboardStore.fetchCities(value);
  citiesLoading.value = false;
}, 500);

const nationalitiesLoading = ref(false);
const searchNationalities = useDebounceFn(async (value) => {
  nationalitiesLoading.value = true;
  await dashboardStore.fetchNationalities(value);
  nationalitiesLoading.value = false;
}, 500);

const loacationLoading = ref(false);
const searchEvents = useDebounceFn(async (value) => {
  loacationLoading.value = true;
  await dashboardStore.fetchEvents(eventsSearch.value, formState.city_id);
  loacationLoading.value = false;
}, 500);

watchDebounced(
  () => formState.city_id,
  async () => {
    loacationLoading.value = true;
    formState.location_id = null;
    await dashboardStore.fetchEvents(eventsSearch.value, formState.city_id);
    loacationLoading.value = false;
  },
  { deep: true, debounce: 300 }
);
onBeforeMount(() => {
  dashboardStore.fetchCities();
  dashboardStore.fetchNationalities();
});
</script>

<template>
  <VCard rounded="xl" class="px-10 py-4" style="margin-block: 50px">
    <VCardTitle style="flex: 0" class="px-0">
      <h1 class="text-h4 font-weight-bold">تسجيل حساب جديد</h1>
      <p class="text-basea">أدخل بياناتك لإنشاء حساب جديد.</p>
    </VCardTitle>
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-0">
      <VForm ref="form" @submit.prevent="registerUser">
        <VRow>
          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              رقم الهوية
            </label>
            <VTextField
              v-model="formState.sa_id"
              density="comfortable"
              placeholder="ادخل رقم الهوية"
              rounded="lg"
              :rules="[saIdValidator]"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الأول
            </label>
            <VTextField
              v-model="formState.first_name"
              density="comfortable"
              placeholder="ادخل الاسم الأول"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>
          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الثانى
            </label>
            <VTextField
              v-model="formState.middle_name"
              density="comfortable"
              placeholder="ادخل الاسم الثانى"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الاسم الأخير
            </label>
            <VTextField
              v-model="formState.last_name"
              density="comfortable"
              placeholder="ادخل الاسم الأخير"
              rounded="lg"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
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
          <VCol cols="12" md="6" class="py-1 mb-4">
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

          <VCol cols="12" md="6" class="py-1 mb-4">
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
              :rules="[requiredValidator]"
              :type="isPasswordVisible ? 'text' : 'password'"
              :append-inner-icon="
                isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
              "
              @click:append-inner="isPasswordVisible = !isPasswordVisible"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              تأكيد كلمة المرور
            </label>
            <VTextField
              v-model="formState.password_confirmation"
              variant="outlined"
              bg-color="white"
              density="comfortable"
              placeholder="أعد إدخال كلمة المرور"
              rounded="lg"
              :rules="[
                requiredValidator,
                confirmPasswordValidator(
                  formState.password,
                  formState.password_confirmation
                ),
              ]"
              :type="isConfirmPasswordVisible ? 'text' : 'password'"
              :append-inner-icon="
                isConfirmPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
              "
              @click:append-inner="
                isConfirmPasswordVisible = !isConfirmPasswordVisible
              "
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              تاريخ الميلاد
            </label>
            <VTextField
              v-model="formState.birth_date"
              density="comfortable"
              placeholder="ادخل تاريخ الميلاد"
              rounded="lg"
              :rules="[requiredValidator]"
              type="date"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الجنس
            </label>
            <VSelect
              v-model="formState.gender"
              :items="genderOPtions"
              density="comfortable"
              placeholder="اختر الجنس"
              rounded="md"
              :rules="[requiredValidator]"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              المدينة
            </label>

            <v-autocomplete
              v-model="formState.city_id"
              :items="cities"
              :loading="citiesLoading"
              item-title="name.ar"
              placeholder="ادخل المدينة"
              label="ادخل المدينة"
              item-value="id"
              no-data-text="لا توجد بيانات!"
              rounded="pill"
              clearable
              :rules="[requiredValidator]"
              @update:search="searchCities"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الجنسية
            </label>
            <v-autocomplete
              v-model="formState.nationality_id"
              :items="nationalities"
              :loading="nationalitiesLoading"
              item-title="name_ar"
              placeholder="ادخل الجنسية"
              label="ادخل الجنسية"
              item-value="id"
              no-data-text="لا توجد بيانات!"
              rounded="pill"
              clearable
              @update:search="searchNationalities"
            />
          </VCol>

          <VCol cols="12" md="6" class="py-1 mb-4" v-if="formState.city_id">
            <label class="d-block text-subtitle-1 mb-2 text-black">
              الفعالية
            </label>
            <v-autocomplete
              v-model="formState.location_id"
              :loading="loacationLoading"
              :items="events"
              v-model:search="eventsSearch"
              item-title="name"
              placeholder="ادخل الفعالية"
              label="ادخل الفعالية"
              item-value="id"
              no-data-text="لا توجد بيانات!"
              rounded="pill"
              clearable
              @update:search="searchEvents"
            />
          </VCol>

          <VCol cols="12" class="d-flex justify-end align-center mt-4">
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="54"
              color="primary"
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
