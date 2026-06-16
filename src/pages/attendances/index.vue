<script setup>
import { useAppStore } from "@/stores/app";
import Magnifier from "@/components/icons/magnifier.vue";
import DataTable from "@/components/dataTable.vue";
import Pagination from "@/@core/components/Pagination.vue";
import Eyeicon from "@/components/icons/eyeicon.vue";
import User from "@/@core/components/icons/user.vue";
import CustomVSelect from "@/@core/components/global/CustomVSelect.vue";
import AttendeWidgets from "@/components/AttendeWidgets.vue";
import { useDebounceFn, watchDebounced } from "@vueuse/core";
import Copy from "@/components/icons/copy.vue";
import Correct from "@/components/icons/correct.vue";
import UserDialogDetails from "@/components/UserDialogDetails.vue";
import axiosIns from "@/plugins/axios";
import { useDashboardStore } from "@/stores/Dashboard";
import FilterMenu from "@/@core/components/filters/filter-menu.vue";
import ExportIcon from "@/components/icons/export.vue";
import { exportData } from "@/@core/utils/helpers";
import { formatTime } from "@/@core/utils/formatters";

const dashboardStore = useDashboardStore();
const appStore = useAppStore();
const isLoading = ref(false);
const perPage = ref(5);
const totalItems = ref(0);
const currentPage = ref(1);
const search = ref("");
const details_dialog = ref(false);
const selectedUser = ref(null);
const siteNameFilter = ref(null);
const tab = ref("profile");
const STATUSOPTIONS = { absent: "غائب", attendance: "حاضر", departed: "غادر" };
const headers = [
  { title: "الاسم والدور الوظيفى", value: "user.full_name", align: "start" },
  { title: "رقم الهوية", value: "user.sa_id" },
  { title: "المدينة", value: "user.city.name.ar" },
  { title: "حالة الحضور", value: "status" },
  { title: "وقت الدخول", value: "check_in_time" },
  { title: "وقت الإنصراف", value: "check_out_time" },
  { title: "الفعالية", value: "location" },
  { title: "", value: "actions" },
];

const userTransfers = [
  {
    month: "فبراير 2025",
    entries: [
      {
        day: "12",
        type: "transfer",
        type_name: "رأس شهري",
        amount: "1,250 ",
        event: "فعالية تصفيات المنطقة الغربية لألعاب القوى",
      },
      {
        day: "12",
        type: "transfer",
        type_name: "بدل سكن",
        amount: "1,250 ",
        event: "فعالية تصفيات المنطقة الغربية لألعاب القوى",
      },
    ],
  },
  {
    month: "يناير 2025",
    entries: [
      {
        day: "12",
        type: "deduction",
        type_name: "خصم ساعات",
        hours: "3 ساعات",
        amount: "1,250 ",
        reason: "تأخر متكرر في تسجيل الدخول",
        event: "فعالية تصفيات المنطقة الغربية لألعاب القوى",
      },
      {
        day: "12",
        type: "transfer",
        type_name: "مكافأة",
        amount: "1,250 ",
        event: "فعالية تصفيات المنطقة الغربية لألعاب القوى",
      },
    ],
  },
];

const eventsData = [
  {
    title: "تصفيات المنطقة الغربية لألعاب القوى",
    duration: "من 10 مارس إلى 20 مارس 2025", // Possibly a year, though corrupted
    user: {
      work_hours: "34",
      role: "مراقب", // Role appears to be "Monitor" or "Observer"
    },
  },
  {
    title: "تصفيات المنطقة الغربية لألعاب القوى",
    duration: "من 10 مارس إلى 20 مارس 2025",
    user: {
      work_hours: "34",
      role: "مراقب",
    },
  },
  {
    title: "تصفيات المنطقة الغربية لألعاب القوى",
    duration: "من 10 مارس إلى 20 مارس 2025",
    user: {
      work_hours: "34",
      role: "مراقب",
    },
  },
  {
    title: "تصفيات المنطقة الغربية لألعاب القوى",
    duration: "من 10 مارس إلى 20 مارس 2025",
    user: {
      work_hours: "34",
      role: "مراقب",
    },
  },
];
const attendances = ref([]);
const currentFilters = ref({});
const widgetsData = ref(null);
// Watch for changes in search with debounce
watchDebounced(
  search,
  (newValue) => {
    fetchAttendances();
  },
  { debounce: 500 }
);
const searchFilter = computed(() =>
  search.value?.trim() ? { search: search.value?.trim() } : {}
);

// fetch attendances data
const fetchAttendances = async () => {
  isLoading.value = true;
  try {
    // ======= get real data =========
    const response = await axiosIns.get("attendance", {
      params: {
        per_page: perPage.value,
        page: currentPage.value,
        ...searchFilter.value,
        ...currentFilters.value,
      },
    });
    if (response.data?.data) {
      attendances.value = response.data?.data;
      perPage.value = response.data?.pagination?.i_per_page;
      totalItems.value = response.data?.pagination?.i_total_objects;
      currentPage.value = response.data?.pagination?.i_current_page;

      widgetsData.value = response.data?.statistics;
    }
  } catch (err) {
    console.error("Error fetching data:", err);
    appStore.showSnackbar({
      message: err.response?.data?.message || "حدث خطأ فى جلب بيانات الجدول ",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};
// fetch user types
// fetch user details
const isUserDetailsLoading = ref(false);
const fetchSelectedUserData = async (userId) => {
  isUserDetailsLoading.value = true;
  try {
    // ======= get real data =========
    const response = await axiosIns.get(`users/${userId}`);
    if (response.data?.data) {
      selectedUser.value = {
        ...response.data?.data,
        location: response.data?.location,
      };
    }
  } catch (err) {
    console.error("Error fetching data:", err);
    appStore.showSnackbar({
      message:
        err.response?.data?.message || "حدث خطأ فى جلب بيانات هذا المستخدم ",
      color: "error",
    });
  } finally {
    isUserDetailsLoading.value = false;
  }
};
// handle search
const handleSearch = useDebounceFn(() => {
  fetchAttendances();
}, 500);
//
const showDetails = (user) => {
  details_dialog.value = true;
  selectedUser.value = user;
  //
  fetchSelectedUserData(user.id);
};

watch(currentPage, () => {
  fetchAttendances();
});
watch(perPage, () => {
  currentPage.value = 1;
  fetchAttendances();
});

const locations = ref([]);
const fetchLocations = async (search = "") => {
  try {
    const searchVal = !search.trim() ? {} : { search: search.trim() };
    const response = await axiosIns.get("locations", {
      params: { per_page: 6, ...searchVal },
    });

    if (response.data.data) {
      locations.value = response.data.data;
    }
  } catch (error) {}
};
const users = ref([]);
const fetchUsers = async (search = "") => {
  try {
    const searchVal = !search.trim() ? {} : { search: search.trim() };
    const response = await axiosIns.get("users", {
      params: { per_page: 6, ...searchVal },
    });

    if (response.data.data) {
      users.value = response.data.data;
    }
  } catch (error) {}
};

onMounted(() => {
  Promise.all([fetchAttendances(), fetchLocations(), fetchUsers()]);
});

//
const filters = computed(() => {
  return [
    {
      type: "select",
      key: "status",
      label: "اختر حالة المستخدم",
      items: [
        { value: "attendance", label: "حاضر" },
        { value: "absent", label: "غائب" },
        { value: "departed", label: "غادر" },
      ],
      itemValue: "value",
      itemTitle: "label",
    },
    {
      type: "searchable",
      key: "user_id",
      label: "اختر مستخدم ",
      items: users.value,
      itemValue: "id",
      itemTitle: "full_name",
      searchTrigger: (value) => fetchUsers(value),
    },
    {
      type: "searchable",
      key: "location_id",
      label: "اختر الموقع ",
      items: locations.value,
      itemValue: "id",
      itemTitle: "name",
      searchTrigger: (value) => fetchLocations(value),
    },
  ];
});

const applyFilters = (filters) => {
  currentPage.value = 1;
  currentFilters.value = filters;
  fetchAttendances();
};
const clearFilters = () => {
  currentPage.value = 1;
  currentFilters.value = {};
  fetchAttendances();
};

const isExporting = ref(false);
const exportAttendanceData = async () => {
  try {
    isExporting.value = true;

    await exportData(
      "/attendance/attendance-export-excel",
      currentFilters.value
    );
  } catch (error) {
    console.error("export failed:", error);
  } finally {
    isExporting.value = false;
  }
};
</script>

<template>
  <v-container fluid class="px-0">
    <AttendeWidgets
      :is-loding="isLoading && !widgetsData"
      :data="widgetsData"
    />

    <v-container fluid class="px-0 px-md-3 py-0 mt-4">
      <!-- Add AttendeWidgets at the top -->

      <VCard class="rounded-lg">
        <v-card-title class="py-5">
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between ga-4">
            <h2 class="text-h5 font-weight-bold">قائمة سجلات الحضور</h2>

            <!-- Search -->
            <!-- Search -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-2"
              style="flex-grow: 1; min-width: 0"
            >
              <v-text-field
                v-model="search"
                placeholder="بحث باسم المستخدم / رقم الهوية  "
                rounded="lg"
                clearable
                height="48"
                class="border-grey-900 flex-grow-1"
                :style="{
                  'min-width': '200px',
                  'max-width': '300px',
                  'margin-inline-end': '12px',
                }"
                density="compact"
                hide-details
                @update:model-value="handleSearch"
              >
                <template v-slot:prepend-inner>
                  <Magnifier />
                </template>
              </v-text-field>
            </div>
            <div class="d-flex flex-wrap align-center justify-md-end ga-3">
              <!--  -->
              <FilterMenu
                :filters="filters"
                @apply-filters="applyFilters"
                @clear-filters="clearFilters"
              />

              <VBtn
                :loading="isExporting"
                :disabled="isExporting"
                variant="outlined"
                class="bg-background border border-grey-900"
                height="45"
                @click="exportAttendanceData"
              >
                <ExportIcon />
              </VBtn>
            </div>
          </div>
        </v-card-title>
        <VCardText>
          <v-row>
            <v-col cols="12">
              <DataTable
                :headers="headers"
                :items="attendances"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              >
                <template #[`item.user.full_name`]="{ item }">
                  <div class="d-flex align-center gap-2">
                    <v-avatar size="sm" v-if="item.user.profile_image">
                      <v-img :src="item.user.profile_image" />
                    </v-avatar>
                    <User v-else />
                    <div>
                      <h4>{{ item.user.full_name }}</h4>
                      <span class="text-xs">{{
                        item.user?.user_type?.name?.ar
                      }}</span>
                    </div>
                  </div>
                </template>
                <template #[`item.status`]="{ item }">
                  <v-chip
                    class="text-sm px-3"
                    :color="item?.status == 'attendance' ? 'success' : 'error'"
                    rounded="lg"
                    size="large"
                    :prepend-icon="item.status == 'attendance' ? Correct : ''"
                  >
                    <span>
                      {{ STATUSOPTIONS[item.status] }}
                    </span>
                  </v-chip>
                </template>
                <template #[`item.check_in_time`]="{ item }">
                  {{ formatTime(item.check_in_time) }}
                </template>
                <template #[`item.check_out_time`]="{ item }">
                  {{
                    item.check_out_time
                      ? formatTime(item.check_out_time)
                      : "لم يسجل"
                  }}
                </template>
                <template #[`item.location`]="{ item }">
                  <div class="d-flex ga-2 align-center" v-if="item.location">
                    <v-img
                      v-if="item.location?.image"
                      :src="item.location?.image"
                      max-width="35"
                      width="35"
                      height="35"
                    />
                    <v-img
                      v-else
                      src="/imgs/event-icon.png"
                      max-width="30"
                      width="35"
                      height="35"
                    />
                    <p class="mb-0">
                      {{ item.location?.name }}
                    </p>
                  </div>
                  <div v-else>-</div>
                </template>
                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      size="small"
                      class="text-sm rounded-lg"
                      color="surface"
                      :prepend-icon="Eyeicon"
                      @click="showDetails(item.user)"
                    >
                      عرض
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <!-- Pagination -->
              <Pagination
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>
    </v-container>

    <!-- show details dialog -->

    <UserDialogDetails
      :is-user-details-loading="isUserDetailsLoading"
      :selected-user="selectedUser"
      v-model="details_dialog"
    />
  </v-container>
</template>

<style lang="scss">
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.text-caption {
  font-size: 16px !important;
}

.text-sm {
  font-size: 14px !important;
}

.day-card {
  .day {
    width: 70px;
    height: 70px;
    background-color: #fff;

    span {
      &:nth-child(1) {
        font-size: 16px;
      }

      &:nth-child(2) {
        font-size: 12px;
        color: #999;
      }
    }
  }

  .text {
    font-size: 14px;
  }
}

.v-dialog > .v-overlay__content {
  overflow-y: auto !important;
  padding-inline-end: 0px !important;
}

.v-skeleton-loader__subtitle {
  .v-skeleton-loader__text {
    margin: 0px !important;
  }
}
</style>
