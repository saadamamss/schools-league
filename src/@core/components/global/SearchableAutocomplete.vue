<script setup lang="ts">
  import axiosIns from '@/plugins/axios'
  import { useDebounceFn } from '@vueuse/core'

  interface SearchableItem {
    id: number | string
    [key: string]: any
  }

  const props = withDefaults(defineProps<{
    searchUrl?: string
    items?: SearchableItem[]
    selectedItems?: SearchableItem[]
    itemTitleConcat?: string[]
    responseDataKey?: string
  }>(), {
    searchUrl: '',
    items: () => [],
    selectedItems: () => [],
    itemTitleConcat: () => [],
    responseDataKey: '',
  })

  const searchedItems = ref<SearchableItem[]>([])
  const isSearching = ref(false)
  const selectedItemIds = ref<(string | number)[]>([])
  const storedSelectedItems = ref<SearchableItem[]>([])

  watch(
    () => props.items,
    items => {
      if (items?.length) {
        searchedItems.value = items
      }
    },
    { deep: true }
  )

  watch(
    () => selectedItemIds.value,
    items => {
      if (items?.length) {
        storedSelectedItems.value = searchedItems.value.filter(item =>
          items.includes(item.id)
        )
      }
    },
    { deep: true }
  )

  const isSelectedItemsAssigned = ref(false)

  const searchItems = async (keywords: string) => {
    if (!props.searchUrl) return

    try {
      isSearching.value = true
      const { data } = await axiosIns.get(props.searchUrl, {
        params: { search: keywords },
      })

      if (!isSelectedItemsAssigned.value && props.selectedItems?.length) {
        storedSelectedItems.value = props.selectedItems
        selectedItemIds.value = props.selectedItems.map(item => item.id)
        isSelectedItemsAssigned.value = true
      }

      const responseDataItems = props.responseDataKey
        ? data?.data[props.responseDataKey] || []
        : data?.data || []

      const itemsData = responseDataItems
        .map((item: Record<string, any>) => {
          if (props.itemTitleConcat.length) {
            return {
              ...item,
              label: props.itemTitleConcat.map(key => item[key]).join(' '),
            }
          }
          return item
        })
        .filter((item: Record<string, any>) => !selectedItemIds.value.includes(item.id))

      searchedItems.value = [...storedSelectedItems.value, ...itemsData]
    } catch {
      // silently fail
    } finally {
      isSearching.value = false
    }
  }

  const startSearch = useDebounceFn(keywords => {
    searchItems(keywords)
  }, 500)

  onMounted(() => {
    if (props.searchUrl) {
      searchItems('')
    }
  })
</script>

<template>
  <v-autocomplete
    bg-color="rgba(var(--v-theme-on-surface), 0.05)"
    class="custom-select"
    clearable
    hide-details
    hide-no-data
    item-title="label"
    item-value="value"
    :items="searchedItems"
    :loading="isSearching"
    :menu-icon="''"
    min-width="150"
    open-on-clear
    return-object
    variant="filled"
    @update:model-value="(e: any) => { if (e) selectedItemIds = e }"
    @update:search="startSearch"
  >
    <template #append-inner>
      <svg
        fill="none"
        height="8"
        viewBox="0 0 12 8"
        width="12"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M11.4937 0.82937C11.8643 1.1023 11.9434 1.62397 11.6705 1.99454C11.4585 2.28238 11.2465 2.556 11.0605 2.7943C10.6892 3.27002 10.1781 3.90713 9.62282 4.54668C9.07112 5.1821 8.45943 5.83898 7.88835 6.34356C7.60377 6.59501 7.30964 6.82626 7.02283 6.99916C6.7589 7.15827 6.39624 7.33365 5.99946 7.33366C5.60269 7.33365 5.24 7.15827 4.97607 6.99916C4.68926 6.82626 4.39513 6.59501 4.11055 6.34356C3.53947 5.83898 2.92778 5.1821 2.37608 4.54668C1.82079 3.90713 1.30968 3.27003 0.93842 2.7943C0.752445 2.556 0.540406 2.28238 0.328413 1.99454C0.0554839 1.62397 0.134644 1.1023 0.505222 0.829371C0.654187 0.719658 0.827573 0.666849 0.999404 0.666993L10.9995 0.666992C11.1713 0.666848 11.3447 0.719658 11.4937 0.82937Z"
          fill="#9EA1A3"
          opacity="0.4"
        />
        <path
          d="M11.4934 0.82937C11.864 1.1023 11.9431 1.62397 11.6702 1.99454C11.4582 2.28238 11.2462 2.556 11.0602 2.7943C10.6889 3.27002 10.1778 3.90713 9.62254 4.54668C9.07084 5.1821 8.45915 5.83898 7.88807 6.34356C7.60349 6.59501 7.30936 6.82626 7.02256 6.99916C6.75862 7.15827 6.39596 7.33365 5.99919 7.33366C5.60241 7.33365 5.23972 7.15827 4.97579 6.99916C4.68898 6.82626 4.39485 6.59501 4.11027 6.34356C3.62491 5.91471 3.1102 5.37585 2.62793 4.83366L6.83261 0.666993L10.9992 0.666992C11.171 0.666848 11.3444 0.719658 11.4934 0.82937Z"
          fill="#9EA1A3"
        />
      </svg>
    </template>
  </v-autocomplete>
</template>
