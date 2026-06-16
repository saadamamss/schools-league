<script setup>
import { defineProps, defineEmits } from "vue";

const props = defineProps({
  items: {
    type: Array,
    required: true,
  },
  loading: {
    type: Boolean,
    default: false,
  },
  page: {
    type: Number,
    required: true,
  },
  itemsPerPage: {
    type: Number,
    required: true,
  },
  totalItems: {
    type: Number,
    required: true,
  },
  headers: {
    type: Array,
    required: true,
  },
});

const emit = defineEmits(["update:options"]);

const handleTableUpdate = (options) => {
  emit("update:options", options);
};
</script>

<template>
  <v-card elevation="0" rounded="lg">
    <v-data-table
      :headers="headers"
      :items="items"
      :items-per-page="itemsPerPage"
      :page="page"
      :total-items="totalItems"
      :loading="loading"
      hide-default-footer
      @update:options="handleTableUpdate"
    >
      <template v-for="(_, slot) in $slots" #[slot]="slotProps">
        <slot :name="slot" v-bind="slotProps" />
      </template>

      <template #bottom>
        <slot name="pagination"></slot>
      </template>
    </v-data-table>
  </v-card>
</template>

<style lang="scss" scoped>
table {
  padding-inline: 10px !important;
  background: #fff !important;
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
    color: #999 !important;
    background-color: transparent !important;
  }
}

:deep(.v-table > .v-table__wrapper > table) {
  border-spacing: 0 10px !important;
  border-collapse: separate !important;
}
.v-table .v-table__wrapper > table > tbody > tr:not(:last-child) > td,
.v-table .v-table__wrapper > table > tbody > tr:not(:last-child) > th {
  // border-bottom: 0px solid #e0e0e0 !important;
  // border: 1px solid rgb(209, 10, 209) !important;
}
</style>
