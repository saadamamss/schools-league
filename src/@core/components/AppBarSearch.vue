<script setup lang="ts">
  import PerfectScrollbar from 'vue3-perfect-scrollbar'
  import { VList, VListItem, VListSubheader } from 'vuetify/components'

  interface SearchItem {
    title: string
    icon?: string
    customIcon?: string
    header?: string
    content?: SearchItem[]
  }

  const props = defineProps<{
    isDialogVisible: boolean
    searchQuery: string
    searchResults: SearchItem[]
    suggestions?: SearchItem[]
    noDataSuggestion?: SearchItem[]
  }>()

  const emit = defineEmits<{
    (e: 'update:isDialogVisible', val: boolean): void
    (e: 'update:searchQuery', val: string): void
    (e: 'itemSelected', item: SearchItem): void
  }>()

  const { ctrlK, metaK } = useMagicKeys()
  const refSearchList = ref()
  const searchQuery = ref(structuredClone(toRaw(props.searchQuery)))
  const refSearchInput = ref()
  const isLocalDialogVisible = ref(structuredClone(toRaw(props.isDialogVisible)))
  const searchResults = ref(structuredClone(toRaw(props.searchResults)))

  // 👉 Watching props change
  watch(props, () => {
    isLocalDialogVisible.value = structuredClone(toRaw(props.isDialogVisible))
    searchResults.value = structuredClone(toRaw(props.searchResults))
    searchQuery.value = structuredClone(toRaw(props.searchQuery))
  })
  watch([ctrlK, metaK], () => {
    isLocalDialogVisible.value = true
    emit('update:isDialogVisible', true)
  })

  // 👉 clear search result and close the dialog
  const clearSearchAndCloseDialog = () => {
    emit('update:isDialogVisible', false)
    emit('update:searchQuery', '')
  }

  watchEffect(() => {
    if (!searchQuery.value.length) searchResults.value = []
  })

  const getFocusOnSearchList = (e: KeyboardEvent) => {
    if (e.key === 'ArrowDown') {
      e.preventDefault()
      refSearchList.value?.focus('next')
    } else if (e.key === 'ArrowUp') {
      e.preventDefault()
      refSearchList.value?.focus('prev')
    }
  }

  const dialogModelValueUpdate = (val: boolean) => {
    emit('update:isDialogVisible', val)
    emit('update:searchQuery', '')
  }

  const resolveCategories = (val: string) => {
    if (val === 'dashboards') return 'Dashboards'
    if (val === 'appsPages') return 'Apps & Pages'
    if (val === 'userInterface') return 'User Interface'
    if (val === 'formsTables') return 'Forms Tables'
    if (val === 'chartsMisc') return 'Charts Misc'

    return 'Misc'
  }
</script>

<template>
  <VDialog
    class="app-bar-search-dialog"
    :fullscreen="$vuetify.display.width < 600"
    :height="$vuetify.display.smAndUp ? '550' : '100%'"
    max-width="600"
    :model-value="isLocalDialogVisible"
    @keyup.esc="clearSearchAndCloseDialog"
    @update:model-value="dialogModelValueUpdate"
  >
    <VCard class="position-relative" height="100%" width="100%">
      <VCardText class="pt-1" style="max-height: 65px">
        <!-- 👉 Search Input -->
        <VTextField
          ref="refSearchInput"
          v-model="searchQuery"
          autofocus
          class="app-bar-autocomplete-box"
          density="comfortable"
          variant="plain"
          @keydown="getFocusOnSearchList"
          @keyup.esc="clearSearchAndCloseDialog"
          @update:model-value="$emit('update:searchQuery', searchQuery)"
        >
          <!-- 👉 Prepend Inner -->
          <template #prepend-inner>
            <VBtn
              class="text-high-emphasis ms-n1"
              color="default"
              icon
              size="x-small"
              variant="text"
            >
              <VIcon icon="mdi-magnify" size="22" />
            </VBtn>
          </template>

          <!-- 👉 Append Inner -->
          <template #append-inner>
            <div class="d-flex align-center">
              <div
                class="text-base text-disabled cursor-pointer me-2"
                @click="clearSearchAndCloseDialog"
              >
                [esc]
              </div>

              <VBtn
                color="default"
                icon
                size="x-small"
                variant="text"
                @click="clearSearchAndCloseDialog"
              >
                <VIcon icon="mdi-close" size="22" />
              </VBtn>
            </div>
          </template>
        </VTextField>
      </VCardText>

      <!-- 👉 Divider -->
      <VDivider />

      <!-- 👉 Perfect Scrollbar -->
      <PerfectScrollbar
        class="h-100"
        :options="{ wheelPropagation: false, suppressScrollX: true }"
      >
        <!-- 👉 Search List -->
        <VList
          v-show="searchQuery.length && !!searchResults.length"
          ref="refSearchList"
          class="app-bar-search-list"
          density="compact"
        >
          <!-- 👉 list Item /List Sub header -->
          <template v-for="item in searchResults" :key="item.title">
            <VListSubheader v-if="'header' in item" class="text-disabled">
              {{ resolveCategories(item.title) }}
            </VListSubheader>

            <template v-else>
              <slot :item="item" name="searchResult">
                <VListItem link @click="$emit('itemSelected', item)">
                  <template #prepend>
                    <div class="d-flex align-center">
                      <div class="me-3">
                        <VIcon
                          v-if="!item.customIcon"
                          :icon="item.icon"
                          size="20"
                        />
                        <span
                          v-else
                          class="custom-icon"
                          v-html="item.customIcon"
                        />
                      </div>
                    </div>
                  </template>

                  <template #append>
                    <VIcon
                      class="enter-icon text-disabled"
                      icon="mdi-corner-down-left"
                      size="20"
                    />
                  </template>

                  <VListItemTitle>
                    {{ item.title }}
                  </VListItemTitle>
                </VListItem>
              </slot>
            </template>
          </template>
        </VList>

        <!-- 👉 Suggestions -->
        <div v-show="!!searchResults && !searchQuery" class="h-100">
          <slot name="suggestions">
            <VCardText class="app-bar-search-suggestions h-100 pa-10">
              <VRow v-if="props.suggestions" class="gap-y-4">
                <VCol
                  v-for="suggestion in props.suggestions"
                  :key="suggestion.title"
                  class="ps-6"
                  cols="12"
                  sm="6"
                >
                  <p class="text-xs text-disabled text-uppercase">
                    {{ suggestion.title }}
                  </p>

                  <VList class="card-list">
                    <VListItem
                      v-for="item in suggestion.content"
                      :key="item.title"
                      class="app-bar-search-suggestion"
                      link
                      :title="item.title"
                      @click="$emit('itemSelected', item)"
                    >
                      <template #prepend>
                        <div class="d-flex align-center">
                          <div class="me-3">
                            <VIcon
                              v-if="!item.customIcon"
                              :icon="item.icon"
                              size="20"
                            />
                            <span
                              v-else
                              class="custom-icon"
                              v-html="item.customIcon"
                            />
                          </div>
                        </div>
                      </template>
                    </VListItem>
                  </VList>
                </VCol>
              </VRow>
            </VCardText>
          </slot>
        </div>

        <!-- 👉 No Data found -->
        <div v-show="!searchResults.length && searchQuery.length" class="h-100">
          <slot name="noData">
            <VCardText class="h-100">
              <div
                class="app-bar-search-suggestions d-flex flex-column align-center justify-center text-high-emphasis h-100"
              >
                <VIcon icon="mdi-file-remove-outline" size="75" />
                <h6 class="text-h6 my-3">No Result For "{{ searchQuery }}"</h6>
                <div v-if="props.noDataSuggestion" class="mt-8">
                  <span class="d-flex justify-center text-disabled">Try searching for</span>
                  <h6
                    v-for="suggestion in props.noDataSuggestion"
                    :key="suggestion.title"
                    class="app-bar-search-suggestion text-sm font-weight-regular cursor-pointer mt-3"
                    @click="$emit('itemSelected', suggestion)"
                  >
                    <div class="d-flex align-center">
                      <div class="me-3">
                        <VIcon
                          v-if="!suggestion.customIcon"
                          :icon="suggestion.icon"
                          size="20"
                        />
                        <span
                          v-else
                          class="custom-icon"
                          v-html="suggestion.customIcon"
                        />
                      </div>
                    </div>
                    <span class="text-sm">{{ suggestion.title }}</span>
                  </h6>
                </div>
              </div>
            </VCardText>
          </slot>
        </div>
      </PerfectScrollbar>
    </VCard>
  </VDialog>
</template>

<style lang="scss">
.app-bar-search-suggestions {
  .app-bar-search-suggestion {
    &:hover {
      color: rgb(var(--v-theme-primary));
    }
  }
}

.app-bar-autocomplete-box {
  .v-field__input {
    padding-block-end: 0.425rem;
    padding-block-start: 0.9375rem;
  }
}

.app-bar-search-dialog {
  .v-list-item-title {
    font-size: 0.875rem !important;
  }

  .app-bar-search-list {
    .v-list-item,
    .v-list-subheader {
      font-size: 0.75rem;
      padding-inline: 1.5rem !important;
    }

    .v-list-item {
      .v-list-item__append {
        .enter-icon {
          visibility: hidden;
        }
      }

      &:hover,
      &:active,
      &:focus {
        .v-list-item__append {
          .enter-icon {
            visibility: visible;
          }
        }
      }
    }

    .v-list-subheader {
      line-height: 1;
      min-block-size: auto;
      padding-block: 0.6875rem 0.3125rem;
      text-transform: uppercase;
    }
  }
}

.custom-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
}

.custom-icon svg {
  width: 20px;
  height: 20px;
}
</style>

<style lang="scss" scoped>
.card-list {
  --v-card-list-gap: 16px;
}
</style>
