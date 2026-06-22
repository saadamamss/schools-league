import type { Component } from 'vue'
import Attendance from '@/components/icons/airplane.vue'
import HomeIcon from '@/components/icons/home.vue'
import Realstate from '@/components/icons/realstate.vue'
import Usergroup from '@/components/icons/usergroup.vue'

interface NavItem {
  title: string
  to: { name: string }
  icon: Component
  permission: boolean
}

const navigation: NavItem[] = [
  {
    title: 'الصفحة الرئيسية',
    to: { name: 'home' },
    icon: HomeIcon,
    permission: true,
  },
  {
    title: 'المستخدمون',
    to: { name: 'users' },
    icon: Usergroup,
    permission: true,
  },
  {
    title: 'المواقع',
    to: { name: 'manage-locations' },
    icon: Realstate,
    permission: true,
  },
  {
    title: 'سجلات الحضور',
    to: { name: 'attendances' },
    icon: Attendance,
    permission: true,
  },
]

export default navigation
