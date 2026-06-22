<script setup lang="ts" generic="T">

  const props = defineProps<{
    items: T[]
    loading: boolean
    page: number
    itemsPerPage: number
    totalItems: number
    headers: { title: string; key?: string; value?: string; [key: string]: unknown }[]
  }>()

  const emit = defineEmits<{ 'update:options': [options: Record<string, unknown>] }>()

  const handleTableUpdate = (options: Record<string, unknown>) => {
    emit('update:options', options)
  }
</script>

<template>
  <v-card elevation="0" rounded="lg">
    <v-data-table
      :headers="headers"
      hide-default-footer
      :items="items"
      :items-per-page="itemsPerPage"
      :loading="loading"
      :page="page"
      :total-items="totalItems"
      @update:options="handleTableUpdate"
    >
      <template v-for="(_, slot) in $slots" #[slot]="slotProps">
        <slot :name="slot" v-bind="slotProps" />
      </template>

      <template #bottom>
        <slot name="pagination" />
      </template>
    </v-data-table>
  </v-card>
</template>

<style lang="scss" scoped>
table {
  padding-inline: 10px !important;
  background: rgb(var(--v-theme-surface)) !important;
}
:deep(.v-data-table) {
  .v-table,
  .v-table__wrapper {
    background: rgb(var(--v-theme-surface));
  }

  .v-data-table-footer {
    background: transparent;
  }

  .v-data-table__tr {
    margin-bottom: 16px;
    border-radius: 16px;
    border: none !important;
  }

  .v-data-table__td {
    background: rgb(var(--v-theme-background)) !important;
    height: 72px !important;
    color: rgb(var(--v-theme-on-background));
    border: none !important;

    &:first-child {
      border-top-right-radius: 16px;
      border-bottom-right-radius: 16px;
    }

    &:last-child {
      border-top-left-radius: 16px;
      border-bottom-left-radius: 16px;
    }
  }
  .v-data-table__th {
    color: rgba(var(--v-theme-on-surface), 0.6) !important;
    background-color: transparent !important;
  }
}

:deep(.v-table > .v-table__wrapper > table) {
  border-spacing: 0 10px !important;
  border-collapse: separate !important;
}

</style>
