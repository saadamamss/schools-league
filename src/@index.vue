<template>
  <!-- No changes to template section -->
  <v-menu
    v-model="filterMenu"
    :close-on-content-click="false"
    location="bottom end"
    min-width="350"
  >
    <template v-slot:activator="{ props }">
      <VBtn
        color="default"
        class="btn-icon border"
        elevation="0"
        v-bind="props"
        :variant="hasActiveFilters ? 'flat' : 'outlined'"
      >
        <svg
          width="1.3rem"
          height="1.3rem"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            opacity="0.4"
            d="M8.85746 12.5061C6.36901 10.6456 4.59564 8.59915 3.62734 7.44867C3.3276 7.09253 3.22938 6.8319 3.17033 6.3728C2.96811 4.8008 2.86701 4.0148 3.32795 3.5074C3.7889 3 4.60404 3 6.23433 3H17.7657C19.396 3 20.2111 3 20.672 3.5074C21.133 4.0148 21.0319 4.8008 20.8297 6.37281C20.7706 6.83191 20.6724 7.09254 20.3726 7.44867C19.403 8.60062 17.6261 10.6507 15.1326 12.5135C14.907 12.6821 14.7583 12.9567 14.7307 13.2614C14.4837 15.992 14.2559 17.4876 14.1141 18.2442C13.8853 19.4657 12.1532 20.2006 11.226 20.8563C10.6741 21.2466 10.0043 20.782 9.93278 20.1778C9.79643 19.0261 9.53961 16.6864 9.25927 13.2614C9.23409 12.9539 9.08486 12.6761 8.85746 12.5061Z"
            fill="#667178"
          />
          <path
            fill-rule="evenodd"
            clip-rule="evenodd"
            d="M6.18176 2.24732C6.1991 2.24732 6.21649 2.24732 6.23394 2.24732L17.8175 2.24732C18.588 2.24728 19.2422 2.24725 19.7583 2.31762C20.3046 2.3921 20.8269 2.56023 21.2268 3.00041C21.6302 3.44445 21.7406 3.97957 21.7491 4.52659C21.757 5.03745 21.6753 5.67204 21.5801 6.41193L21.5732 6.46581C21.5395 6.72764 21.489 6.98088 21.3836 7.23442C21.2767 7.49184 21.1294 7.71109 20.946 7.92897C19.9664 9.09285 18.1451 11.1962 15.5811 13.1117C15.5396 13.1427 15.4872 13.2161 15.4773 13.3263C15.2282 16.0792 15.009 17.5358 14.8509 18.3796C14.6799 19.2921 13.9839 19.9237 13.3838 20.3585C13.0697 20.5861 12.7364 20.791 12.4394 20.9717C12.4156 20.9862 12.3921 21.0005 12.3689 21.0146C12.0915 21.1833 11.8557 21.3267 11.6586 21.466C11.1181 21.8482 10.4945 21.8214 10.0179 21.5437C9.56811 21.2817 9.25159 20.8038 9.1876 20.2633C9.04713 19.0769 8.79249 16.7542 8.51139 13.3199C8.50238 13.2097 8.48517 13.1778 8.48373 13.1751C8.48275 13.1733 8.48022 13.1686 8.47182 13.1594C8.46243 13.149 8.44366 13.1308 8.40798 13.1041C5.84915 11.191 4.03146 9.09132 3.05314 7.92893C2.87044 7.71185 2.71884 7.49783 2.61076 7.23754C2.50494 6.98273 2.45983 6.7282 2.42607 6.46581C2.42375 6.44778 2.42144 6.42982 2.41914 6.41192C2.32392 5.67203 2.24226 5.03744 2.25016 4.52659C2.25863 3.97957 2.36905 3.44445 2.77244 3.00041C3.17232 2.56023 3.69466 2.3921 4.24094 2.31762C4.75704 2.24725 5.4112 2.24728 6.18176 2.24732Z"
            fill="#667178"
          />
        </svg>
      </VBtn>
    </template>

    <v-card class="pa-4 rounded-lg filter-menu-card">
      <v-card-title class="px-0 pt-0 d-flex justify-space-between align-center">
        <span class="text-subtitle-1 font-weight-bold">فلترة البيانات</span>
        <v-btn
          variant="text"
          color="primary"
          size="small"
          :disabled="!hasActiveFilters"
          @click="resetFilters"
        >
          إعادة تعيين
          <v-icon right>mdi-refresh</v-icon>
        </v-btn>
      </v-card-title>

      <v-divider class="mb-4"></v-divider>

      <v-card-text class="px-0 pb-0">
        <!-- Location Type Filter -->
        <div class="mb-4">
          <v-select
            v-model="tempLocationTypeFilter"
            label="نوع الموقع"
            :items="[{ id: '', title: 'الكل' }, ...locationTypes]"
            item-value="id"
            item-title="title"
            variant="outlined"
            hide-details
            density="comfortable"
            class="mb-3"
            :loading="isLoading"
            :disabled="isLoading"
            :error-messages="locationTypeErrors"
            @update:model-value="handleLocationTypeChange"
          ></v-select>
        </div>

        <!-- Location Subtype Filter -->
        <div class="mb-4">
          <v-select
            v-model="tempLocationSubTypeFilter"
            label="النوع الفرعي للموقع"
            :items="[{ id: '', title: 'الكل' }, ...filteredLocationSubTypes]"
            item-value="id"
            item-title="title"
            variant="outlined"
            hide-details
            density="comfortable"
            class="mb-3"
            :loading="isLoading"
            :disabled="isLoading || !tempLocationTypeFilter"
            :error-messages="locationSubTypeErrors"
          ></v-select>
        </div>

        <!-- Date Filter -->
        <div class="mb-4">
          <v-label class="text-body-2 mb-1 d-block">تاريخ الإنشاء</v-label>
          <v-date-picker
            v-model="tempDateFilter"
            density="compact"
            elevation="0"
            hide-details
            class="rounded-lg border"
            width="100%"
          ></v-date-picker>
        </div>
      </v-card-text>

      <v-card-actions class="px-0 pt-4">
        <v-spacer></v-spacer>
        <v-btn variant="text" color="error" @click="filterMenu = false">
          إلغاء
        </v-btn>
        <v-btn color="primary" @click="applyFilters"> تطبيق الفلتر </v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script>
import { ref, computed, watch, onMounted } from "vue";
import { useAppStore } from "@/stores/app";
import axios from "@/plugins/axios";

const locationTypes = ref([]);
const locationSubTypes = ref([]);
const isLoading = ref(true);

const locationTypeErrors = ref([]);
const locationSubTypeErrors = ref([]);

const locationTypeFilter = ref("");
const locationSubTypeFilter = ref("");
const dateFilter = ref(null);
const filterMenu = ref(false);

const tempLocationTypeFilter = ref("");
const tempLocationSubTypeFilter = ref("");
const tempDateFilter = ref(null);

const filteredLocationSubTypes = computed(() => {
  if (!tempLocationTypeFilter.value) return [];
  return locationSubTypes.value.filter(
    (subType) => subType.location_type_id === tempLocationTypeFilter.value
  );
});

const hasActiveFilters = computed(() => {
  return (
    locationTypeFilter.value !== "" ||
    locationSubTypeFilter.value !== "" ||
    dateFilter.value !== null
  );
});

const handleLocationTypeChange = (value) => {
  if (value !== tempLocationTypeFilter.value) {
    tempLocationSubTypeFilter.value = "";
  }
};

const fetchLocationTypes = async () => {
  try {
    const response = await axios.get("/location-types");
    if (response.data?.data) {
      locationTypes.value = response.data.data;
    }
  } catch (error) {
    console.error("Error fetching location types:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب أنواع المواقع",
      color: "error",
    });
  }
};

const fetchLocationSubTypes = async () => {
  try {
    const response = await axios.get("/location-sub-types");
    if (response.data?.data) {
      locationSubTypes.value = response.data.data;
    }
  } catch (error) {
    console.error("Error fetching location subtypes:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب الأنواع الفرعية للمواقع",
      color: "error",
    });
  }
};

const applyFilters = () => {
  locationTypeFilter.value = tempLocationTypeFilter.value;
  locationSubTypeFilter.value = tempLocationSubTypeFilter.value;
  dateFilter.value = tempDateFilter.value;

  filterMenu.value = false;

  currentPage.value = 1;
  fetchData();
};

const resetFilters = () => {
  locationTypeFilter.value = "";
  locationSubTypeFilter.value = "";
  dateFilter.value = null;

  tempLocationTypeFilter.value = "";
  tempLocationSubTypeFilter.value = "";
  tempDateFilter.value = null;

  filterMenu.value = false;

  currentPage.value = 1;
  fetchData();
};

watch(
  [locationTypeFilter, locationSubTypeFilter, dateFilter],
  ([newLocationType, newLocationSubType, newDate]) => {
    tempLocationTypeFilter.value = newLocationType;
    tempLocationSubTypeFilter.value = newLocationSubType;
    tempDateFilter.value = newDate;
  }
);

watch(filterMenu, (isOpen) => {
  if (isOpen) {
    tempLocationTypeFilter.value = locationTypeFilter.value;
    tempLocationSubTypeFilter.value = locationSubTypeFilter.value;
    tempDateFilter.value = dateFilter.value;
  }
});

const fetchData = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value,
    });

    if (locationTypeFilter.value) {
      queryParams.append("location_type_id", locationTypeFilter.value);
    }

    if (locationSubTypeFilter.value) {
      queryParams.append("location_sub_type_id", locationSubTypeFilter.value);
    }

    if (dateFilter.value) {
      queryParams.append("created_at", dateFilter.value);
    }

    const response = await axios.get(`/sites?${queryParams.toString()}`);
    if (response.data?.data) {
      sites.value = response.data.data;
      totalSites.value = response.data.pagination?.i_total_objects || 0;
      perPage.value = response.data.pagination?.i_per_page || perPage.value;
    }
  } catch (error) {
    console.error("Error fetching sites:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    sites.value = [];
    totalSites.value = 0;
  } finally {
    isLoading.value = false;
  }
};

onMounted(async () => {
  await Promise.all([fetchLocationTypes(), fetchLocationSubTypes()]);
  fetchData();
});
</script>

<style lang="scss" scoped>
.filter-menu-card {
  box-shadow: 0 4px 25px 0 rgba(0, 0, 0, 0.1);

  .v-card-title {
    font-size: 1.1rem;
  }

  .v-date-picker {
    box-shadow: none;
  }
}
</style>
