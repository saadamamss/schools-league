import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'

export const useBreadcrumbs = () => {
  const route = useRoute()

  // Base home route that will be present in all breadcrumbs
  const baseHomeRoute = {
    title: 'الرئيسية',
    href: '/',
    disabled: false
  }

  // Function to generate breadcrumb items based on current route
  const generateBreadcrumbItems = (customItems = []) => {
    const items = [baseHomeRoute]

    if (customItems.length > 0) {
      items.push(...customItems)
    } else {
      // Generate from route if no custom items provided
      const pathSegments = route.path.split('/').filter(segment => segment)

      let currentPath = ''
      pathSegments.forEach((segment, index) => {
        currentPath += `/${segment}`

        // Skip adding breadcrumb for the last segment if it's a dynamic parameter
        if (segment.startsWith(':')) return

        items.push({
          title: segment.charAt(0)?.toUpperCase() + segment.slice(1).replace(/-/g, ' '),
          href: currentPath,
          disabled: index === pathSegments.length - 1
        })
      })
    }

    return items
  }

  // Computed breadcrumb items
  const breadcrumbItems = computed(() => generateBreadcrumbItems())

  return {
    generateBreadcrumbItems,
    breadcrumbItems
  }
}
