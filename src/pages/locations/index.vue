<script setup lang="ts">
  import { usePaginatedFetch } from '@/composables/usePaginatedFetch'
  import { useAppStore } from '@/stores/app'
  import Magnifier from '@/components/icons/magnifier.vue'
  import DataTable from '@/components/DataTable.vue'
  import Pagination from '@/@core/components/Pagination.vue'
  import Eyeicon from '@/components/icons/eyeicon.vue'
  import EventsWidgets from '@/components/SitesWidgets.vue'
  import { useDashboardStore } from '@/stores/dashboard'
  import ExportIcon from '@/components/icons/export.vue'
  import FilterMenu from '@/@core/components/filters/filter-menu.vue'
  import { exportData } from '@/@core/utils/helpers'
  import LocationDetailsDialog from '@/components/LocationDetailsDialog.vue'

  import type { SitesWidgetData } from '@/types/models'

  const appStore = useAppStore()
  const dashboardStore = useDashboardStore()
  const {
    data: locations,
    isLoading,
    initialLoading: widgetsLoading,
    fetchError,
    perPage,
    totalItems,
    currentPage,
    search,
    currentFilters,
    widgetsData,
    fetchData: fetchLocations,
    handleSearch,
    applyFilters,
    clearFilters,
  } = usePaginatedFetch('locations', { defaultPerPage: 5, extractStatistics: true })

  const widgetStats = computed(() => widgetsData.value as unknown as SitesWidgetData | null)

  const detailsDialog = ref(false)
  const selectedLocationId = ref<number | null>(null)
  const headers = [
    { title: 'اسم الموقع', value: 'name', align: 'start' },
    { title: 'المدينة', value: 'city.name.ar' },
    { title: 'عدد الموظفين المرتبطين', value: 'currentEventsCount' },
    { title: '', value: 'actions' },
  ]

  const showDetails = (location: Record<string, any>) => {
    selectedLocationId.value = location.id
    detailsDialog.value = true
  }
  const closeDetails = () => {
    detailsDialog.value = false
  }

  onMounted(() => {
    dashboardStore.fetchCities()
  })

  const filters = computed(() => {
    return [
      {
        type: 'select',
        key: 'city_id',
        label: 'اختر المدينة ',
        items: dashboardStore.city,
        itemValue: 'id',
        itemTitle: 'name.ar',
      },
      {
        type: 'date',
        key: 'created_at',
        label: 'تاريخ الإنشاء',
      },
    ]
  })

  const isExporting = ref(false)
  const exportLocationsData = async () => {
    try {
      isExporting.value = true
      await exportData('locations/locations-export-excel', currentFilters.value)
    } catch (error: any) {
      // export failed silently
    } finally {
      isExporting.value = false
    }
  }
</script>

<template>
  <v-container class="px-0" fluid>
    <!-- Add eventsWidgets component -->
    <EventsWidgets :data="widgetStats" :is-loading="widgetsLoading" />

    <v-container class="px-0 px-md-3 py-0 mt-4" fluid>
      <VCard class="rounded-lg">
        <VCardTitle class="py-4">
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between ga-4">
            <h2 class="text-h5 font-weight-bold">قائمة المواقع</h2>

            <!-- Search -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-3"
              style="flex-grow: 1; min-width: 0"
            >
              <v-text-field
                v-model="search"
                class="border-grey-900 flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                placeholder="بحث عن موقع"
                rounded="lg"
                :style="{
                  'min-width': '200px',
                  'max-width': '300px',
                  'margin-inline-end': '12px',
                }"
                @update:model-value="handleSearch"
              >
                <template #prepend-inner>
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
                class="bg-background border border-grey-900"
                :disabled="isExporting"
                height="45"
                :loading="isExporting"
                variant="outlined"
                @click="exportLocationsData"
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
                v-model:page="currentPage"
                class="elevation-0"
                :headers="headers"
                :hide-default-footer="true"
                :items="locations"
                :items-per-page="perPage"
                :loading="widgetsLoading"
                :total-items="totalItems"
              >
                <template #[`item.currentEventsCount`]="{ item }">
                  <span> {{ item.currentEventsCount || 0 }} موظف </span>
                </template>
                <!--  {{ item.observerNumber || 0 }} مراقب -->

                <template #[`item.totalWorkingHours`]="{ item }">
                  {{ item.totalWorkingHours }} ساعة
                </template>
                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      class="text-sm rounded-lg"
                      color="surface"
                      size="small"
                      @click="showDetails(item)"
                    >
                      <template #prepend>
                        <Eyeicon />
                      </template>
                      عرض
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <!-- Error state -->
              <div v-if="fetchError && !isLoading" class="pa-8 text-center">
                <p class="text-error text-h6 mb-2">حدث خطأ في تحميل البيانات</p>
                <v-btn color="primary" variant="outlined" @click="fetchLocations">
                  إعادة المحاولة
                </v-btn>
              </div>

              <!-- Empty state -->
              <div
                v-else-if="!locations.length && !isLoading && !fetchError"
                class="pa-8 text-center text-grey"
              >
                <p class="text-h6">لا توجد مواقع</p>
                <p class="text-sm">لم يتم العثور على نتائج تطابق بحثك</p>
              </div>

              <!-- Pagination -->
              <Pagination
                v-if="locations.length && !fetchError"
                v-model:page="currentPage"
                v-model:per-page="perPage"
                :total-items="totalItems"
              />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>
    </v-container>

    <!-- show details dialog -->
    <LocationDetailsDialog v-if="selectedLocationId" v-model="detailsDialog" :location-id="selectedLocationId" @close="closeDetails" />
  </v-container>
</template>

<style lang="scss">
.show-on-map {
  position: absolute;
  left: 0px;
  top: 0px;
}
</style>
