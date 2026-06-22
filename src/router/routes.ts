import type { RouteRecordRaw } from 'vue-router'
import DashboardLayout from '@/layouts/DashboardLayout.vue'
import AuthLayout from '@/layouts/AuthLayout.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: DashboardLayout,
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/pages/home/index.vue'),
      },
      {
        path: '/manage-locations',
        name: 'manage-locations',
        component: () => import('@/pages/locations/index.vue'),
      },
      {
        path: '/attendances',
        name: 'attendances',
        component: () => import('@/pages/attendances/index.vue'),
      },
      {
        path: '/users',
        name: 'users',
        component: () => import('@/pages/users/users.vue'),
      },
      {
        path: '/profile',
        name: 'profile',
        component: () => import('@/pages/profile/index.vue'),
      },
    ],
  },
  {
    path: '/auth',
    component: AuthLayout,
    meta: {
      requiresAuth: false,
      guest: true,
    },
    children: [
      {
        path: 'login',
        name: 'login',
        component: () => import('@/pages/auth/login.vue'),
      },
      {
        path: 'register',
        name: 'register',
        component: () => import('@/pages/auth/register.vue'),
      },
      {
        path: 'create-password',
        name: 'create-password',
        component: () => import('@/pages/auth/create-password.vue'),
      },
      {
        path: 'reset-password',
        name: 'reset-password',
        component: () => import('@/pages/auth/reset-password.vue'),
      },
      {
        path: 'forgot-password',
        name: 'forgot-password',
        component: () => import('@/pages/auth/forgot-password.vue'),
      },
      {
        path: 'otp',
        name: 'otp',
        component: () => import('@/pages/auth/otp.vue'),
      },
      {
        path: 'verify',
        name: 'VerifyAccount',
        component: () => import('@/pages/auth/verify-account.vue'),
      },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/pages/NotFound.vue'),
  },
]

export default routes
