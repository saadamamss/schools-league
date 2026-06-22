<template>
  <v-menu v-model="filterMenu" :close-on-content-click="false" location="end">
    <template #activator="{ props: activatorProps }">
      <v-btn
        v-bind="activatorProps"
        class="bg-background border border-grey-900"
        color="primary"
        height="45"
        variant="outlined"
      >
        <v-icon>
          <FilterIcon />
        </v-icon>
        <v-badge
          v-if="activeFiltersCount"
          class="position-absolute"
          color="primary"
          :content="activeFiltersCount"
          style="top: 0px; left: 0px"
        />
      </v-btn>
    </template>

    <v-card class="border bg-app-surface" min-width="350">
      <v-card-title class="mb-4"> خيارات التصفية </v-card-title>
      <v-card-text>
        <div v-for="(filter, i) in filters" :key="i" class="mb-5">
          {{ filterSearchValues[filter.key] }}
          <v-text-field
            v-if="filter.type == 'text-field'"
            v-model="filterValues[filter.key]"
            clearable
            density="compact"
            :label="filter.label"
            :placeholder="filter.label"
            rounded="pill"
          />
          <v-select
            v-else-if="filter.type == 'select'"
            v-model="filterValues[filter.key]"
            density="compact"
            :item-title="filter.itemTitle"
            :item-value="filter.itemValue"
            :items="filter.items"
            :label="filter.label"
            :placeholder="filter.label"
            rounded="pill"
          />
          <!-- :search="filterSearchValues[filter.key]" -->
          <v-autocomplete
            v-else-if="filter.type == 'searchable'"
            v-model="filterValues[filter.key]"
            clearable
            :item-title="filter.itemTitle"
            :item-value="filter.itemValue"
            :items="filter.items"
            :label="filter.label"
            no-data-text="لا توجد بيانات!"
            rounded="pill"
            @update:search="onchange($event, filter)"
          />
          <v-text-field
            v-else-if="filter.type == 'date'"
            v-model="filterValues[filter.key]"
            dir="rtl"
            label="تاريخ الأنشاء"
            rounded="pill"
            type="date"
          />
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn color="error" variant="text" @click="clearFilters">
          مسح الفلاتر
        </v-btn>
        <v-btn color="primary" variant="text" @click="applyFilters">
          تطبيق
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>
<script setup lang="ts">
  import { ref } from 'vue'
  import FilterIcon from '@/components/icons/filter.vue'
  import { useDebounceFn } from '@vueuse/core'
  const filterMenu = ref(false)
  const filterValues = ref<Record<string, any>>({})
  const menu = ref(false)
  interface Filter {
    key: string
    type: string
    label: string
    items?: any[]
    itemTitle?: string
    itemValue?: string
    searchTrigger?: (val: string) => void
  }
  const props = withDefaults(defineProps<{
    filters: Filter[]
  }>(), {
    filters: () => [],
  })
  const filterSearchValues = ref<Record<string, any>>({})
  const emit = defineEmits<{
    (e: 'applyFilters', val: Record<string, any>): void
    (e: 'clearFilters'): void
  }>()
  const activeFiltersCount = computed(() => {
    return Object.values(filterValues.value).filter(v => !!v).length
  })

  const applyFilters = () => {
    filterMenu.value = false
    emit('applyFilters', filterValues.value)
  }
  const clearFilters = () => {
    filterValues.value = {}
    emit('clearFilters')
  }
  const onchange = (val: string, filter: Filter) => {
    filter.searchTrigger?.(val)
  }
</script>
