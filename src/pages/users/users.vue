<script setup>
import { useAppStore } from "@/stores/app";
import Magnifier from "@/components/icons/magnifier.vue";
import DataTable from "@/components/dataTable.vue";
import Pagination from "@/@core/components/Pagination.vue";
import Eyeicon from "@/components/icons/eyeicon.vue";
import User from "@/@core/components/icons/user.vue";
import UsersWidgets from "@/components/UsersWidgets.vue";
import axiosIns from "@/plugins/axios";
import { watch } from "vue";
import { useDebounceFn, watchDebounced } from "@vueuse/core";
import { computed } from "vue";
import FilterMenu from "@/@core/components/filters/filter-menu.vue";
import { useDashboardStore } from "@/stores/Dashboard";
import { exportData } from "@/@core/utils/helpers";
import ExportIcon from "@/components/icons/export.vue";
import UserDialogDetails from "@/components/UserDialogDetails.vue";

// import UserWidget from "@/@core/components/widgets/UserWidget.vue";
const dashboardStore = useDashboardStore();
const appStore = useAppStore();
const isLoading = ref(false);
const perPage = ref(5);
const totalItems = ref(0);
const currentPage = ref(1);
const search = ref("");
const details_dialog = ref(false);
const selectedUser = ref(null);
const widgetsData = ref(null);
const headers = [
  { title: "الاسم", value: "full_name", align: "start" },
  { title: "رقم الهوية", value: "sa_id" },
  { title: "دور المستخدم", value: "role" },
  { title: "المدينة", value: "city.name.ar" },
  { title: "رقم الجوال", value: "phone" },
  // { title: "الفعاليات", value: "events" },
  { title: "أيام العمل", value: "working_days_count" },
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

const users = ref([]);
const currentFilters = ref({});
// Watch for changes in search with debounce
watchDebounced(
  search,
  (newValue) => {
    fetchUsers();
  },
  { debounce: 500 }
);
const searchFilter = computed(() =>
  search.value?.trim() ? { search: search.value?.trim() } : {}
);
// fetch users data
const fetchUsers = async () => {
  isLoading.value = true;
  try {
    // ======= get real data =========
    const response = await axiosIns.get("users", {
      params: {
        per_page: perPage.value,
        page: currentPage.value,
        ...searchFilter.value,
        ...currentFilters.value,
      },
    });
    if (response.data?.data) {
      users.value = response.data?.data;
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
  fetchUsers();
}, 500);
//
const showDetails = (user) => {
  details_dialog.value = true;
  selectedUser.value = user;
  //
  fetchSelectedUserData(user.id);
};

watch(currentPage, () => {
  fetchUsers();
});
watch(perPage, () => {
  currentPage.value = 1;
  fetchUsers();
});

onMounted(() => {
  Promise.all([
    fetchUsers(),
    dashboardStore.fetchCities(),
    dashboardStore.fetchUserTypes(),
  ]);
});

//
const filters = computed(() => {
  return [
    {
      type: "select",
      key: "gender",
      label: "اختر جنس المستخدم",
      items: [
        { label: "ذكر", value: "male" },
        { label: "أنثى", value: "female" },
      ],
      itemValue: "value",
      itemTitle: "label",
    },
    {
      type: "select",
      key: "user_type_id",
      label: "اختر دور المستخدم",
      items: dashboardStore.userTypes,
      itemValue: "id",
      itemTitle: "name.ar",
    },
    {
      type: "select",
      key: "city_id",
      label: "اختر المدينة ",
      items: dashboardStore.city,
      itemValue: "id",
      itemTitle: "name.ar",
      searchTrigger: (value) => dashboardStore.fetchCities(value),
    },
    {
      type: "date",
      key: "created_at",
      label: "تاريخ الإنشاء",
    },
  ];
});

const applyFilters = (filters) => {
  currentPage.value = 1;
  currentFilters.value = filters;
  fetchUsers();
};
const clearFilters = () => {
  currentPage.value = 1;
  currentFilters.value = {};
  fetchUsers();
};

const isExporting = ref({
  rating: false,
});
const exportUserData = async () => {
  try {
    isExporting.value.rating = true;

    await exportData("/users/users-export-excel", currentFilters.value);
  } catch (error) {
    console.error("export failed:", error);
  } finally {
    isExporting.value.rating = false;
  }
};
</script>

<template>
    <v-container fluid class="px-0">
      <!-- Add UsersWidgets component -->
      <UsersWidgets
        :is-loding="isLoading && !widgetsData"
        :data="widgetsData"
        class="mb-4"
      />

      <div class="px-md-3">
        <VCard class="rounded-lg">
          <VCardTitle class="py-5">
            <!-- Header Section -->
            <div
              class="d-flex flex-wrap align-center justify-space-between ga-4"
            >
              <h2 class="text-h5 font-weight-bold">قائمة المستخدمين</h2>

              <!-- Search -->
              <div
                class="d-flex flex-wrap align-center justify-md-end ga-3"
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

                <!--  -->
                <FilterMenu
                  :filters="filters"
                  @apply-filters="applyFilters"
                  @clear-filters="clearFilters"
                />

                <VBtn
                  :loading="isExporting.rating"
                  :disabled="isExporting.rating"
                  variant="outlined"
                  height="45"
                  class="bg-background border border-grey-900"
                  @click="exportUserData"
                >
                  <ExportIcon />
                </VBtn>
              </div>
            </div>
          </VCardTitle>
          <VCardText>
            <v-row>
              <v-col cols="12">
                <DataTable
                  :headers="headers"
                  :items="users"
                  :loading="isLoading"
                  class="elevation-0"
                  :items-per-page="perPage"
                  v-model:page="currentPage"
                  :hide-default-footer="true"
                >
                  <template #[`item.full_name`]="{ item }">
                    <div class="d-flex align-center gap-2">
                      <v-avatar size="sm" v-if="item.profile_image"></v-avatar>
                      <User v-else />
                      <span>{{ item.full_name }}</span>
                    </div>
                  </template>
                  <template #[`item.role`]="{ item }">
                    <v-chip
                      color="success"
                      class="py-2 px-3"
                      rounded="lg"
                      size="lg"
                    >
                      {{ item.user_type?.name?.ar }}
                    </v-chip>
                  </template>
                  <template #[`item.working_days_count`]="{ item }">
                    {{
                      item.working_days_count
                        ? `${item.working_days_count} يوم`
                        : ""
                    }}
                  </template>
                  <template #[`item.actions`]="{ item }">
                    <div class="d-flex gap-2">
                      <v-btn
                        size="small"
                        class="text-sm rounded-lg"
                        color="surface"
                        :prepend-icon="Eyeicon"
                        @click="showDetails(item)"
                      >
                        عرض
                      </v-btn>
                    </div>
                  </template>
                </DataTable>
              </v-col>
              <v-col cols="12" v-if="users.length">
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
      </div>
      <!-- show details dialog -->
      <UserDialogDetails
        :is-user-details-loading="isUserDetailsLoading"
        :selected-user="{ ...selectedUser, eventsData, userTransfers }"
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
.text-xs {
  font-size: 12px !important;
}

.font-bold {
  font-weight: 600;
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
