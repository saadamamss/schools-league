<script setup>
// Remove TypeScript for now since the project setup doesn't seem to fully support it
import { useRouter } from "vue-router";

const props = defineProps({
  data: {
    type: Object,
    required: true,
    validator: (value) => {
      return (
        value &&
        typeof value.id !== "undefined" &&
        typeof value.title !== "undefined" &&
        typeof value.start_date !== "undefined" &&
        typeof value.end_date !== "undefined"
      );
    },
  },
});

const router = useRouter();

const goToDetails = () => {
  router.push({
    path: `/phases/details/${props.data.id}`,
  });
};
</script>

<template>
  <VCard
    class="rounded-lg"
    style="box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)"
  >
    <v-card-text class="pa-3 pa-sm-4">
      <div class="d-flex justify-space-between align-center ga-2">
        <div>
          <span class="text-black">{{ data.title }}</span>
        </div>
        <v-btn
          size="small"
          elevation="0"
          class="btn-card-link px-3 text-primary rounded-xl"
          link
          :to="`/phases/details/${data.id}`"
        >
          <div class="d-flex align-center ga-2">
            <span class="font-weight-bold">عرض</span>
            <svg
              width="1.1rem"
              height="1.1rem"
              viewBox="0 0 16 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M1.99973 7.9998C1.99973 7.63161 2.29821 7.33313 2.6664 7.33313L13.3331 7.33313C13.7013 7.33313 13.9997 7.63161 13.9997 7.9998C13.9997 8.36799 13.7013 8.66646 13.3331 8.66646L2.6664 8.66646C2.29821 8.66646 1.99973 8.36799 1.99973 7.9998Z"
                fill="#667178"
              />
              <path
                d="M3.35137 7.99988C3.41358 8.10306 3.54397 8.28847 3.68655 8.44984C3.97038 8.77106 4.36153 9.13798 4.76987 9.49251C5.17489 9.84416 5.58187 10.1708 5.88878 10.4103C6.04189 10.5298 6.3061 10.7294 6.39486 10.7964C6.69128 11.0148 6.75459 11.4321 6.53627 11.7285C6.31793 12.025 5.9006 12.0883 5.60413 11.87L5.60166 11.8681C5.5054 11.7954 5.22632 11.5846 5.06848 11.4614C4.7504 11.2132 4.32404 10.8712 3.89573 10.4993C3.47073 10.1303 3.02855 9.71881 2.68738 9.33269C2.51747 9.14039 2.35851 8.93868 2.23842 8.73948C2.12936 8.55858 1.99948 8.29535 1.99947 7.9999C1.99948 7.70446 2.12936 7.44118 2.23842 7.26028C2.35851 7.06107 2.51747 6.85936 2.68738 6.66707C3.02855 6.28095 3.47073 5.86944 3.89573 5.50044C4.32404 5.12857 4.7504 4.78656 5.06848 4.53832C5.22632 4.41515 5.50522 4.2045 5.60147 4.1318L5.60413 4.12979C5.9006 3.91145 6.31793 3.97479 6.53627 4.27125C6.75459 4.5677 6.69128 4.985 6.39486 5.20335C6.3061 5.27039 6.04189 5.46998 5.88878 5.58946C5.58187 5.82898 5.17489 6.15559 4.76987 6.50725C4.36153 6.86177 3.97038 7.2287 3.68655 7.54992C3.54397 7.71129 3.41358 7.8967 3.35137 7.99988Z"
                fill="#667178"
              />
            </svg>
          </div>
        </v-btn>
      </div>

      <VDivider class="my-5" />
      <div class="d-flex align-center justify-space-between ga-4">
        <div class="d-flex align-start ga-2">
          <svg
            width="23"
            height="24"
            viewBox="0 0 23 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              opacity="0.4"
              d="M20.6854 12.714V12.2385C20.6854 10.6372 20.6854 9.35368 20.6129 8.30957H3.16526C3.09277 9.35368 3.09277 10.6372 3.09277 12.2385V12.714C3.09277 16.7485 3.09277 18.7658 4.25214 20.0192C5.41151 21.2725 7.27749 21.2725 11.0094 21.2725H12.7687C16.5007 21.2725 18.3666 21.2725 19.526 20.0192C20.6854 18.7658 20.6854 16.7485 20.6854 12.714Z"
              fill="#667178"
            />
          </svg>

          <div class="d-flex flex-column">
            <div style="color: #75797c">تاريخ البداية</div>
            <span class="text-sm font-weight-bold">{{ data.start_date }}</span>
          </div>
        </div>

        <div class="d-flex align-start ga-2">
          <svg
            width="23"
            height="24"
            viewBox="0 0 23 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              opacity="0.4"
              d="M20.6854 12.714V12.2385C20.6854 10.6372 20.6854 9.35368 20.6129 8.30957H3.16526C3.09277 9.35368 3.09277 10.6372 3.09277 12.2385V12.714C3.09277 16.7485 3.09277 18.7658 4.25214 20.0192C5.41151 21.2725 7.27749 21.2725 11.0094 21.2725H12.7687C16.5007 21.2725 18.3666 21.2725 19.526 20.0192C20.6854 18.7658 20.6854 16.7485 20.6854 12.714Z"
              fill="#667178"
            />
          </svg>

          <div class="d-flex flex-column">
            <div style="color: #75797c">تاريخ النهاية</div>
            <span class="text-sm font-weight-bold">{{ data.end_date }}</span>
          </div>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>

<style scoped>
.btn-card-link {
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--v-primary-base);
}

.text-sm {
  font-size: 0.875rem;
}

.ga-2 {
  gap: 0.5rem;
}

.ga-4 {
  gap: 1rem;
}
</style>
