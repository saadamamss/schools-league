<script setup>
import { computed, watch, onMounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const props = defineProps({
  id: {
    type: [Number, String],
    required: true,
  },
  title: {
    type: String,
    default: "",
  },
  locations_count: {
    type: Number,
    default: 0,
  },
});

// Debug watcher for props
watch(
  () => props,
  (newProps) => {
    console.log("Contract Widget Props:", {
      id: newProps.id,
      title: newProps.title,
      created_at: newProps.created_at,
      updated_at: newProps.updated_at,
    });
  },
  { immediate: true, deep: true }
);

// Debug on mount
onMounted(() => {
  console.log("Contract Widget Mounted with props:", props);
});

const handleClick = () => {
  router.push(`/contracts/${props.id}`);
};

const formatDate = (date) => {
  if (!date) {
    console.log("No date provided");
    return "";
  }
  try {
    console.log("Formatting date:", date);
    const dateObj = new Date(date);
    // First check if it's a valid date
    if (isNaN(dateObj.getTime())) {
      console.log("Invalid date:", date);
      return date;
    }

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

    const formattedDate = formatter.format(dateObj);
    console.log("Formatted date:", formattedDate);
    return formattedDate;
  } catch (error) {
    console.error("Error formatting date:", error);
    return date;
  }
};
</script>

<template>
  <VCard
    class="rounded-lg contract-card"
    style="box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)"
  >
    <v-card-text class="pa-3 pa-sm-4">
      <div
        class="d-flex justify-space-between align-center ga-2 border-b pb-4 mb-4"
      >
        <div class="text-truncate" style="max-width: 70%">
          <span
            class="text-lg text-black font-weight-medium text-truncate d-inline-block"
            style="max-width: 100%"
            >{{ title }}</span
          >
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
              fill="#B69265"
            />
          </svg>
        </v-btn>
      </div>

      <div class="d-flex align-start ga-2">
        <svg
          width="14"
          height="16"
          viewBox="0 0 14 16"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            opacity="0.4"
            d="M1.88216 0.924465C2.20066 0.854497 2.57551 0.833008 3 0.833008L7 0.833008C7.42449 0.833008 7.79934 0.854497 8.11785 0.924465C8.44201 0.995678 8.73885 1.12375 8.97405 1.35895C9.20926 1.59416 9.33733 1.891 9.40854 2.21516C9.47851 2.53366 9.5 2.90852 9.5 3.33301L9.5 14.6663C9.5 14.9425 9.27614 15.1663 9 15.1663L1 15.1663C0.723858 15.1663 0.5 14.9425 0.5 14.6663L0.5 3.33301C0.5 2.90852 0.521489 2.53366 0.591458 2.21516C0.66267 1.891 0.790744 1.59416 1.02595 1.35895C1.26115 1.12375 1.55799 0.995678 1.88216 0.924465Z"
            fill="#B69265"
          />
          <path
            d="M3.86821 10.1946C4.08341 10.1656 4.34797 10.1657 4.63651 10.1657H5.36284C5.65139 10.1657 5.91594 10.1656 6.13115 10.1946C6.36814 10.2264 6.61862 10.3014 6.82464 10.5074C7.03065 10.7134 7.10562 10.9639 7.13748 11.2009C7.16641 11.4161 7.16638 11.6807 7.16635 11.9692L7.16635 15.1657L6.16635 15.1657L6.16635 11.999C6.16635 11.6706 6.16528 11.4746 6.1464 11.3341C6.13108 11.2202 6.11215 11.201 5.9979 11.1856C5.85742 11.1668 5.66142 11.1657 5.33301 11.1657H4.66635C4.33794 11.1657 4.14194 11.1668 4.00146 11.1856C3.8826 11.2016 3.86859 11.2179 3.85296 11.3341C3.83407 11.4746 3.83301 11.6706 3.83301 11.999L3.83301 15.1657H2.83301L2.83301 11.9692C2.83298 11.6807 2.83294 11.4161 2.86188 11.2009C2.89374 10.9639 2.96871 10.7134 3.17472 10.5074C3.38074 10.3014 3.63122 10.2264 3.86821 10.1946Z"
            fill="#B69265"
          />
          <path
            d="M9.49967 4.49902H10.833C11.2611 4.49902 11.6503 4.52048 11.9866 4.59436C12.3305 4.6699 12.6602 4.80904 12.9249 5.07379C13.1897 5.33853 13.3288 5.66822 13.4043 6.01209C13.4782 6.3484 13.4997 6.7376 13.4997 7.16569L13.4997 14.499C13.4997 14.8672 13.2012 15.1657 12.833 15.1657H8.99967C9.27582 15.1657 9.49967 14.9418 9.49967 14.6657V13.8324H12.1663V7.16569C12.1663 6.76645 12.1446 6.49198 12.1021 6.29817C12.0611 6.11191 12.0105 6.04502 11.9821 6.01659C11.9537 5.98817 11.8868 5.93756 11.7005 5.89664C11.5067 5.85407 11.2322 5.83236 10.833 5.83236H9.49967V4.49902Z"
            fill="#B69265"
          />
          <path
            d="M3.83301 3.99902C3.83301 3.72288 4.05687 3.49902 4.33301 3.49902L5.66634 3.49902C5.94248 3.49902 6.16634 3.72288 6.16634 3.99902C6.16634 4.27517 5.94248 4.49902 5.66634 4.49902L4.33301 4.49902C4.05687 4.49902 3.83301 4.27517 3.83301 3.99902Z"
            fill="#B69265"
          />
          <path
            d="M3.83301 5.99902C3.83301 5.72288 4.05687 5.49902 4.33301 5.49902L5.66634 5.49902C5.94248 5.49902 6.16634 5.72288 6.16634 5.99902C6.16634 6.27517 5.94248 6.49902 5.66634 6.49902L4.33301 6.49902C4.05687 6.49902 3.83301 6.27517 3.83301 5.99902Z"
            fill="#B69265"
          />
          <path
            d="M3.83301 7.99902C3.83301 7.72288 4.05687 7.49902 4.33301 7.49902L5.66634 7.49902C5.94248 7.49902 6.16634 7.72288 6.16634 7.99902C6.16634 8.27517 5.94248 8.49902 5.66634 8.49902H4.33301C4.05687 8.49902 3.83301 8.27517 3.83301 7.99902Z"
            fill="#B69265"
          />
        </svg>

        <div>
          <div class="text-body-2 text-medium-emphasis">عدد المواقع</div>
          <div class="text-body-2 font-weight-medium">
            {{ locations_count || 0 }}
          </div>
        </div>
      </div>

      <!-- <div class="d-flex flex-column ga-2">
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
        </div> -->
    </v-card-text>
  </VCard>
</template>

<style lang="scss" scoped>
.contract-card {
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0px 8px 48px 8px rgba(0, 0, 0, 0.08) !important;
  }
}

.btn-card-link {
  background-color: rgba(182, 146, 101, 0.1);
  color: #b69265;
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
