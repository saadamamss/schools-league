<script setup>
import { computed, watch, onMounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const props = defineProps({
  questionnaire: {
    type: Object,
    default: () => ({}),
  },
  id: {
    type: [Number, String],
    required: true,
  },
  title: {
    type: String,
    default: "",
  },
  contract: {
    type: Object,
    default: () => ({}),
  },
  imtithal_phase: {
    type: Object,
    default: () => ({}),
  },
  created_at: {
    type: String,
    default: "",
  },
  updated_at: {
    type: String,
    default: "",
  },
});

// Computed properties to extract nested data
const contractName = computed(() => props.contract?.name || "");
const phaseName = computed(() => props.imtithal_phase?.title || "");

// Debug watcher for props
watch(
  () => props,
  (newProps) => {
    console.log("Questionnaire Widget Props:", {
      id: newProps.id,
      title: newProps.title,
      contract: newProps.contract,
      imtithal_phase: newProps.imtithal_phase,
      created_at: newProps.created_at,
      updated_at: newProps.updated_at,
    });
  },
  { immediate: true, deep: true }
);

const handleClick = () => {
  router.push(`/questionnaires/details?id=${props.id}`);
};

const formatDate = (date) => {
  if (!date) return "";
  try {
    const dateObj = new Date(date);
    if (isNaN(dateObj.getTime())) return date;

    // Format date in Arabic with Gregorian calendar
    const formatter = new Intl.DateTimeFormat("ar", {
      calendar: "gregory",
      day: "numeric",
      month: "short",
      year: "numeric",
      hour: "numeric",
      minute: "numeric",
      hour12: true,
    });

    return formatter.format(dateObj);
  } catch (error) {
    console.error("Error formatting date:", error);
    return date;
  }
};
</script>

<template>
  <VCard
    class="rounded-lg questionnaire-card"
    style="box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)"
  >
    <v-card-text class="pa-3 pa-sm-4">
      <div class="d-flex justify-space-between align-center ga-2 flex-wrap">
        <div>
          <h2>
            <router-link
              :to="`/questionnaires/details?id=${id}`"
              class="text-body-2 text-black font-weight-bold"
              style="white-space: wrap"
            >
              {{ title }}
            </router-link>
          </h2>
        </div>
        <v-btn
          size="small"
          elevation="0"
          class="btn-card-link px-3 text-primary rounded-xl"
          @click.stop="handleClick"
        >
          <span class="font-weight-bold">عرض</span>
          <svg
            style="rotate: 180deg"
            width="1.1rem"
            height="1.1rem"
            viewBox="0 0 16 16"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M6.38 12.2867C6.28667 12.2867 6.19333 12.2533 6.12 12.18C5.97333 12.0333 5.97333 11.7867 6.12 11.64L9.75333 8.00668L6.12 4.37334C5.97333 4.22668 5.97333 3.98001 6.12 3.83334C6.26667 3.68668 6.51333 3.68668 6.66 3.83334L10.6133 7.78668C10.76 7.93334 10.76 8.18001 10.6133 8.32668L6.66 12.28C6.56667 12.2533 6.47333 12.2867 6.38 12.2867Z"
              fill="#667178"
            />
          </svg>
        </v-btn>
      </div>

      <div class="d-flex flex-column mt-3">
        <div class="d-flex align-center mb-2">
          <svg
            width="1rem"
            height="1rem"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            class="me-2"
          >
            <path
              d="M8 2V5"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M16 2V5"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M3.5 9.09H20.5"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M21 8.5V17C21 20 19.5 22 16 22H8C4.5 22 3 20 3 17V8.5C3 5.5 4.5 3.5 8 3.5H16C19.5 3.5 21 5.5 21 8.5Z"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M15.6947 13.7H15.7037"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M15.6947 16.7H15.7037"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M11.9955 13.7H12.0045"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M11.9955 16.7H12.0045"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M8.29431 13.7H8.30329"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M8.29431 16.7H8.30329"
              stroke="#667178"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
          <span class="text-body-2 text-grey-darken-1">{{
            phaseName || "-"
          }}</span>
        </div>
        <div class="d-flex align-center mb-2">
          <svg
            width="1rem"
            height="1rem"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            class="me-2"
          >
            <path
              d="M21 7V17C21 20 19.5 22 16 22H8C4.5 22 3 20 3 17V7C3 4 4.5 2 8 2H16C19.5 2 21 4 21 7Z"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M14.5 4.5V6.5C14.5 7.6 15.4 8.5 16.5 8.5H18.5"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M8 13H12"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <path
              d="M8 17H16"
              stroke="#667178"
              stroke-width="1.5"
              stroke-miterlimit="10"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
          <span class="text-body-2 text-grey-darken-1">{{
            contractName || "-"
          }}</span>
        </div>

        <!-- <div class="d-flex align-center mb-2">
          <svg
            width="1rem"
            height="1rem"
            viewBox="0 0 32 33"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            class="me-2"
          >
            <g opacity="0.4">
              <path
                d="M10.668 26.1686C11.2203 26.1686 11.668 26.6164 11.668 27.1686V28.9687C11.668 29.2201 11.668 29.3458 11.7461 29.4239C11.8242 29.502 11.9499 29.502 12.2013 29.502H14.468C14.7194 29.502 14.8451 29.502 14.9232 29.4239C15.0013 29.3458 15.0013 29.2201 15.0013 28.9687V27.1686C15.0013 26.6164 15.449 26.1686 16.0013 26.1686C16.5536 26.1686 17.0013 26.6164 17.0013 27.1686V28.9687C17.0013 29.2201 17.0013 29.3458 17.0794 29.4239C17.1575 29.502 17.2832 29.502 17.5346 29.502L23.3601 29.502C23.7181 29.502 23.897 29.502 23.9763 29.3845C24.0556 29.267 23.9886 29.101 23.8546 28.769L21.1807 22.1445C21.1245 22.0052 21.0964 21.9356 21.0396 21.8915C20.9829 21.8474 20.9093 21.8375 20.762 21.8176C19.2331 21.6113 17.6399 21.502 16.0015 21.502C11.138 21.502 6.67398 22.465 3.10692 24.0678L1.97677 24.6635C1.83322 24.7392 1.76144 24.777 1.72294 24.844C1.68444 24.9111 1.68786 24.9907 1.69472 25.1501C1.71028 25.5115 1.73505 25.8436 1.77446 26.1462C1.8858 27.001 2.12969 27.7705 2.73126 28.3915C3.33714 29.017 4.09529 29.2747 4.93826 29.3917C5.7334 29.5021 6.73422 29.502 7.93059 29.502L9.13465 29.502C9.38606 29.502 9.51177 29.502 9.58988 29.4239C9.66798 29.3458 9.66798 29.2201 9.66798 28.9687V27.1686C9.66798 26.6164 10.1157 26.1686 10.668 26.1686Z"
                fill="#667178"
              />
              <path
                d="M14.2755 11.0622C14.9204 10.9961 15.6545 10.9209 16.3273 11.3379C16.709 11.5744 16.9499 11.8928 17.1323 12.2238C17.2934 12.5162 17.4401 12.8797 17.5939 13.2609L19.8722 18.9052C20.021 19.2738 20.0954 19.4581 20.0065 19.5781C19.9177 19.6981 19.7175 19.6806 19.3172 19.6456C18.2332 19.5509 17.1258 19.502 16.0005 19.502C15.6284 19.502 15.2582 19.5073 14.8902 19.5179C14.627 19.5255 14.4953 19.5293 14.4145 19.4507C14.3337 19.3722 14.3337 19.2418 14.3337 18.981V17.8353C14.3337 17.283 13.8859 16.8353 13.3337 16.8353C12.7814 16.8353 12.3337 17.283 12.3337 17.8353V19.1938C12.3337 19.4231 12.3337 19.5378 12.2656 19.6137C12.1976 19.6897 12.0824 19.7024 11.8521 19.7278C11.1004 19.8106 10.361 19.9156 9.63584 20.0416C9.33884 20.0933 9.19034 20.1191 9.09533 20.0392C9.00033 19.9592 9.00033 19.8102 9.00033 19.5121V18.502C9.00033 17.9497 8.55261 17.502 8.00033 17.502C7.44804 17.502 7.00033 17.9497 7.00033 18.502L7.00033 20.1941C7.00033 20.3921 7.00033 20.4911 6.94546 20.5628C6.89059 20.6345 6.79391 20.6607 6.60056 20.7132C5.05753 21.1315 3.60185 21.6503 2.25612 22.257L2.22807 22.2697C2.07931 22.3481 2.00493 22.3873 1.94451 22.3897C1.83812 22.3938 1.73944 22.3343 1.69351 22.2382C1.66742 22.1837 1.66742 22.0996 1.66741 21.9314L1.66723 18.5402C1.66577 18.0732 1.66417 17.5642 1.88552 17.0888C2.10653 16.6141 2.47867 16.303 2.8164 16.0206C5.80268 13.509 9.38432 11.7564 13.1875 11.1928C13.4992 11.1466 13.9602 11.0931 14.2755 11.0622Z"
                fill="#667178"
              />
            </g>
            <path
              d="M23.229 22.3824C23.1231 22.5056 23.2053 22.7095 23.3699 23.1172L25.7975 29.1314C25.8657 29.3004 25.8999 29.3849 25.9725 29.4308C26.0451 29.4768 26.1341 29.4714 26.3119 29.4608C26.5709 29.4453 26.8133 29.4237 27.0391 29.3936C27.8804 29.2814 28.6395 29.0347 29.2502 28.4288C29.862 27.8219 30.112 27.0655 30.2256 26.227C30.2728 25.879 30.2992 25.4913 30.3141 25.0647C30.32 24.896 30.3229 24.8116 30.2802 24.7428C30.2374 24.6739 30.1594 24.6385 30.0032 24.5678L28.9224 24.0783C27.4188 23.4004 25.755 22.8364 23.9686 22.4104C23.5462 22.3097 23.335 22.2593 23.229 22.3824Z"
              fill="#667178"
            />
            <path
              d="M22.1338 19.9652C22.1875 20.0086 22.2601 20.0213 22.4052 20.0467C25.0775 20.5145 27.5563 21.2685 29.7444 22.255L29.7459 22.2557C29.9326 22.3403 30.026 22.3826 30.1016 22.3725C30.1805 22.362 30.2506 22.3168 30.2927 22.2493C30.3331 22.1845 30.3331 22.082 30.3331 21.8771L30.3332 20.1279C30.3344 19.0825 30.3353 18.2414 29.926 17.4487C29.5179 16.6583 28.9044 16.2209 28.1499 15.6831L28.0428 15.6067C26.4655 14.4801 24.7626 13.5643 22.9621 12.8959C22.6551 12.7819 22.3119 12.8254 22.0431 13.0124C21.7743 13.1993 21.614 13.5059 21.614 13.8333V14.9687C21.614 15.2181 21.614 15.3429 21.537 15.4208L21.5348 15.423C21.4569 15.5 21.3322 15.5 21.0827 15.5C20.7276 15.5 20.55 15.5 20.4706 15.6159L20.4684 15.6192C20.3906 15.7362 20.4571 15.9009 20.59 16.2302L21.9983 19.719C22.0528 19.8541 22.0801 19.9217 22.1338 19.9652Z"
              fill="#667178"
            />
            <path
              d="M12.7895 8.07345L13.1008 8.86285C13.2137 9.1492 13.2702 9.29238 13.209 9.4025C13.1478 9.51263 12.9935 9.54087 12.6849 9.59736C9.12021 10.2498 5.4639 11.9055 2.56292 14.0946C2.16276 14.3965 1.96268 14.5475 1.81492 14.4739C1.66716 14.4003 1.66716 14.1544 1.66715 13.6626L1.6671 10.4945C1.66605 9.41577 1.66522 8.56787 2.06105 7.76583C2.45879 6.9599 3.05972 6.50991 3.80657 5.95066C4.14719 5.69513 4.66762 5.32572 5.01992 5.09128C5.82978 4.55221 6.55425 4.06997 7.23148 3.79063C8.00133 3.47309 8.76448 3.3905 9.62185 3.66992C10.5169 3.96162 11.0997 4.55529 11.5605 5.32348C11.9831 6.02798 12.3544 6.96965 12.7895 8.07345Z"
              fill="#667178"
            />
          </svg>

          <span class="text-body-2 text-grey-darken-1">{{
            questionnaire?.company?.name || "-"
          }}</span>
        </div> -->
      </div>

      <!-- <div class="d-flex align-center justify-space-between mt-4">
        <div class="d-flex flex-column ga-2">
          <span class="text-caption text-medium-emphasis">تاريخ الإنشاء</span>
          <span class="text-body-2 text-grey-darken-1" dir="rtl">
            {{ created_at ? formatDate(created_at) : "No date" }}
          </span>
        </div>
        <div class="d-flex flex-column ga-2">
          <span class="text-caption text-medium-emphasis">آخر تحديث</span>
          <span class="text-body-2 text-grey-darken-1" dir="rtl">
            {{ updated_at ? formatDate(updated_at) : "No date" }}
          </span>
        </div>
      </div> -->
    </v-card-text>
  </VCard>
</template>

<style lang="scss" scoped>
.questionnaire-card {
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0px 8px 48px 8px rgba(0, 0, 0, 0.08) !important;
  }
}

.btn-card-link {
  background-color: rgba(182, 146, 101, 0.1);
  color: #667178;
  text-decoration: none;
  z-index: 1;

  &:hover {
    background-color: rgba(182, 146, 101, 0.2);
  }
}

.ga-2 {
  gap: 0.5rem;
}

.text-grey-darken-1 {
  color: rgba(0, 0, 0, 0.7);
}
</style>
