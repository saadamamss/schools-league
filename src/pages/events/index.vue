<script setup>
import { useAppStore } from "@/stores/app";
import Magnifier from "@/components/icons/magnifier.vue";
import DataTable from "@/components/dataTable.vue";
import Pagination from "@/@core/components/Pagination.vue";
import Eyeicon from "@/components/icons/eyeicon.vue";
import EventsWidgets from "@/components/SitesWidgets.vue";
import { watchDebounced } from "@vueuse/core";
import { useDashboardStore } from "@/stores/Dashboard";
import axiosIns from "@/plugins/axios";
import ExportIcon from "@/components/icons/export.vue";
import FilterMenu from "@/@core/components/filters/filter-menu.vue";
import { exportData } from "@/@core/utils/helpers";
import MapDisplay from "@/components/maps/MapDisplay.vue";
import User from "@/@core/components/icons/user.vue";
import { formatTime, formatTimeTo12Hour } from "@/@core/utils/formatters";

const dashboardStore = useDashboardStore();
const appStore = useAppStore();
const isLoading = ref(false);
const perPage = ref(5);
const totalItems = ref(0);
const currentPage = ref(1);
const search = ref("");
const details_dialog = ref(false);
const selectedEvent = ref(null);
const selectedEventSupervisor = ref(null);
const selectedEventInspectors = ref(null);

const tab = ref("profile");
const headers = [
  { title: "اسم الفعالية", value: "name", align: "start" },
  //{ title: "نوع الفعالية", value: "event_type" },
  { title: "المدينة", value: "city.name.ar" },
  { title: "عدد الموظفين المرتبطين", value: "current_events_count" },
  // { title: "ساعات العمل / الشهر", value: "total_working_hours" },
  { title: "", value: "actions" },
];

const events = ref([]);
const widgetsData = ref(null);
const currentFilters = ref({});
// Watch for changes in search with debounce
watchDebounced(
  search,
  (newValue) => {
    fetchEvents();
  },
  { debounce: 500 }
);
const searchFilter = computed(() =>
  search.value?.trim() ? { search: search.value?.trim() } : {}
);
// fetch events data
const fetchEvents = async () => {
  isLoading.value = true;
  try {
    // ======= get real data =========
    const response = await axiosIns.get("locations", {
      params: {
        per_page: perPage.value,
        page: currentPage.value,
        ...searchFilter.value,
        ...currentFilters.value,
      },
    });
    if (response.data?.data) {
      events.value = response.data?.data;
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
// fetch event types
// fetch event details
const iseventDetailsLoading = ref(false);
const fetchselectedEventData = async (eventId) => {
  iseventDetailsLoading.value = true;
  try {
    // ======= get real data =========
    const response = await axiosIns.get(`locations/${eventId}`);
    if (response.data?.data) {
      selectedEvent.value = response.data?.data;
      selectedEventSupervisor.value = response.data?.supervisor;
      selectedEventInspectors.value = response.data?.inspectors;
    }
  } catch (err) {
    console.error("Error fetching data:", err);
    appStore.showSnackbar({
      message:
        err.response?.data?.message || "حدث خطأ فى جلب بيانات هذا المستخدم ",
      color: "error",
    });
  } finally {
    iseventDetailsLoading.value = false;
  }
};
// handle search
const handleSearch = useDebounceFn(() => {
  fetchEvents();
}, 500);
//
const showDetails = (event) => {
  details_dialog.value = true;
  selectedEvent.value = event;
  //
  fetchselectedEventData(event.id);
};

watch(currentPage, () => {
  fetchEvents();
});
watch(perPage, () => {
  currentPage.value = 1;
  fetchEvents();
});

onMounted(() => {
  Promise.all([fetchEvents(), dashboardStore.fetchCities()]);
});

//
const filters = computed(() => {
  return [
    {
      type: "select",
      key: "city_id",
      label: "اختر المدينة ",
      items: dashboardStore.city,
      itemValue: "id",
      itemTitle: "name.ar",
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
  fetchEvents();
};
const clearFilters = () => {
  currentPage.value = 1;
  currentFilters.value = {};
  fetchEvents();
};

const isExporting = ref(false);
const exportEventData = async () => {
  try {
    isExporting.value = true;

    await exportData("locations/locations-export-excel", currentFilters.value);
  } catch (error) {
    console.error("export failed:", error);
  } finally {
    isExporting.value = false;
  }
};

const openOnMap = () => {
  window.open(
    `https://maps.google.com/maps?q=${selectedEvent.value?.latitude},${selectedEvent.value?.longitude}`,
    "_blank"
  );
};
</script>

<template>
<v-container fluid class="px-0">
    <!-- Add eventsWidgets component -->
    <EventsWidgets :is-loding="isLoading && !widgetsData" :data="widgetsData" />

    <v-container fluid class="px-0 px-md-3 py-0 mt-4">
      <VCard class="rounded-lg">
        <VCardTitle class="py-4">
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between ga-4">
            <h2 class="text-h5 font-weight-bold">قائمة الفعاليات</h2>

            <!-- Search -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-3"
              style="flex-grow: 1; min-width: 0"
            >
              <v-text-field
                v-model="search"
                placeholder="بحث عن موقع"
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
                :loading="isExporting"
                :disabled="isExporting"
                variant="outlined"
                class="bg-background border border-grey-900"
                height="45"
                @click="exportEventData"
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
                :items="events"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              >
                <template #[`item.current_events_count`]="{ item }">
                  <span> {{ item.current_events_count || 0 }} موظف </span>
                </template>
                <!--  {{ item.observer_number || 0 }} مراقب -->

                <template #[`item.total_working_hours`]="{ item }">
                  {{ item.total_working_hours }} ساعة
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
    <v-dialog
      v-model="details_dialog"
      max-width="650"
      min-height="calc(100% - 40px)"
    >
      <v-card rounded="lg" class="d-block">
        <!-- Header -->
        <v-card-title
          class="bg-light-gray border-b-md border-grey-900 rounded-t-lg py-4"
        >
          <div
            class="w-100 d-flex justify-end"
            style="position: absolute; left: 10px; top: 10px"
          >
            <button @click="details_dialog = false">
              <v-icon icon="mdi-close" color="primary"> </v-icon>
            </button>
          </div>
          <div class="text-center">
            <v-avatar class="rounded-pill" color="light-success" :size="80">
              <v-img
                v-if="selectedEvent?.image"
                :src="selectedEvent?.image"
                width="32"
                height="32"
              />
              <svg
                v-else
                width="31"
                height="34"
                viewBox="0 0 31 34"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M3.57271 1.13391C4.30771 0.972441 5.17276 0.922852 6.15235 0.922852H15.3831C16.3627 0.922852 17.2278 0.972442 17.9628 1.13391C18.7108 1.29824 19.3959 1.5938 19.9386 2.13657C20.4814 2.67935 20.777 3.36437 20.9413 4.11244C21.1028 4.84744 21.1523 5.7125 21.1523 6.69208V32.8459C21.1523 33.4832 20.6358 33.9998 19.9985 33.9998H1.53696C0.899712 33.9998 0.383118 33.4832 0.383118 32.8459V6.69208C0.383118 5.71249 0.432708 4.84744 0.594174 4.11244C0.75851 3.36437 1.05407 2.67935 1.59684 2.13657C2.13962 1.5938 2.82464 1.29824 3.57271 1.13391Z"
                  fill="rgba(var(--v-theme-primary))"
                />
                <path
                  d="M8.15662 22.5284C8.65326 22.4616 9.26377 22.4617 9.92964 22.4618H11.6058C12.2717 22.4617 12.8822 22.4616 13.3788 22.5284C13.9257 22.6019 14.5037 22.7749 14.9792 23.2503C15.4546 23.7258 15.6276 24.3038 15.7011 24.8507C15.7679 25.3473 15.7678 25.9579 15.7677 26.6237L15.7677 34.0002H13.46V26.6925C13.46 25.9347 13.4576 25.4824 13.414 25.1582C13.3786 24.8952 13.3349 24.8509 13.0713 24.8155C12.7471 24.7719 12.2948 24.7695 11.5369 24.7695H9.99848C9.24062 24.7695 8.78831 24.7719 8.46412 24.8155C8.18984 24.8524 8.1575 24.89 8.12144 25.1582C8.07785 25.4824 8.0754 25.9347 8.0754 26.6925V34.0002H5.76771L5.76771 26.6237C5.76763 25.9578 5.76755 25.3473 5.83433 24.8507C5.90785 24.3038 6.08085 23.7258 6.55627 23.2503C7.03169 22.7749 7.60973 22.6019 8.15662 22.5284Z"
                  fill="rgba(var(--v-theme-primary))"
                />
                <path
                  d="M21.1523 9.38484H24.2292C25.2171 9.38484 26.1153 9.43436 26.8914 9.60485C27.6849 9.77918 28.4458 10.1003 29.0567 10.7112C29.6677 11.3222 29.9887 12.083 30.1631 12.8765C30.3336 13.6526 30.3831 14.5508 30.3831 15.5387V32.4618C30.3831 33.3114 29.6943 34.0002 28.8446 34.0002H19.9985C20.6357 34.0002 21.1523 33.4836 21.1523 32.8464V30.9233H27.3062V15.5387C27.3062 14.6174 27.2561 13.984 27.1578 13.5367C27.0634 13.1069 26.9466 12.9525 26.881 12.8869C26.8154 12.8213 26.661 12.7045 26.2312 12.6101C25.784 12.5119 25.1506 12.4618 24.2292 12.4618H21.1523V9.38484Z"
                  fill="rgba(var(--v-theme-primary))"
                />
                <path
                  d="M8.07539 8.23099C8.07539 7.59374 8.59199 7.07715 9.22924 7.07715H12.3062C12.9434 7.07715 13.46 7.59374 13.46 8.23099C13.46 8.86825 12.9434 9.38484 12.3062 9.38484H9.22924C8.59199 9.38484 8.07539 8.86825 8.07539 8.23099Z"
                  fill="rgba(var(--v-theme-primary))"
                />
                <path
                  d="M8.07539 12.8464C8.07539 12.2091 8.59199 11.6925 9.22924 11.6925H12.3062C12.9434 11.6925 13.46 12.2091 13.46 12.8464C13.46 13.4836 12.9434 14.0002 12.3062 14.0002H9.22924C8.59199 14.0002 8.07539 13.4836 8.07539 12.8464Z"
                  fill="rgba(var(--v-theme-primary))"
                />
                <path
                  d="M8.07539 17.4618C8.07539 16.8245 8.59199 16.3079 9.22924 16.3079H12.3062C12.9434 16.3079 13.46 16.8245 13.46 17.4618C13.46 18.099 12.9434 18.6156 12.3062 18.6156H9.22924C8.59199 18.6156 8.07539 18.099 8.07539 17.4618Z"
                  fill="rgba(var(--v-theme-primary))"
                />
              </svg>
            </v-avatar>

            <h5 class="py-2">{{ selectedEvent?.name }}</h5>
            <p class="text-sm text-sec-text mb-0">
              {{ selectedEvent?.address }}
            </p>
          </div>
        </v-card-title>

        <!-- Present/Absent Status -->
        <v-card-text class="pt-4 d-flex justify-center px-3">
          <!-- Attendance Section -->
          <v-tabs v-model="tab" class="rounded-lg" hide-slider>
            <v-tab
              value="event-details"
              class="rounded-lg mx-1 bg-light-gray"
              :class="{
                'bg-light-success text-onSuccess': tab == 'event-details',
              }"
            >
              تفاصيل عن الفعالية
            </v-tab>
            <v-tab
              value="observers"
              class="rounded-lg mx-1 bg-light-gray"
              :class="{ 'bg-light-success text-onSuccess': tab == 'observers' }"
            >
              المشرفون
            </v-tab>

            <v-tab
              value="employees"
              class="rounded-lg mx-1 bg-light-gray"
              :class="{ 'bg-light-success text-onSuccess': tab == 'employees' }"
            >
              الموظفون
            </v-tab>
          </v-tabs>
        </v-card-text>
        <v-card-text class="px-0">
          <v-window v-model="tab">
            <!-- Profile Tab -->
            <v-window-item value="event-details" class="px-3 px-lg-6 py-3">
              <!-- Basic Info -->
              <VCard class="mb-4 px-0 border">
                <VCardTitle class="text-primary-text mb-1">
                  <h5>تفاصيل عن الفعالية</h5>
                </VCardTitle>
                <v-divider></v-divider>
                <VCardText class="px-3 px-lg-6">
                  <v-row class="mx-0 gap-2 px-0">
                    <v-col cols="12" class="px-0 d-flex gap-2">
                      <span>
                        <svg
                          width="24"
                          height="26"
                          viewBox="0 0 24 26"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            opacity="0.4"
                            d="M1.99616 8.89718C3.52407 5.32553 7.1744 3.0625 11.0006 3.0625C14.8267 3.0625 18.4771 5.32553 20.005 8.89718C20.4256 9.88053 20.6544 10.821 20.7264 11.7208C20.7492 12.0049 20.7605 12.1469 20.6695 12.2129C20.5785 12.2788 20.4359 12.2189 20.1508 12.0993C19.3169 11.7493 18.4146 11.5625 17.502 11.5625C16.7817 11.5625 16.0678 11.6789 15.3889 11.9C15.1732 11.9703 15.0653 12.0054 14.9913 11.9675C14.9173 11.9296 14.8815 11.8177 14.81 11.5939C14.2946 9.98068 12.7832 8.8125 10.999 8.8125C8.78988 8.8125 6.99902 10.6034 6.99902 12.8125C6.99902 14.8205 8.47867 16.483 10.4071 16.769C10.6366 16.8031 10.7514 16.8201 10.8007 16.8858C10.85 16.9515 10.8348 17.0634 10.8043 17.2873C10.5741 18.976 11.1162 20.4623 11.9153 21.6672C12.276 22.2111 12.7049 22.7214 13.1447 23.1892C13.26 23.312 13.3177 23.3733 13.323 23.4438C13.3241 23.4583 13.3237 23.4721 13.3217 23.4864C13.3119 23.5564 13.2512 23.6133 13.1298 23.7269C12.5548 24.2653 11.7909 24.5625 11.0006 24.5625C10.2102 24.5625 9.44636 24.2653 8.87134 23.727L8.86968 23.7254C8.55943 23.4331 8.2369 23.1349 7.90757 22.8303L7.90645 22.8293C6.17737 21.2303 4.2608 19.4579 2.94674 17.472C1.34399 15.0498 0.574203 12.2211 1.99616 8.89718Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                          <path
                            fill-rule="evenodd"
                            clip-rule="evenodd"
                            d="M17.5011 13.0625C15.4495 13.0625 13.4828 14.2717 12.6556 16.2009C11.8819 18.0055 12.3099 19.5497 13.1645 20.8382C13.8536 21.8774 14.8591 22.8045 15.7352 23.6124L15.7353 23.6125C15.9005 23.7649 16.0611 23.913 16.2142 24.0568L16.2157 24.0582C16.565 24.3845 17.0263 24.5625 17.5011 24.5625C17.9758 24.5625 18.4371 24.3845 18.7865 24.0582C18.9316 23.9226 19.0833 23.7832 19.2392 23.64C20.1242 22.8269 21.143 21.8909 21.8393 20.8386C22.6928 19.5489 23.1193 18.0033 22.3466 16.2009C21.5194 14.2717 19.5526 13.0625 17.5011 13.0625ZM17.492 16.3125C16.3924 16.3125 15.501 17.2079 15.501 18.3125C15.501 19.4171 16.3924 20.3125 17.492 20.3125H17.5099C18.6095 20.3125 19.501 19.4171 19.501 18.3125C19.501 17.2079 18.6095 16.3125 17.5099 16.3125H17.492Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                        </svg>
                      </span>
                      <div class="w-100">
                        <div
                          class="position-relative text-sec-text mb-2 d-flex align-center justify-space-between"
                        >
                          <span>العنوان التفصيلي</span>

                          <VBtn
                            class="font-bold show-on-map"
                            variant="flat"
                            rounded="pill"
                            color="light-success text-primary"
                            size="small"
                            density="compact"
                            max-height="26"
                            @click="openOnMap"
                          >
                            عرض على الخريطة
                            <svg
                              width="16"
                              height="17"
                              viewBox="0 0 16 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M1.99976 8.31266C1.99976 7.94447 2.29823 7.646 2.66642 7.646L13.3331 7.646C13.7013 7.646 13.9998 7.94447 13.9998 8.31266C13.9998 8.68085 13.7013 8.97933 13.3331 8.97933L2.66642 8.97933C2.29823 8.97933 1.99976 8.68085 1.99976 8.31266Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M3.35136 8.3125C3.41356 8.41568 3.54395 8.60109 3.68653 8.76246C3.97036 9.08368 4.36152 9.4506 4.76985 9.80513C5.17487 10.1568 5.58185 10.4834 5.88877 10.7229C6.04187 10.8424 6.30608 11.042 6.39484 11.109C6.69126 11.3274 6.75458 11.7447 6.53625 12.0411C6.31791 12.3376 5.90058 12.4009 5.60411 12.1826L5.60164 12.1807C5.50538 12.108 5.2263 11.8972 5.06847 11.7741C4.75038 11.5258 4.32402 11.1838 3.89571 10.8119C3.47071 10.4429 3.02853 10.0314 2.68736 9.64531C2.51745 9.45301 2.35849 9.25131 2.2384 9.0521C2.12935 8.8712 1.99946 8.60797 1.99945 8.31252C1.99946 8.01708 2.12935 7.7538 2.2384 7.5729C2.35849 7.37369 2.51745 7.17199 2.68736 6.97969C3.02853 6.59357 3.47071 6.18206 3.89571 5.81306C4.32402 5.44119 4.75038 5.09918 5.06847 4.85095C5.2263 4.72777 5.5052 4.51712 5.60146 4.44442L5.60411 4.44241C5.90058 4.22407 6.31791 4.28741 6.53625 4.58387C6.75458 4.88032 6.69126 5.29762 6.39484 5.51597C6.30608 5.58301 6.04187 5.7826 5.88877 5.90208C5.58185 6.1416 5.17487 6.46821 4.76985 6.81987C4.36152 7.1744 3.97036 7.54132 3.68653 7.86254C3.54395 8.02391 3.41356 8.20932 3.35136 8.3125Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </VBtn>
                        </div>
                        <div v-if="iseventDetailsLoading">
                          <v-skeleton-loader
                            type="subtitle"
                            style="width: 100%; max-width: 200px"
                          ></v-skeleton-loader>
                        </div>
                        <div v-else class="text-primary-text">
                          {{ selectedEvent?.address }}
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" class="px-0 d-flex gap-2">
                      <span>
                        <svg
                          width="24"
                          height="25"
                          viewBox="0 0 24 25"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            opacity="0.4"
                            d="M1.25 12.3125C1.25 6.37544 6.06294 1.5625 12 1.5625C17.9371 1.5625 22.75 6.37544 22.75 12.3125C22.75 18.2496 17.9371 23.0625 12 23.0625C6.06294 23.0625 1.25 18.2496 1.25 12.3125Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                          <path
                            d="M12 6.3125C12.5523 6.3125 13 6.76022 13 7.3125V10.0205C13.883 10.4063 14.5 11.2873 14.5 12.3125C14.5 12.6349 14.439 12.9431 14.3278 13.2261L15.7071 14.6054C16.0976 14.9959 16.0976 15.6291 15.7071 16.0196C15.3166 16.4101 14.6834 16.4101 14.2929 16.0196L12.9136 14.6403C12.6306 14.7515 12.3224 14.8125 12 14.8125C10.6193 14.8125 9.5 13.6932 9.5 12.3125C9.5 11.2873 10.117 10.4063 11 10.0205V7.3125C11 6.76022 11.4477 6.3125 12 6.3125Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                        </svg>
                      </span>
                      <div xlass="w-100">
                        <div class="text-sec-text mb-2">
                          إجمالي الساعات هذا الأسبوع
                        </div>
                        <div v-if="iseventDetailsLoading">
                          <v-skeleton-loader
                            type="subtitle"
                            style="width: 100%; max-width: 200px"
                          ></v-skeleton-loader>
                        </div>
                        <div v-else class="text-primary-text">
                          {{ selectedEvent?.total_working_hours }} ساعة
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" class="px-0 d-flex gap-2">
                      <span>
                        <svg
                          width="22"
                          height="22"
                          viewBox="0 0 22 22"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            opacity="0.4"
                            d="M13.0001 7.0625C12.3634 7.0625 11.8011 7.09473 11.3233 7.19969C10.8371 7.30651 10.3918 7.49862 10.039 7.85142C9.68621 8.20422 9.4941 8.64949 9.38728 9.13573C9.28232 9.61348 9.25009 10.1758 9.25009 10.8125V19.5625H13.9997V17.8125C13.9997 17.2602 14.4474 16.8125 14.9997 16.8125C15.5519 16.8125 15.9997 17.2602 15.9997 17.8125V19.5625H20.7501V10.8125C20.7501 10.1758 20.7179 9.61348 20.6129 9.13573C20.5061 8.64949 20.314 8.20422 19.9612 7.85142C19.6084 7.49862 19.1631 7.30651 18.6769 7.19969C18.1991 7.09473 17.6368 7.0625 17.0001 7.0625H13.0001Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                          <path
                            d="M0.250183 20.5625C0.250183 20.0102 0.697898 19.5625 1.25018 19.5625V13.5625H3.99974C4.41396 13.5625 4.74974 13.2267 4.74974 12.8125C4.74974 12.3983 4.41396 12.0625 3.99974 12.0625H1.25018V9.5625H3.99974C4.41396 9.5625 4.74974 9.22671 4.74974 8.8125C4.74974 8.39829 4.41396 8.0625 3.99974 8.0625H1.25018V5.5625H3.99974C4.41396 5.5625 4.74974 5.22671 4.74974 4.8125C4.74974 4.39829 4.41396 4.0625 3.99974 4.0625H1.25018V3.8125C1.25018 3.17577 1.28242 2.61348 1.38737 2.13573C1.49419 1.64949 1.6863 1.20422 2.0391 0.851419C2.39191 0.498615 2.83717 0.306505 3.32342 0.199686C3.80117 0.0947342 4.36345 0.0625 5.00018 0.0625H11.0002C11.6369 0.0625 12.1992 0.0947342 12.677 0.199686C13.1632 0.306505 13.6085 0.498617 13.9613 0.851419C14.3141 1.20422 14.5062 1.64949 14.613 2.13573C14.7179 2.61348 14.7502 3.17577 14.7502 3.8125V4.9625C14.7502 5.24534 14.7502 5.38676 14.6623 5.47463C14.5744 5.5625 14.433 5.5625 14.1502 5.5625H13.0002C12.3306 5.5625 11.6395 5.59449 11.0016 5.73462C10.3386 5.88027 9.59739 6.17181 8.97844 6.79076C8.35949 7.40971 8.06795 8.15089 7.9223 8.81389C7.78217 9.45177 7.75018 10.143 7.75018 10.8125V19.5625H14.0002V17.8125C14.0002 17.2602 14.4479 16.8125 15.0002 16.8125C15.5525 16.8125 16.0002 17.2602 16.0002 17.8125V19.5625H20.7502C21.3025 19.5625 21.7502 20.0102 21.7502 20.5625C21.7502 21.1148 21.3025 21.5625 20.7502 21.5625H1.25018C0.697898 21.5625 0.250183 21.1148 0.250183 20.5625Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                          <path
                            d="M13.2502 10.8125C13.2502 10.3983 13.586 10.0625 14.0002 10.0625H16.0002C16.4144 10.0625 16.7502 10.3983 16.7502 10.8125C16.7502 11.2267 16.4144 11.5625 16.0002 11.5625H14.0002C13.586 11.5625 13.2502 11.2267 13.2502 10.8125Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                          <path
                            d="M13.2502 13.8125C13.2502 13.3983 13.586 13.0625 14.0002 13.0625H16.0002C16.4144 13.0625 16.7502 13.3983 16.7502 13.8125C16.7502 14.2267 16.4144 14.5625 16.0002 14.5625H14.0002C13.586 14.5625 13.2502 14.2267 13.2502 13.8125Z"
                            fill="rgba(var(--v-theme-primary))"
                          />
                        </svg>
                      </span>
                      <div class="w-100">
                        <div class="text-sec-text mb-2">الحد الأقصى للحضور</div>
                        <div v-if="iseventDetailsLoading">
                          <v-skeleton-loader
                            type="subtitle"
                            style="width: 100%; max-width: 200px"
                          ></v-skeleton-loader>
                        </div>
                        <div v-else class="text-primary-text">
                          {{ selectedEvent.max_users }} شخص
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" class="px-0">
                      <!-- <v-img src="/imgs/map.png"></v-img> -->
                      <MapDisplay
                        height="250px"
                        :longitude="selectedEvent?.longitude"
                        :latitude="selectedEvent?.latitude"
                        readonly
                      />
                    </v-col>
                  </v-row>
                </VCardText>
              </VCard>
            </v-window-item>

            <!-- observers Tab -->
            <v-window-item value="observers" class="px-3 px-lg-6 py-3">
              <v-card class="px-0 border">
                <v-card-title class="text-primary-text">
                  <h5>المشرفون</h5>
                </v-card-title>
                <v-divider></v-divider>

                <VCardText class="px-3 px-lg-4">
                  <v-skeleton-loader
                    v-if="iseventDetailsLoading"
                    type="article"
                  ></v-skeleton-loader>

                  <v-card
                    v-else-if="selectedEventSupervisor"
                    rounded="lg"
                    class="border border-light mb-2"
                    v-for="supervisor in [selectedEventSupervisor]"
                  >
                    <VCardTitle class="d-flex ga-2">
                      <span>
                        <User :size="45" />
                      </span>
                      <div class="d-flex flex-column">
                        <h5 class="mb-0">
                          {{ supervisor?.full_name }}
                        </h5>
                        <span class="text-xs text-sec-text" size="small">
                          {{ supervisor?.user_type?.name?.ar }}
                        </span>
                      </div>
                    </VCardTitle>
                    <v-divider></v-divider>
                    <VCardText class="px-2 py-4">
                      <v-row class="mx-0">
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="16"
                              height="17"
                              viewBox="0 0 16 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M8.97691 3.97949L6.36561 3.97949C5.20285 3.97948 4.27835 3.97947 3.55406 4.0787C2.80652 4.18111 2.19829 4.39757 1.71904 4.88592C1.24107 5.37297 1.03039 5.98891 0.930487 6.74614C0.833356 7.48232 0.833364 8.49655 0.833374 9.68305C0.833364 10.8695 0.833356 11.81 0.930487 12.5462C1.03039 13.3034 1.24107 13.9193 1.71904 14.4064C2.19829 14.8948 2.80652 15.1112 3.55406 15.2136C4.27834 15.3128 5.20281 15.3128 6.36555 15.3128H8.17049C7.54282 14.5393 7.16671 13.5533 7.16671 12.4795C7.16671 9.99421 9.18143 7.97949 11.6667 7.97949C12.7405 7.97949 13.7265 8.3556 14.5 8.98327V8.141L14.4999 8.13585C14.47 6.68558 14.3514 5.62771 13.6235 4.88592C13.1442 4.39757 12.536 4.18111 11.7885 4.0787C11.0642 3.97947 10.1397 3.97948 8.97691 3.97949Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M7.66715 1.646C8.13514 1.64597 8.65961 1.64595 8.99318 1.68357C9.35279 1.72412 9.69225 1.81377 10.001 2.03353C10.3077 2.25191 10.4973 2.53702 10.6468 2.85445C10.787 3.15207 10.9142 3.52923 11.0629 3.96972L11.0775 4.01309C10.6762 3.99021 10.2274 3.98283 9.72729 3.98046C9.6347 3.71589 9.54936 3.48589 9.48561 3.35054C9.38388 3.13458 9.30721 3.0529 9.23902 3.00436C9.17277 2.9572 9.07572 2.9134 8.84354 2.88722C8.59365 2.85904 8.18039 2.85806 7.66715 2.85806C7.15392 2.85806 6.74065 2.85904 6.49076 2.88722C6.25858 2.9134 6.16154 2.9572 6.09528 3.00436C6.0271 3.0529 5.95042 3.13458 5.8487 3.35054C5.78494 3.4859 5.69959 3.71591 5.607 3.9805C5.10668 3.98294 4.65789 3.99047 4.25659 4.01363L4.27141 3.96972C4.42005 3.52923 4.54733 3.15207 4.68752 2.85445C4.83703 2.53702 5.02658 2.25191 5.33335 2.03353C5.64205 1.81377 5.98151 1.72412 6.34112 1.68357C6.67469 1.64595 7.19916 1.64597 7.66715 1.646ZM8.16675 12.4793C8.16675 10.5463 9.73375 8.97933 11.6667 8.97933C13.5997 8.97933 15.1667 10.5463 15.1667 12.4793C15.1667 14.4123 13.5997 15.9793 11.6667 15.9793C9.73375 15.9793 8.16675 14.4123 8.16675 12.4793ZM11.6667 10.7788C11.9429 10.7788 12.1667 11.0027 12.1667 11.2788V12.5112L12.8441 12.9628C13.0739 13.116 13.136 13.4264 12.9828 13.6562C12.8296 13.8859 12.5192 13.948 12.2894 13.7948L11.3894 13.1948C11.2503 13.1021 11.1667 12.946 11.1667 12.7788V11.2788C11.1667 11.0027 11.3906 10.7788 11.6667 10.7788Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">
                              ساعات العمل هذا الشهر
                            </div>
                            <div class="text-primary-text">
                              {{ supervisor?.monthly_working_hours }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="16"
                              height="18"
                              viewBox="0 0 16 18"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M2.99149 2.86267C3.30733 2.79328 3.67907 2.77197 4.10002 2.77197H8.06668C8.48763 2.77197 8.85937 2.79328 9.17521 2.86267C9.49668 2.93329 9.79104 3.06029 10.0243 3.29354C10.2575 3.52678 10.3845 3.82115 10.4552 4.14261C10.5245 4.45846 10.5459 4.83019 10.5459 5.25114V16.49C10.5459 16.7639 10.3239 16.9859 10.05 16.9859H2.11668C1.84284 16.9859 1.62085 16.7639 1.62085 16.49V5.25114C1.62085 4.83019 1.64216 4.45846 1.71155 4.14261C1.78216 3.82115 1.90917 3.52678 2.14241 3.29354C2.37566 3.06029 2.67003 2.93329 2.99149 2.86267Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.96127 12.0562C5.17468 12.0276 5.43703 12.0276 5.72317 12.0276H6.44345C6.72959 12.0276 6.99194 12.0276 7.20535 12.0562C7.44036 12.0878 7.68876 12.1622 7.89306 12.3665C8.09736 12.5708 8.1717 12.8192 8.20329 13.0542C8.23199 13.2676 8.23196 13.53 8.23192 13.8161L8.23192 16.9859H7.24025V13.8457C7.24025 13.52 7.2392 13.3256 7.22047 13.1863C7.20528 13.0733 7.1865 13.0543 7.07321 13.0391C6.9339 13.0203 6.73953 13.0193 6.41386 13.0193H5.75275C5.42708 13.0193 5.23271 13.0203 5.0934 13.0391C4.97554 13.0549 4.96164 13.0711 4.94615 13.1863C4.92742 13.3256 4.92636 13.52 4.92636 13.8457V16.9859H3.9347L3.93469 13.8161C3.93466 13.53 3.93463 13.2676 3.96332 13.0542C3.99492 12.8192 4.06926 12.5708 4.27356 12.3665C4.47786 12.1622 4.72625 12.0878 4.96127 12.0562Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M10.5458 6.40817H11.868C12.2926 6.40817 12.6785 6.42945 13.012 6.50272C13.353 6.57763 13.68 6.71561 13.9425 6.97814C14.205 7.24068 14.343 7.56763 14.4179 7.90862C14.4912 8.24213 14.5125 8.62809 14.5125 9.05262V16.3248C14.5125 16.69 14.2165 16.9859 13.8514 16.9859H10.05C10.3238 16.9859 10.5458 16.764 10.5458 16.4901V15.6637H13.1902V9.05262C13.1902 8.6567 13.1687 8.38452 13.1265 8.19233C13.0859 8.00762 13.0357 7.94128 13.0075 7.9131C12.9794 7.88491 12.913 7.83472 12.7283 7.79414C12.5361 7.75192 12.2639 7.73039 11.868 7.73039H10.5458V6.40817Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 5.91234C4.92636 5.6385 5.14835 5.4165 5.42219 5.4165H6.74441C7.01826 5.4165 7.24025 5.6385 7.24025 5.91234C7.24025 6.18618 7.01826 6.40817 6.74441 6.40817H5.42219C5.14835 6.40817 4.92636 6.18618 4.92636 5.91234Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 7.89567C4.92636 7.62183 5.14835 7.39984 5.42219 7.39984H6.74441C7.01826 7.39984 7.24025 7.62183 7.24025 7.89567C7.24025 8.16951 7.01826 8.3915 6.74441 8.3915H5.42219C5.14835 8.3915 4.92636 8.16951 4.92636 7.89567Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 9.879C4.92636 9.60516 5.14835 9.38317 5.42219 9.38317H6.74441C7.01826 9.38317 7.24025 9.60516 7.24025 9.879C7.24025 10.1528 7.01826 10.3748 6.74441 10.3748H5.42219C5.14835 10.3748 4.92636 10.1528 4.92636 9.879Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">المدينة</div>
                            <div class="text-primary-text">
                              {{ supervisor?.city?.name?.ar }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="17"
                              height="17"
                              viewBox="0 0 17 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M1.33325 8.81266C1.33325 4.85462 4.54188 1.646 8.49992 1.646C12.458 1.646 15.6666 4.85462 15.6666 8.81266C15.6666 12.7707 12.458 15.9793 8.49992 15.9793C4.54188 15.9793 1.33325 12.7707 1.33325 8.81266Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M8.49992 4.8125C8.86811 4.8125 9.16659 5.11098 9.16659 5.47917V7.28451C9.75522 7.5417 10.1666 8.12906 10.1666 8.8125C10.1666 9.02745 10.1259 9.2329 10.0518 9.42156L10.9713 10.3411C11.2317 10.6014 11.2317 11.0236 10.9713 11.2839C10.711 11.5443 10.2889 11.5443 10.0285 11.2839L9.10898 10.3644C8.92032 10.4385 8.71487 10.4792 8.49992 10.4792C7.57944 10.4792 6.83325 9.73298 6.83325 8.8125C6.83325 8.12906 7.24462 7.5417 7.83325 7.28451V5.47917C7.83325 5.11098 8.13173 4.8125 8.49992 4.8125Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">تسجيل حضور</div>
                            <div class="text-primary-text">
                              {{
                                supervisor?.today_check_in
                                  ? `اليوم - ${formatTimeTo12Hour(
                                      supervisor?.today_check_in
                                    )}`
                                  : "لم يسجل"
                              }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="17"
                              height="17"
                              viewBox="0 0 17 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M1.33325 8.81266C1.33325 4.85462 4.54188 1.646 8.49992 1.646C12.458 1.646 15.6666 4.85462 15.6666 8.81266C15.6666 12.7707 12.458 15.9793 8.49992 15.9793C4.54188 15.9793 1.33325 12.7707 1.33325 8.81266Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M8.49992 4.8125C8.86811 4.8125 9.16659 5.11098 9.16659 5.47917V7.28451C9.75522 7.5417 10.1666 8.12906 10.1666 8.8125C10.1666 9.02745 10.1259 9.2329 10.0518 9.42156L10.9713 10.3411C11.2317 10.6014 11.2317 11.0236 10.9713 11.2839C10.711 11.5443 10.2889 11.5443 10.0285 11.2839L9.10898 10.3644C8.92032 10.4385 8.71487 10.4792 8.49992 10.4792C7.57944 10.4792 6.83325 9.73298 6.83325 8.8125C6.83325 8.12906 7.24462 7.5417 7.83325 7.28451V5.47917C7.83325 5.11098 8.13173 4.8125 8.49992 4.8125Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">تسجيل انصراف</div>
                            <div class="text-primary-text">
                              {{
                                supervisor?.today_check_out
                                  ? `اليوم - ${formatTimeTo12Hour(
                                      supervisor?.today_check_out
                                    )}`
                                  : "لم يسجل"
                              }}
                            </div>
                          </div>
                        </v-col>
                      </v-row>
                    </VCardText>
                  </v-card>
                  <div v-else>
                    <p class="text-center">لا توجد بيانات</p>
                  </div>
                </VCardText>
              </v-card>
            </v-window-item>

            <!-- employees Tab -->
            <v-window-item
              value="employees"
              class="px-3 px-lg-6 py-3 align-start"
            >
              <v-card class="px-0 border">
                <v-card-title class="text-primary-text">
                  <h5>الموظفون</h5>
                </v-card-title>
                <v-divider></v-divider>

                <VCardText class="px-3 px-lg-4">
                  <v-skeleton-loader
                    v-if="iseventDetailsLoading"
                    type="article"
                  ></v-skeleton-loader>
                  <v-card
                    v-else-if="selectedEventInspectors?.length"
                    rounded="lg"
                    class="border border-light mb-2"
                    v-for="instructor in selectedEventInspectors"
                  >
                    <VCardTitle class="d-flex ga-2">
                      <span>
                        <User :size="45" />
                      </span>
                      <div class="d-flex flex-column">
                        <h5 class="mb-0">{{ instructor.full_name }}</h5>
                        <span class="text-xs text-sec-text" size="small">
                          {{ instructor.user_type?.name?.ar }}
                        </span>
                      </div>
                    </VCardTitle>
                    <v-divider></v-divider>
                    <VCardText class="px-2 py-4">
                      <v-row class="mx-0">
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="16"
                              height="17"
                              viewBox="0 0 16 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M8.97691 3.97949L6.36561 3.97949C5.20285 3.97948 4.27835 3.97947 3.55406 4.0787C2.80652 4.18111 2.19829 4.39757 1.71904 4.88592C1.24107 5.37297 1.03039 5.98891 0.930487 6.74614C0.833356 7.48232 0.833364 8.49655 0.833374 9.68305C0.833364 10.8695 0.833356 11.81 0.930487 12.5462C1.03039 13.3034 1.24107 13.9193 1.71904 14.4064C2.19829 14.8948 2.80652 15.1112 3.55406 15.2136C4.27834 15.3128 5.20281 15.3128 6.36555 15.3128H8.17049C7.54282 14.5393 7.16671 13.5533 7.16671 12.4795C7.16671 9.99421 9.18143 7.97949 11.6667 7.97949C12.7405 7.97949 13.7265 8.3556 14.5 8.98327V8.141L14.4999 8.13585C14.47 6.68558 14.3514 5.62771 13.6235 4.88592C13.1442 4.39757 12.536 4.18111 11.7885 4.0787C11.0642 3.97947 10.1397 3.97948 8.97691 3.97949Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M7.66715 1.646C8.13514 1.64597 8.65961 1.64595 8.99318 1.68357C9.35279 1.72412 9.69225 1.81377 10.001 2.03353C10.3077 2.25191 10.4973 2.53702 10.6468 2.85445C10.787 3.15207 10.9142 3.52923 11.0629 3.96972L11.0775 4.01309C10.6762 3.99021 10.2274 3.98283 9.72729 3.98046C9.6347 3.71589 9.54936 3.48589 9.48561 3.35054C9.38388 3.13458 9.30721 3.0529 9.23902 3.00436C9.17277 2.9572 9.07572 2.9134 8.84354 2.88722C8.59365 2.85904 8.18039 2.85806 7.66715 2.85806C7.15392 2.85806 6.74065 2.85904 6.49076 2.88722C6.25858 2.9134 6.16154 2.9572 6.09528 3.00436C6.0271 3.0529 5.95042 3.13458 5.8487 3.35054C5.78494 3.4859 5.69959 3.71591 5.607 3.9805C5.10668 3.98294 4.65789 3.99047 4.25659 4.01363L4.27141 3.96972C4.42005 3.52923 4.54733 3.15207 4.68752 2.85445C4.83703 2.53702 5.02658 2.25191 5.33335 2.03353C5.64205 1.81377 5.98151 1.72412 6.34112 1.68357C6.67469 1.64595 7.19916 1.64597 7.66715 1.646ZM8.16675 12.4793C8.16675 10.5463 9.73375 8.97933 11.6667 8.97933C13.5997 8.97933 15.1667 10.5463 15.1667 12.4793C15.1667 14.4123 13.5997 15.9793 11.6667 15.9793C9.73375 15.9793 8.16675 14.4123 8.16675 12.4793ZM11.6667 10.7788C11.9429 10.7788 12.1667 11.0027 12.1667 11.2788V12.5112L12.8441 12.9628C13.0739 13.116 13.136 13.4264 12.9828 13.6562C12.8296 13.8859 12.5192 13.948 12.2894 13.7948L11.3894 13.1948C11.2503 13.1021 11.1667 12.946 11.1667 12.7788V11.2788C11.1667 11.0027 11.3906 10.7788 11.6667 10.7788Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">
                              ساعات العمل هذا الشهر
                            </div>
                            <div class="text-primary-text">
                              {{ instructor.monthly_working_hours }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="16"
                              height="18"
                              viewBox="0 0 16 18"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M2.99149 2.86267C3.30733 2.79328 3.67907 2.77197 4.10002 2.77197H8.06668C8.48763 2.77197 8.85937 2.79328 9.17521 2.86267C9.49668 2.93329 9.79104 3.06029 10.0243 3.29354C10.2575 3.52678 10.3845 3.82115 10.4552 4.14261C10.5245 4.45846 10.5459 4.83019 10.5459 5.25114V16.49C10.5459 16.7639 10.3239 16.9859 10.05 16.9859H2.11668C1.84284 16.9859 1.62085 16.7639 1.62085 16.49V5.25114C1.62085 4.83019 1.64216 4.45846 1.71155 4.14261C1.78216 3.82115 1.90917 3.52678 2.14241 3.29354C2.37566 3.06029 2.67003 2.93329 2.99149 2.86267Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.96127 12.0562C5.17468 12.0276 5.43703 12.0276 5.72317 12.0276H6.44345C6.72959 12.0276 6.99194 12.0276 7.20535 12.0562C7.44036 12.0878 7.68876 12.1622 7.89306 12.3665C8.09736 12.5708 8.1717 12.8192 8.20329 13.0542C8.23199 13.2676 8.23196 13.53 8.23192 13.8161L8.23192 16.9859H7.24025V13.8457C7.24025 13.52 7.2392 13.3256 7.22047 13.1863C7.20528 13.0733 7.1865 13.0543 7.07321 13.0391C6.9339 13.0203 6.73953 13.0193 6.41386 13.0193H5.75275C5.42708 13.0193 5.23271 13.0203 5.0934 13.0391C4.97554 13.0549 4.96164 13.0711 4.94615 13.1863C4.92742 13.3256 4.92636 13.52 4.92636 13.8457V16.9859H3.9347L3.93469 13.8161C3.93466 13.53 3.93463 13.2676 3.96332 13.0542C3.99492 12.8192 4.06926 12.5708 4.27356 12.3665C4.47786 12.1622 4.72625 12.0878 4.96127 12.0562Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M10.5458 6.40817H11.868C12.2926 6.40817 12.6785 6.42945 13.012 6.50272C13.353 6.57763 13.68 6.71561 13.9425 6.97814C14.205 7.24068 14.343 7.56763 14.4179 7.90862C14.4912 8.24213 14.5125 8.62809 14.5125 9.05262V16.3248C14.5125 16.69 14.2165 16.9859 13.8514 16.9859H10.05C10.3238 16.9859 10.5458 16.764 10.5458 16.4901V15.6637H13.1902V9.05262C13.1902 8.6567 13.1687 8.38452 13.1265 8.19233C13.0859 8.00762 13.0357 7.94128 13.0075 7.9131C12.9794 7.88491 12.913 7.83472 12.7283 7.79414C12.5361 7.75192 12.2639 7.73039 11.868 7.73039H10.5458V6.40817Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 5.91234C4.92636 5.6385 5.14835 5.4165 5.42219 5.4165H6.74441C7.01826 5.4165 7.24025 5.6385 7.24025 5.91234C7.24025 6.18618 7.01826 6.40817 6.74441 6.40817H5.42219C5.14835 6.40817 4.92636 6.18618 4.92636 5.91234Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 7.89567C4.92636 7.62183 5.14835 7.39984 5.42219 7.39984H6.74441C7.01826 7.39984 7.24025 7.62183 7.24025 7.89567C7.24025 8.16951 7.01826 8.3915 6.74441 8.3915H5.42219C5.14835 8.3915 4.92636 8.16951 4.92636 7.89567Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M4.92636 9.879C4.92636 9.60516 5.14835 9.38317 5.42219 9.38317H6.74441C7.01826 9.38317 7.24025 9.60516 7.24025 9.879C7.24025 10.1528 7.01826 10.3748 6.74441 10.3748H5.42219C5.14835 10.3748 4.92636 10.1528 4.92636 9.879Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">المدينة</div>
                            <div class="text-primary-text">
                              {{ instructor.city?.name?.ar }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="17"
                              height="17"
                              viewBox="0 0 17 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M1.33325 8.81266C1.33325 4.85462 4.54188 1.646 8.49992 1.646C12.458 1.646 15.6666 4.85462 15.6666 8.81266C15.6666 12.7707 12.458 15.9793 8.49992 15.9793C4.54188 15.9793 1.33325 12.7707 1.33325 8.81266Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M8.49992 4.8125C8.86811 4.8125 9.16659 5.11098 9.16659 5.47917V7.28451C9.75522 7.5417 10.1666 8.12906 10.1666 8.8125C10.1666 9.02745 10.1259 9.2329 10.0518 9.42156L10.9713 10.3411C11.2317 10.6014 11.2317 11.0236 10.9713 11.2839C10.711 11.5443 10.2889 11.5443 10.0285 11.2839L9.10898 10.3644C8.92032 10.4385 8.71487 10.4792 8.49992 10.4792C7.57944 10.4792 6.83325 9.73298 6.83325 8.8125C6.83325 8.12906 7.24462 7.5417 7.83325 7.28451V5.47917C7.83325 5.11098 8.13173 4.8125 8.49992 4.8125Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">تسجيل حضور</div>
                            <div class="text-primary-text">
                              {{
                                instructor?.today_check_in
                                  ? `اليوم - ${formatTimeTo12Hour(
                                      instructor?.today_check_in
                                    )}`
                                  : "لم يسجل"
                              }}
                            </div>
                          </div>
                        </v-col>
                        <v-col cols="12" sm="6" class="d-flex gap-2">
                          <span>
                            <svg
                              width="17"
                              height="17"
                              viewBox="0 0 17 17"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                d="M1.33325 8.81266C1.33325 4.85462 4.54188 1.646 8.49992 1.646C12.458 1.646 15.6666 4.85462 15.6666 8.81266C15.6666 12.7707 12.458 15.9793 8.49992 15.9793C4.54188 15.9793 1.33325 12.7707 1.33325 8.81266Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                              <path
                                d="M8.49992 4.8125C8.86811 4.8125 9.16659 5.11098 9.16659 5.47917V7.28451C9.75522 7.5417 10.1666 8.12906 10.1666 8.8125C10.1666 9.02745 10.1259 9.2329 10.0518 9.42156L10.9713 10.3411C11.2317 10.6014 11.2317 11.0236 10.9713 11.2839C10.711 11.5443 10.2889 11.5443 10.0285 11.2839L9.10898 10.3644C8.92032 10.4385 8.71487 10.4792 8.49992 10.4792C7.57944 10.4792 6.83325 9.73298 6.83325 8.8125C6.83325 8.12906 7.24462 7.5417 7.83325 7.28451V5.47917C7.83325 5.11098 8.13173 4.8125 8.49992 4.8125Z"
                                fill="rgba(var(--v-theme-primary))"
                              />
                            </svg>
                          </span>
                          <div class="text-xs font-bold">
                            <div class="text-sec-text mb-2">تسجيل انصراف</div>
                            <div class="text-primary-text">
                              {{
                                instructor?.today_check_out
                                  ? `اليوم - ${formatTimeTo12Hour(
                                      instructor?.today_check_out
                                    )}`
                                  : "لم يسجل"
                              }}
                            </div>
                          </div>
                        </v-col>
                      </v-row>
                    </VCardText>
                  </v-card>
                  <div v-else>
                    <p class="text-center">لا توجد بيانات</p>
                  </div>
                </VCardText>
              </v-card>
            </v-window-item>
          </v-window>
        </v-card-text>
      </v-card>
    </v-dialog>
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
  font-weight: bolder !important;
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
.show-on-map {
  position: absolute;
  left: 0px;
  top: 0px;
}
</style>
