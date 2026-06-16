<template>
  <v-menu v-model="filterMenu" :close-on-content-click="false" location="end">
    <template v-slot:activator="{ props }">
      <v-btn
        v-bind="props"
        color="primary"
        variant="outlined"
        class="bg-background border border-grey-900"
        height="45"
      >
        <v-icon>
          <FilterIcon />
        </v-icon>
        <v-badge
          v-if="activeFiltersCount"
          :content="activeFiltersCount"
          color="primary"
          class="position-absolute"
          style="top: 0px; left: 0px"
        ></v-badge>
      </v-btn>
    </template>

    <v-card min-width="350" class="border bg-app-surface">
      <v-card-title class="mb-4"> خيارات التصفية </v-card-title>
      <v-card-text>
        <div v-for="(filter, i) in filters" :key="i" class="mb-5">
          {{ filterSearchValues[filter.key] }}
          <v-text-field
            v-if="filter.type == 'text-field'"
            v-model="filterValues[filter.key]"
            :placeholder="filter.label"
            :label="filter.label"
            density="compact"
            rounded="pill"
            clearable
          >
          </v-text-field>
          <v-select
            v-else-if="filter.type == 'select'"
            v-model="filterValues[filter.key]"
            :items="filter.items"
            :placeholder="filter.label"
            :label="filter.label"
            :item-value="filter.itemValue"
            :item-title="filter.itemTitle"
            density="compact"
            rounded="pill"
          />
          <!-- :search="filterSearchValues[filter.key]" -->
          <v-autocomplete
            v-else-if="filter.type == 'searchable'"
            v-model="filterValues[filter.key]"
            :items="filter.items"
            :item-title="filter.itemTitle"
            :item-value="filter.itemValue"
            :label="filter.label"
            no-data-text="لا توجد بيانات!"
            rounded="pill"
            clearable
            @update:search="onchange($event, filter)"
          />
          <v-text-field
            v-else-if="filter.type == 'date'"
            v-model="filterValues[filter.key]"
            type="date"
            label="تاريخ الأنشاء"
            rounded="pill"
            dir="rtl"
          >
          </v-text-field>
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
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
<script setup>
import { ref } from "vue";
import FilterIcon from "@/components/icons/filter.vue";
import { useDebounceFn } from "@vueuse/core";
const filterMenu = ref(false);
const filterValues = ref({});
const menu = ref(false);
// filters[{key , type}]
const props = defineProps({
  filters: {
    type: Array,
    default: [],
    required: true,
  },
});
const filterSearchValues = ref({});
const emit = defineEmits(["applyFilters", "clearFilters"]);
const activeFiltersCount = computed(() => {
  return Object.values(filterValues.value).filter((v) => !!v).length;
});

const applyFilters = () => {
  emit("applyFilters", filterValues.value);
};
const clearFilters = () => {
  filterValues.value = {};
  emit("clearFilters");
};
const onchange = (val, filter) => {
  filter.searchTrigger(val);
};
</script>
