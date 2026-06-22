<script setup lang="ts">
  import { usePaginatedFetch } from '@/composables/usePaginatedFetch'
  import Magnifier from '@/components/icons/magnifier.vue'
  import DataTable from '@/components/DataTable.vue'
  import Pagination from '@/@core/components/Pagination.vue'
  import Eyeicon from '@/components/icons/eyeicon.vue'
  import User from '@/@core/components/icons/user.vue'
  import UsersWidgets from '@/components/UsersWidgets.vue'

  import FilterMenu from '@/@core/components/filters/filter-menu.vue'
  import { computed } from 'vue'
  import { useDashboardStore } from '@/stores/dashboard'
  import { exportData } from '@/@core/utils/helpers'
  import ExportIcon from '@/components/icons/export.vue'
  import UserDialogDetails from '@/components/UserDialogDetails.vue'
  import { useUserDetails } from '@/composables/useUserDetails'
  import type { DashboardStats } from '@/types/models'

  const dashboardStore = useDashboardStore()
  const {
    data: users,
    isLoading,
    initialLoading,
    fetchError,
    perPage,
    totalItems,
    currentPage,
    search,
    widgetsData,
    currentFilters,
    fetchData: fetchUsers,
    handleSearch,
    applyFilters,
    clearFilters,
  } = usePaginatedFetch('users', { defaultPerPage: 5, extractStatistics: true })

  const widgetStats = computed(() => widgetsData.value as unknown as DashboardStats | null)

  const headers = [
    { title: 'الاسم', value: 'fullName', align: 'start' },
    { title: 'رقم الهوية', value: 'saId' },
    { title: 'دور المستخدم', value: 'userType' },
    { title: 'المدينة', value: 'city.name.ar' },
    { title: 'رقم الجوال', value: 'phone' },
    { title: 'أيام العمل', value: 'workingDaysCount' },
    { title: '', value: 'actions' },
  ]

  const {
    detailsDialog,
    selectedUser,
    isUserDetailsLoading,
    showDetails,
  } = useUserDetails({ mergeLocation: true })

  onMounted(() => {
    Promise.all([
      dashboardStore.fetchCities(),
      dashboardStore.fetchUserTypes(),
    ])
  })

  const filters = computed(() => {
    return [
      {
        type: 'select',
        key: 'gender',
        label: 'اختر جنس المستخدم',
        items: [
          { label: 'ذكر', value: 'male' },
          { label: 'أنثى', value: 'female' },
        ],
        itemValue: 'value',
        itemTitle: 'label',
      },
      {
        type: 'select',
        key: 'user_type_id',
        label: 'اختر دور المستخدم',
        items: dashboardStore.userTypes,
        itemValue: 'id',
        itemTitle: 'name.ar',
      },
      {
        type: 'select',
        key: 'city_id',
        label: 'اختر المدينة ',
        items: dashboardStore.city,
        itemValue: 'id',
        itemTitle: 'name.ar',
        searchTrigger: (value: string) => dashboardStore.fetchCities(value),
      },
      {
        type: 'date',
        key: 'created_at',
        label: 'تاريخ الإنشاء',
      },
    ]
  })

  const isExporting = ref({
    rating: false,
  })
  const exportUserData = async () => {
    try {
      isExporting.value.rating = true
      await exportData('/users/users-export-excel', currentFilters.value)
    } catch (error: any) {
      // export failed silently
    } finally {
      isExporting.value.rating = false
    }
  }
</script>

<template>
  <v-container class="px-0" fluid>
    <!-- Add UsersWidgets component -->
    <UsersWidgets
      class="mb-4"
      :data="widgetStats"
      :is-loading="initialLoading"
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

              <!--  -->
              <FilterMenu
                :filters="filters"
                @apply-filters="applyFilters"
                @clear-filters="clearFilters"
              />

              <VBtn
                class="bg-background border border-grey-900"
                :disabled="isExporting.rating"
                height="45"
                :loading="isExporting.rating"
                variant="outlined"
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
                v-model:page="currentPage"
                class="elevation-0"
                :headers="headers"
                :hide-default-footer="true"
                :items="users"
                :items-per-page="perPage"
                :loading="initialLoading"
                :total-items="totalItems"
              >
                <template #[`item.fullName`]="{ item }">
                  <div class="d-flex align-center gap-2">
                    <v-avatar v-if="item.profileImage" size="sm">
                      <v-img :src="item.profileImage" />
                    </v-avatar>
                    <User v-else />
                    <span>{{ item.fullName }}</span>
                  </div>
                </template>
                <template #[`item.userType`]="{ item }">
                  <v-chip
                    class="py-2 px-3"
                    color="success"
                    rounded="lg"
                    size="lg"
                  >
                    {{ item.userType?.name?.ar }}
                  </v-chip>
                </template>
                <template #[`item.workingDaysCount`]="{ item }">
                  {{
                    item.workingDaysCount
                      ? `${item.workingDaysCount} يوم`
                      : ""
                  }}
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
                <v-btn color="primary" variant="outlined" @click="fetchUsers">
                  إعادة المحاولة
                </v-btn>
              </div>

              <!-- Empty state -->
              <div
                v-else-if="!users.length && !isLoading && !fetchError"
                class="pa-8 text-center text-grey"
              >
                <p class="text-h6">لا يوجد مستخدمين</p>
                <p class="text-sm">لم يتم العثور على نتائج تطابق بحثك</p>
              </div>
            </v-col>
            <v-col v-if="users.length && !fetchError" cols="12">
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
    </div>
    <!-- show details dialog -->
    <UserDialogDetails
      v-model="detailsDialog"
      :is-user-details-loading="isUserDetailsLoading"
      :selected-user="selectedUser"
    />
  </v-container>
</template>
