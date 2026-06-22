export interface NavItem {
  title: string
  icon?: string
  to?: string
  badgeContent?: string
  badgeClass?: string
  permission?: string | string[]
  children?: NavItem[]
  heading?: string
}
