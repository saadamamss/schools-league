<script setup lang="ts">
  import { useLayouts } from '@layouts'
  import { config as configRaw } from '@layouts/config'
  import { can } from '@layouts/plugins/casl'
  const config = configRaw as any

  const props = defineProps<{
    item: Record<string, any>
  }>()

  const { isVerticalNavMini, dynamicI18nProps } = useLayouts()
  const { width: windowWidth } = useWindowSize()
  const shallRenderIcon = isVerticalNavMini(windowWidth.value)
</script>

<template>
  <li
    v-if="can(item.action, item.subject)"
    class="nav-section-title"
  >
    <div class="title-wrapper">
      <Transition
        mode="out-in"
        name="vertical-nav-section-title"
      >
        <!-- eslint-disable vue/no-v-text-v-html-on-component -->
        <Component
          :is="shallRenderIcon ? config.app.iconRenderer : config.app.enableI18n ? 'i18n-t' : 'span'"
          :key="shallRenderIcon"
          :class="shallRenderIcon ? 'placeholder-icon' : 'title-text'"
          v-bind="{ ...config.icons.sectionTitlePlaceholder, ...dynamicI18nProps(item.heading, 'span') }"
          v-text="!shallRenderIcon ? item.heading : null"
        />
        <!-- eslint-enable vue/no-v-text-v-html-on-component -->
      </Transition>
    </div>
  </li>
</template>
