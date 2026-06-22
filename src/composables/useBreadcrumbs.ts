import { computed } from 'vue'
import { useRoute } from 'vue-router'

export const useBreadcrumbs = () => {
  const route = useRoute()

  const baseHomeRoute = {
    title: 'الرئيسية',
    href: '/',
    disabled: false,
  }

  const generateBreadcrumbItems = (customItems: Array<{ title: string; href: string; disabled: boolean }> = []) => {
    const items = [baseHomeRoute]

    if (customItems.length > 0) {
      items.push(...customItems)
    } else {
      const pathSegments = route.path.split('/').filter(segment => segment)

      let currentPath = ''
      pathSegments.forEach((segment, index) => {
        currentPath += `/${segment}`

        if (segment.startsWith(':')) return

        items.push({
          title: segment.charAt(0)?.toUpperCase() + segment.slice(1).replace(/-/g, ' '),
          href: currentPath,
          disabled: index === pathSegments.length - 1,
        })
      })
    }

    return items
  }

  const breadcrumbItems = computed(() => generateBreadcrumbItems())

  return {
    generateBreadcrumbItems,
    breadcrumbItems,
  }
}
