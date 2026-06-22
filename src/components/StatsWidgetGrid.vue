<script setup lang="ts">
  import type { Component } from 'vue'
  import StatsWidget from '@/@core/components/widgets/StatsWidget.vue'

  interface WidgetItem {
    title: string
    value: number
    icon: Component
  }

  defineProps<{
    isLoading: boolean
    widgets: WidgetItem[]
  }>()

  const colClass = (count: number) => {
    if (count <= 3) return 'cols-12 col-md-4'
    return 'cols-6 col-lg-3 col-md-3'
  }
</script>

<template>
  <v-row v-if="isLoading" class="mx-md-0">
    <v-col
      v-for="item in widgets.length || 4"
      :key="item"
      cols="6"
      md="3"
      sm="6"
    >
      <VCard class="py-2" elevation="0">
        <v-card-text class="px-1 py-2">
          <v-skeleton-loader
            class="mx-auto"
            elevation="0"
            max-width="400"
            type="list-item-two-line, table-heading"
          />
        </v-card-text>
      </VCard>
    </v-col>
  </v-row>

  <v-row v-else-if="widgets.length" class="mx-md-0">
    <v-col
      v-for="(widget, idx) in widgets"
      :key="idx"
      :class="colClass(widgets.length)"
    >
      <StatsWidget :number="widget.value" :title="widget.title">
        <template #icon>
          <component :is="widget.icon" />
        </template>
      </StatsWidget>
    </v-col>
  </v-row>
  <div v-else class="pa-5 text-center text-grey">
    لا توجد بيانات متاحة
  </div>
</template>
