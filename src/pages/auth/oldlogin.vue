<template>
  <div class="login-wrapper">
    <v-main class="grey lighten-3 h-100 py-0 px-0">
      <v-container class="fill-height px-0 py-0" fluid>
        <v-card
          class="overflow-hidden px-0 py-0 w-100 bg-white"
          elevation="10"
          height="100vh"
        >
          <v-card-text
            class="d-flex justify-center justify-lg-start px-0 py-0 h-100"
          >
            <!-- Left side - Decorative/image section -->
            <div
              class="primary-gradient flex-1 d-none d-md-flex align-center px-0"
            >
              <v-card-text class="text-start pa-8">
                <h2 class="text-h3 text-white font-bold mb-1">
                  # دوري المدارس
                </h2>
                <p
                  class="text-base text-white mb-8 w-75"
                  style="line-height: 1.8"
                >
                  دوري المدارس هو مشروع رياضي وطني أطلقته وزارة الرياضة بالتعاون
                  مع وزارة التعليم، يهدف إلى اكتشاف ورعاية المواهب الرياضية في
                  المدارس، لبناء جيل رياضي متميّز يمثل المملكة في المنافسات
                  المحلية والدولية. يشمل المشروع البنين والبنات في مختلف
                  المناطق.
                </p>

                <v-btn to="#" rounded="pill"> معرفة المزيد </v-btn>
              </v-card-text>
            </div>

            <!-- Right side - Login form -->
            <div
              class="d-flex form-container flex-1 align-center justify-center justify-lg-start"
            >
              <div class="login-form">
                <h1 class="text-h4 font-weight-bold text-dark">
                  مرحبًا بعودتك
                </h1>
                <p class="text-body-1 text-sec-text mb-10">
                  أدخل بياناتك لتسجيل الدخول إلى حسابك.
                </p>

                <v-form ref="form" @submit.prevent="loginUser">
                  <v-text-field
                    v-model="formState.email"
                    outlined
                    label="البريد الألكتروني"
                    placeholder="أدخل البريد الألكتروني"
                    type="email"
                    prepend-inner-icon="mdi-email"
                    :rules="emailRules"
                    required
                    rounded="pill"
                    class="mb-6"
                  ></v-text-field>

                  <v-text-field
                    v-model="formState.password"
                    outlined
                    label="كلمة المرور"
                    placeholder="أدخل كلمة المرور"
                    prepend-inner-icon="mdi-lock"
                    :type="isPasswordVisible ? 'text' : 'password'"
                    :append-inner-icon="
                      isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
                    "
                    @click:append-inner="isPasswordVisible = !isPasswordVisible"
                    :rules="passwordRules"
                    required
                    rounded="pill"
                    class="mb-6"
                  ></v-text-field>

                  <v-btn
                    type="submit"
                    block
                    large
                    color="primary"
                    class="py-6"
                    rounded="pill"
                    :loading="isSubmitting"
                    :disabled="isSubmitting"
                  >
                    تسجيل الدخول
                  </v-btn>

                  <div class="mt-4 text-center">
                    <v-btn variant="text" color="dark">نسيت كلمة المرور؟</v-btn>
                  </div>
                </v-form>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-container>
    </v-main>
  </div>
</template>

<script setup>
import { useAuthStore } from "@/stores/Auth";
import { ref } from "vue";
import { reactive } from "vue";
import { useRouter } from "vue-router";
import { toast } from "vue3-toastify";

const emailRules = [
  (v) => !!v || "البريد الإلكتروني مطلوب",
  (v) => /.+@.+\..+/.test(v) || "يجب أن يكون البريد الإلكتروني صالحًا",
];
const passwordRules = [
  (v) => !!v || "كلمة المرور مطلوبة",
  (v) => (v && v.length >= 6) || "يجب أن تكون كلمة المرور 6 أحرف على الأقل",
];

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
      location.href = "/";
      // location.href = "/rating";
    } else {
      throw new Error("بيانات تسجيل الدخول غير صحيحة");
    }
  } catch (error) {
    console.error("Login error:", error);

    if (error.response?.status == 422) {
      toast.error("بيانات الدخول غير صحيحة", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      return;
    }

    if (error?.response?.data?.message) {
      toast.error(error.response.data.message, {
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
<style lang="scss" scoped>
.login-wrapper {
  min-height: 100vh;
}
.primary-gradient {
  background: linear-gradient(135deg, #37bf9b 0%, #184142 100%);
  max-width: 65%;
}
.form-container {
  max-width: 400px;
  margin-inline-start: 6rem;
}

/* Ensure RTL direction for all elements */
[dir="rtl"] {
  text-align: right;
}

/* Adjust Vuetify components for RTL */
[dir="rtl"] .v-input__prepend-inner {
  margin-right: 0;
  margin-left: 8px;
}

[dir="rtl"] .v-input__icon--prepend-inner .v-icon {
  transform: scaleX(-1);
}
:deep(.v-field) {
  background-color: #fff !important;
  // color: rgb(194 194 194) !important;
  color: rgb(var(--v-theme-dark)) !important;
  // &.v-field--focused {
  //   color: rgb(var(--v-theme-primary-text)) !important;
  // }

  // &.v-field--active {
  //   color: rgb(var(--v-theme-primary-text)) !important;
  // }

  .v-field__outline {
    --v-field-border-opacity: 1 !important;
  }
}
.flex-1 {
  flex: 1;
}
.login-form {
  max-width: 320px;
  width: 100%;
}

@media (max-width: 1279.1px) {
  .form-container {
    max-width: 400px;
    margin-inline-start: 0rem;
  }
}
@media (max-width: 959.1px) {
  .login-form {
    max-width: 400px !important;
  }
}
</style>
