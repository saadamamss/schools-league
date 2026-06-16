<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";
import VOtpInput from "vue3-otp-input";
import { onMounted } from "vue";
import Cookies from "js-cookie";
import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const email = Cookies.get("reset-otp"); //route.query.email;
if (!email) {
  router.replace({ name: "forgot-password" });
}

const formState = reactive({
  code: "",
});

const isSubmitting = ref(false);
const otpInput = ref(null);

const submitOTP = async () => {
  try {
    isSubmitting.value = true;
    const response = await auth.verifyOTP({ email, otp: formState.code });

    const data = response.data;
    if (data?.status?.success && data?.data?.can_reset_password) {
      Cookies.set("reset-otp-code", data.data.otp, { expires: 300 * 100 });
      //show message
      toast.success("تم التحقق بنجاح!", {
        rtl: true,
        hideProgressBar: true,
      });
      // got to reset teh password
      setTimeout(() => {
        router.replace({ name: "reset-password" });
        // remove expiry time
        localStorage.removeItem("reset-otp-expiry");
      }, 1000);
    }
  } catch (error) {
    console.log(error);

    if (error?.response?.data?.status?.code === 400) {
      toast.error("كود التحقق غير صحيح ؟", {
        rtl: true,
        hideProgressBar: true,
      });
      return;
    }
    if (error?.response?.data?.status?.message) {
      toast.error(error.response.data?.status.message, {
        rtl: true,
        hideProgressBar: true,
      });
      return;
    }

    toast.error("خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
  } finally {
    isSubmitting.value = false;
  }
};

const handleOnComplete = (value) => {
  formState.code = value;
  submitOTP();
};

const handleOnChange = (value) => {
  formState.code = value;
};

const remainingTime = ref("");
let countdownInterval = null;

// State
const expiryTime = ref(0);
// Computed properties for display
const minutes = computed(() => Math.floor(remainingTime.value / 60));
const seconds = computed(() => remainingTime.value % 60);
const isCount = ref(false);
// Initialize the countdown
const initCountdown = () => {
  // Get stored expiry time or set new one
  const storedExpiry = localStorage.getItem("reset-otp-expiry");

  if (storedExpiry) {
    expiryTime.value = parseInt(storedExpiry);
  } else {
    const newExpiry = Date.now();
    expiryTime.value = newExpiry;
    localStorage.setItem("reset-otp-expiry", newExpiry.toString());
  }

  startCountdown();
};

// Start the countdown timer
const startCountdown = () => {
  clearInterval(countdownInterval); // Clear any existing interval

  countdownInterval = setInterval(() => {
    const now = Date.now();
    remainingTime.value = Math.max(
      0,
      Math.floor((expiryTime.value - now) / 1000)
    );
    isCount.value = true;

    if (remainingTime.value <= 0) {
      clearInterval(countdownInterval);
    }
  }, 1000);
};

// Resend OTP function
const resendOTP = async () => {
  try {
    // Call your API to resend OTP
    const response = await auth.resetPassword(email);
    if (response.data?.status?.success) {
      // Reset the expiration
      const newExpiry = Date.now() + 300000; // 5 minutes in ms
      expiryTime.value = newExpiry;
      localStorage.setItem("reset-otp-expiry", newExpiry.toString());

      // Set cookie again
      Cookies.set("verf-mail", response.data.email, { expires: 300 / 86400 });

      // Restart countdown
      startCountdown();
      toast.success("تم إرسال رمز التحقق مرة أخرى", {
        rtl: true,
        hideProgressBar: true,
      });
    }
  } catch (error) {
    console.error("Error resending OTP:", error);
    toast.error("حدث خطأ أثناء إعادة إرسال رمز التحقق", {
      rtl: true,
      hideProgressBar: true,
    });
  }
};

const buttonText = computed(() => {
  return remainingTime.value > 0
    ? `إعادة الإرسال خلال ${minutes.value} دقيقة ${seconds.value} ثانية`
    : "إعادة إرسال الرمز";
});
// Lifecycle hooks
onMounted(() => {
  initCountdown();
});

onBeforeUnmount(() => {
  clearInterval(countdownInterval);
});
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
    <v-divider></v-divider>
    <VCardText style="flex: 0" class="px-4">
      <VForm @submit.prevent="submitOTP" class="">
        <VRow>
          <VCol cols="12" class="py-1">
            <div dir="ltr" class="mb-4">
              <VOtpInput
                ref="otpInput"
                v-model="formState.code"
                :disabled="isSubmitting"
                :rules="[requiredValidator]"
                :length="6"
                :input-classes="'otp-input'"
                separator=" "
                :num-inputs="6"
                input-type="number"
                should-auto-focus
                @on-change="handleOnChange"
                @on-complete="handleOnComplete"
              />
            </div>
          </VCol>
          <VCol cols="12" class="py-1 my-5">
            <p class="text-center font-weight-bold mb-0">لم يصلك رمز التحقق؟</p>
            <div class="d-flex justify-center">
              <VBtn
                variant="text"
                @click="resendOTP"
                :disabled="!isCount || remainingTime > 0"
              >
                {{ buttonText }}
              </VBtn>
            </div>
          </VCol>

          <VCol cols="12" class="py-1 d-flex justify-space-between">
            <VBtn
              rounded="pill"
              :disabled="isSubmitting"
              size="large"
              height="54"
              class="px-10"
              color="light-gray"
              style="max-width: 280px"
              @click="$router.replace('/auth/login')"
            >
              إلغاء
            </VBtn>
            <VBtn
              rounded="pill"
              :loading="isSubmitting"
              type="submit"
              size="large"
              height="54"
              class="px-10"
              style="max-width: 280px"
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
  border: solid 1px #eee;
  background-color: white;
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
