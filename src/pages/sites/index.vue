<script setup lang="ts">
  import { useAppStore } from '@/stores/app'
  import Magnifier from '@/components/icons/magnifier.vue'
  import DataTable from '@/components/DataTable.vue'
  import Pagination from '@/@core/components/Pagination.vue'
  import Eyeicon from '@/components/icons/eyeicon.vue'
  import SitesWidgets from '@/components/SitesWidgets.vue'
  import SiteDetailsDialog from '@/components/SiteDetailsDialog.vue'
  import { locationsApi } from '@/api'
  import { watchDebounced } from '@vueuse/core'
  import type { Location, SitesWidgetData } from '@/types/models'

  const appStore = useAppStore()
  const isLoading = ref(false)
  const fetchError = ref(false)
  const perPage = ref(5)
  const totalItems = ref(0)
  const currentPage = ref(1)
  const search = ref('')
  const detailsDialog = ref(false)
  const selectedSiteId = ref(null)
  const headers = [
    { title: 'اسم الموقع', value: 'name', align: 'start' },
    { title: 'نوع الموقع', value: 'type' },
    { title: 'رقم تصريح المسكن', value: 'license' },
    { title: 'رقم تواصل المفوض', value: 'phone' },
    { title: 'عدد الموظفين المرتبطين', value: 'joinedEmployee' },
    { title: 'ساعات العمل', value: 'workHours' },
    { title: '', value: 'actions' },
  ]

  const sites = ref<Location[]>([])
  const widgetsData = ref<SitesWidgetData | null>(null)

  watchDebounced(search, () => { fetchSites() }, { debounce: 500 })

  const fetchSites = async () => {
    isLoading.value = true
    fetchError.value = false
    try {
      const response = await locationsApi.getList(
        { per_page: perPage.value, page: currentPage.value, search: search.value || undefined }
      )
      if (response.data?.data) {
        sites.value = response.data.data
        perPage.value = response.data?.pagination?.iPerPage ?? perPage.value
        totalItems.value = response.data?.pagination?.iTotalObjects ?? 0
        currentPage.value = response.data?.pagination?.iCurrentPage ?? 1
        widgetsData.value = (response.data?.statistics ?? null) as unknown as SitesWidgetData
      }
    } catch (err: any) {
      fetchError.value = true
      appStore.showSnackbar({
        message: err.response?.data?.message || 'حدث خطأ فى جلب بيانات الجدول',
        color: 'error',
      })
    } finally {
      isLoading.value = false
    }
  }

  const showDetails = (site: Record<string, any>) => {
    selectedSiteId.value = site.id
    detailsDialog.value = true
  }
  const closeDetails = () => {
    detailsDialog.value = false
  }

  watch(currentPage, () => { fetchSites() })
  watch(perPage, () => { currentPage.value = 1; fetchSites() })

  onMounted(() => { fetchSites() })
</script>

<template>
  <v-container class="px-0" fluid>
    <!-- Add SitesWidgets component -->
    <SitesWidgets
      :data="widgetsData"
      :is-loding="isLoading && !widgetsData"
      :sites="sites"
    />

    <v-container class="px-0 px-sm-4 px-xl-0 py-0 mt-4" fluid>
      <VCard class="rounded-lg">
        <VCardTitle class="py-4">
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between ga-4">
            <h2 class="text-h5 font-weight-bold">قائمة المواقع</h2>

            <!-- Search -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-2"
              style="flex-grow: 1; min-width: 0"
            >
              <v-text-field
                v-model="search"
                class="bg-primary_2 border-grey-light bg-white flex-grow-1"
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
                  'background-color': 'rgb(var(--v-theme-surface))',
                }"
              >
                <template #prepend-inner>
                  <Magnifier />
                </template>
              </v-text-field>
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
                :items="sites"
                :items-per-page="perPage"
                :loading="isLoading"
                :total-items="totalItems"
              >
                <template #[`item.joinedEmployee`]="{ item }">
                  <span> {{ item.employeeNumber }} موظف </span> -
                  {{ item.observerNumber }} مراقب
                </template>

                <template #[`item.workHours`]="{ item }">
                  {{ item.workHours }} ساعة هذا الشهر
                </template>
                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      class="text-sm rounded-lg"
                      color="rgba(var(--v-theme-light-gray))"
                      size="small"
                      style="color: var(--v-theme-on-background)"
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
                <v-btn color="primary" variant="outlined" @click="fetchSites">
                  إعادة المحاولة
                </v-btn>
              </div>

              <!-- Empty state -->
              <div v-else-if="!sites.length && !isLoading && !fetchError" class="pa-8 text-center text-grey">
                <p class="text-h6">لا توجد مواقع</p>
                <p class="text-sm">لم يتم العثور على نتائج تطابق بحثك</p>
              </div>

              <v-col v-if="sites.length && !fetchError" cols="12">
                <!-- Pagination -->
                <Pagination
                  v-model:page="currentPage"
                  v-model:per-page="perPage"
                  :total-items="totalItems"
                />
              </v-col>
            </v-col>
          </v-row>
        </VCardText>
      </VCard>
    </v-container>

    <SiteDetailsDialog v-if="selectedSiteId" v-model="detailsDialog" :site-id="selectedSiteId" @close="closeDetails" />
  </v-container>
</template>
