<script setup lang="ts">
  import { usePaginatedFetch } from '@/composables/usePaginatedFetch'
  import Magnifier from '@/components/icons/magnifier.vue'
  import DataTable from '@/components/DataTable.vue'
  import Pagination from '@/@core/components/Pagination.vue'
  import Eyeicon from '@/components/icons/eyeicon.vue'
  import User from '@/@core/components/icons/user.vue'
  import AttendeWidgets from '@/components/AttendeWidgets.vue'
  import Correct from '@/components/icons/correct.vue'
  import UserDialogDetails from '@/components/UserDialogDetails.vue'
  import { useUserDetails } from '@/composables/useUserDetails'
  import { locationsApi, usersApi } from '@/api'
  import { useDashboardStore } from '@/stores/dashboard'
  import FilterMenu from '@/@core/components/filters/filter-menu.vue'
  import ExportIcon from '@/components/icons/export.vue'
  import { exportData } from '@/@core/utils/helpers'
  import { formatTimeTo12Hour } from '@/@core/utils/formatters'
  import type { DashboardStats } from '@/types/models'

  const dashboardStore = useDashboardStore()
  const {
    data: attendances,
    isLoading,
    initialLoading,
    fetchError,
    perPage,
    totalItems,
    currentPage,
    search,
    currentFilters,
    widgetsData,
    fetchData: fetchAttendances,
    handleSearch,
    applyFilters,
    clearFilters,
  } = usePaginatedFetch('attendance', { defaultPerPage: 5, extractStatistics: true })

  const widgetStats = computed(() => widgetsData.value as unknown as DashboardStats | null)

  const {
    detailsDialog,
    selectedUser,
    isUserDetailsLoading,
    showDetails,
  } = useUserDetails()
  const tab = ref('profile')
  const STATUSOPTIONS: Record<string, string> = { absent: 'غائب', attendance: 'حاضر', departed: 'غادر' }
  const headers = [
    { title: 'الاسم والدور الوظيفى', value: 'user.fullName', align: 'start' },
    { title: 'رقم الهوية', value: 'user.saId' },
    { title: 'المدينة', value: 'userCity' },
    { title: 'حالة الحضور', value: 'status' },
    { title: 'وقت الدخول', value: 'checkIn' },
    { title: 'وقت الإنصراف', value: 'checkOut' },
    { title: 'الموقع', value: 'location' },
    { title: '', value: 'actions' },
  ]

  const locations = ref<any[]>([])
  const fetchLocations = async (search = '') => {
    try {
      const searchVal = !search.trim() ? {} : { search: search.trim() }
      const response = await locationsApi.getList({ per_page: 6, ...searchVal })
      if (response.data.data) {
        locations.value = response.data.data
      }
    } catch (error: any) {
      // silently fail
    }
  }
  const users = ref<any[]>([])
  const fetchUsers = async (search = '') => {
    try {
      const searchVal = !search.trim() ? {} : { search: search.trim() }
      const response = await usersApi.getList({ per_page: 6, ...searchVal })
      if (response.data.data) {
        users.value = response.data.data
      }
    } catch (error: any) {
      // silently fail
    }
  }

  onMounted(() => {
    fetchLocations()
    fetchUsers()
  })

  const filters = computed(() => {
    return [
      {
        type: 'select',
        key: 'status',
        label: 'اختر حالة المستخدم',
        items: [
          { value: 'attendance', label: 'حاضر' },
          { value: 'absent', label: 'غائب' },
          { value: 'departed', label: 'غادر' },
        ],
        itemValue: 'value',
        itemTitle: 'label',
      },
      {
        type: 'searchable',
        key: 'user_id',
        label: 'اختر مستخدم ',
        items: users.value,
        itemValue: 'id',
        itemTitle: 'fullName',
        searchTrigger: (value: string) => fetchUsers(value),
      },
      {
        type: 'searchable',
        key: 'location_id',
        label: 'اختر الموقع ',
        items: locations.value,
        itemValue: 'id',
        itemTitle: 'name',
        searchTrigger: (value: string) => fetchLocations(value),
      },
    ]
  })

  const isExporting = ref(false)
  const exportAttendanceData = async () => {
    try {
      isExporting.value = true
      await exportData(
        '/attendance/attendance-export-excel',
        currentFilters.value
      )
    } catch (error: any) {
      // export failed silently
    } finally {
      isExporting.value = false
    }
  }
</script>

<template>
  <v-container class="px-0" fluid>
    <AttendeWidgets
      :data="widgetStats"
      :is-loading="initialLoading"
    />

    <v-container class="px-0 px-md-3 py-0 mt-4" fluid>
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
                class="border-grey-900 flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                placeholder="بحث باسم المستخدم / رقم الهوية  "
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
            </div>
            <div class="d-flex flex-wrap align-center justify-md-end ga-3">
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
                v-model:page="currentPage"
                class="elevation-0"
                :headers="headers"
                :hide-default-footer="true"
                :items="attendances"
                :items-per-page="perPage"
                :loading="initialLoading"
                :total-items="totalItems"
              >
                <template #[`item.user.fullName`]="{ item }">
                  <div class="d-flex align-center gap-2">
                    <v-avatar v-if="item.user?.profileImage" size="sm">
                      <v-img :src="item.user.profileImage" />
                    </v-avatar>
                    <User v-else />
                    <div>
                      <h4>{{ item.user?.fullName }}</h4>
                      <span class="text-xs">{{
                        item.user?.userType?.name?.ar
                      }}</span>
                    </div>
                  </div>
                </template>
                <template #[`item.userCity`]="{ item }">
                  {{ item.user?.city?.name?.ar || '-' }}
                </template>
                <template #[`item.status`]="{ item }">
                  <v-chip
                    class="text-sm px-3"
                    :color="item?.status == 'attendance' ? 'success' : 'error'"
                    :prepend-icon="item.status == 'attendance' ? Correct : ''"
                    rounded="lg"
                    size="large"
                  >
                    <span>
                      {{ STATUSOPTIONS[item.status] }}
                    </span>
                  </v-chip>
                </template>
                <template #[`item.checkIn`]="{ item }">
                  {{
                    item.checkIn
                      ? formatTimeTo12Hour(item.checkIn)
                      : "لم يسجل"
                  }}
                </template>
                <template #[`item.checkOut`]="{ item }">
                  {{
                    item.checkOut
                      ? formatTimeTo12Hour(item.checkOut)
                      : "لم يسجل"
                  }}
                </template>
                <template #[`item.location`]="{ item }">
                  <div v-if="item.location" class="d-flex ga-2 align-center">
                    <v-img
                      v-if="item.location?.image"
                      height="35"
                      max-width="35"
                      :src="item.location?.image"
                      width="35"
                    />
                    <v-icon
                      v-else
                      class="text-primary"
                      size="35"
                    >
                      mdi-map-marker
                    </v-icon>
                    <p class="mb-0">
                      {{ item.location?.name }}
                    </p>
                  </div>
                  <div v-else>-</div>
                </template>
                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      class="text-sm rounded-lg"
                      color="surface"
                      size="small"
                      @click="showDetails(item.user)"
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
                <v-btn color="primary" variant="outlined" @click="fetchAttendances">
                  إعادة المحاولة
                </v-btn>
              </div>

              <!-- Empty state -->
              <div
                v-else-if="!attendances.length && !isLoading && !fetchError"
                class="pa-8 text-center text-grey"
              >
                <p class="text-h6">لا توجد سجلات حضور</p>
                <p class="text-sm">لم يتم العثور على نتائج تطابق بحثك</p>
              </div>
            </v-col>
            <v-col v-if="attendances.length && !fetchError" cols="12">
              <!-- Pagination -->
              <Pagination
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

    <UserDialogDetails
      v-model="detailsDialog"
      :is-user-details-loading="isUserDetailsLoading"
      :selected-user="selectedUser"
    />
  </v-container>
</template>
