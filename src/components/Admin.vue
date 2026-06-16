<script setup>
import { ref, computed, onMounted, watch } from "vue";
import Widgets from "@/components/Widgets.vue";
import axiosIns from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import BarChart from "@/components/charts/BarChart.vue";
import { useTheme } from "vuetify";
const attendanceStates = ref([]);
const attendRelatedToSite = ref([]);
const absentRelatedToSite = ref([]);
const widgetsData = ref(null);
const loading = ref(false);
const error = ref(null);
const appStore = useAppStore();
const theme = useTheme();

const isDark = computed(() => theme.name.value == "dark");

watchDebounced(
  isDark,
  () => {
    isAllAttendancesLoading.value = true;
    isAllAttendancesByLocationLoading.value = true;
    isAllAbsenceByLocationLoading.value = true;
    setTimeout(() => {
      isAllAttendancesLoading.value = false;
      isAllAttendancesByLocationLoading.value = false;
      isAllAbsenceByLocationLoading.value = false;
    }, 300);
  },
  { debounce: 0 }
);

const FILTEROPTIONS = [
  { value: "month", label: "هذا الشهر" },
  { value: "week", label: "هذا الأسبوع" },
  { value: "day", label: "هذا اليوم" },
];
const attendance_period = ref("month");
const attendance_by_location_period = ref("month");
const absence_by_location_period = ref("month");

const month = (stringDate) => {
  const date = new Date(stringDate);
  return date.toLocaleString("ar-EG", { month: "long", day: "numeric" });
};
const fetchData = async () => {
  loading.value = true;
  error.value = null;
  try {
    // ======= get real data =========
    const response = await axiosIns.get("statistics", {
      params: {
        attendance_by_location_period: attendance_by_location_period.value,
        absence_by_location_period: absence_by_location_period.value,
      },
    });
    // ========= get fake data =========
    // const response = generatedData();
    if (response.data.data) {
      attendanceStates.value = response.data?.attendance_statistics;
      attendRelatedToSite.value = response.data?.attendance_by_location;
      absentRelatedToSite.value = response.data?.absence_by_location;

      //
      widgetsData.value = response.data?.data;
    }
  } catch (err) {
    console.error("Error fetching data:", err);
    appStore.showSnackbar({
      message: err.response?.data?.message || "حدث خطأ فى جلب إحصائيات الحضور",
      color: "error",
    });
  } finally {
    loading.value = false;
    isAllAttendancesLoading.value = false;
    isAllAttendancesByLocationLoading.value = false;
    isAllAbsenceByLocationLoading.value = false;
  }
};

const isAllAttendancesLoading = ref(true);
const isAllAttendancesByLocationLoading = ref(true);
const isAllAbsenceByLocationLoading = ref(true);
const fetchAttendaces = async () => {
  isAllAttendancesLoading.value = true;
  fetchData();
};
const fetchAttendacesByLocation = async () => {
  isAllAttendancesByLocationLoading.value = true;
  fetchData();
};
const fetchAbsenceByLocation = async () => {
  isAllAbsenceByLocationLoading.value = true;
  fetchData();
};
// Initial load
onMounted(() => {
  fetchData();
});

//

// Main attendance statistics
const attendStatistics = computed(() => {
  const items = attendanceStates.value;
  return {
    labels: items.map((i) => month(i.date)),
    dataset: items.map((item) => item.attendance + item.absence),
    attendance: items.map((item) => item.attendance),
    absence: items.map((item) => item.absence),
    statNames: ["الحضور", "الغياب"],
  };
});

// Attendance by site
const attendOnlyStatistics = computed(() => {
  const items = attendRelatedToSite.value;
  const attendaceCount = items.map((item) => item.attendance_count);
  return {
    labels: items.map((i) => i.location_name),
    dataset: attendaceCount,
    attendance: attendaceCount,
    absence: [],
    statNames: ["الحضور", "الغياب"],
  };
});

// Absence by site
const absenceOnlyStatistics = computed(() => {
  const items = absentRelatedToSite.value;
  const absenceCount = items.map((item) => item.absence_count);
  return {
    labels: items.map((i) => i.location_name),
    dataset: absenceCount,
    attendance: [],
    absence: absenceCount,
    statNames: ["الحضور", "الغياب"],
  };
});
</script>
<template>
  <v-container fluid class="px-0">
    <Widgets :isLoading="loading && !widgetsData" :data="widgetsData" />
    <v-row class="mx-0">
      <v-col cols="12" class="px-0 px-md-3">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>إحصائيات الحضور</h5>
              <p class="text-sm text-gray">ساعة هذا الأسبوع</p>
            </div>
            <div>
              <v-select
                item-title="label"
                item-value="value"
                hide-details
                hide-no-data
                min-width="100"
                :items="FILTEROPTIONS"
                placeholder="هذا اليوم"
                width="130"
                class="no-default-radius"
                v-model="attendance_period"
                @update:model-value="fetchAttendaces"
              />
            </div>
          </VCardTitle>
          <v-divider></v-divider>
          <VCardText class="px-0 pb-0" style="min-height: 355px">
            <BarChart
              :height="300"
              v-if="!isAllAttendancesLoading"
              title=""
              :data="attendStatistics"
              color="#297A6542"
            />

            <div v-else class="loading-spinner">
              <div class="spinner"></div>
            </div>
          </VCardText>
        </VCard>
      </v-col>
      <v-col cols="12" md="6" class="px-0 px-md-3">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>توزيع الحضور حسب الفعاليات</h5>
              <p class="text-sm text-gray">ساعة هذا الأسبوع</p>
            </div>
            <div>
              <v-select
                item-title="label"
                item-value="value"
                hide-details
                hide-no-data
                min-width="100"
                :items="FILTEROPTIONS"
                placeholder="هذا اليوم"
                width="130"
                class="no-default-radius"
                v-model="attendance_by_location_period"
                @update:model-value="fetchAttendacesByLocation"
              />
            </div>
          </VCardTitle>
          <v-divider></v-divider>
          <VCardText class="px-0 pb-0" style="min-height: 455px">
            <BarChart
              title=""
              :height="400"
              :data="attendOnlyStatistics"
              v-if="!isAllAttendancesByLocationLoading"
              color="#8abeae"
            />

            <div v-else class="loading-spinner">
              <div class="spinner"></div>
            </div>
          </VCardText>
        </VCard>
      </v-col>

      <v-col cols="12" md="6" class="px-0 px-md-3">
        <VCard class="px-5">
          <VCardTitle class="px-0 d-flex align-center justify-space-between">
            <div>
              <h5>الغيابات حسب الفعاليات</h5>
              <p class="text-sm text-gray">ساعة هذا الأسبوع</p>
            </div>
            <div>
              <v-select
                item-title="label"
                item-value="value"
                hide-details
                hide-no-data
                min-width="100"
                :items="FILTEROPTIONS"
                placeholder="هذا اليوم"
                width="130"
                class="no-default-radius"
                v-model="absence_by_location_period"
                @update:model-value="fetchAbsenceByLocation"
              />
            </div>
          </VCardTitle>
          <v-divider></v-divider>
          <VCardText class="px-0 pb-0" style="min-height: 455px">
            <BarChart
              title=""
              :height="400"
              :data="absenceOnlyStatistics"
              v-if="!isAllAbsenceByLocationLoading"
              color="#8abeae"
            />
            <div v-else class="loading-spinner">
              <div class="spinner"></div>
            </div>
          </VCardText>
        </VCard>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped lang="scss">
.loading-spinner {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 400px;
}

.spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border-left-color: #4caf50;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
