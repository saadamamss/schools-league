import DashboardLayout from "@/layouts/DashboardLayout";
import LoginPage from "@/pages/auth/login";
import RegisterPage from "@/pages/auth/register";
import CreatePasswordPage from "@/pages/auth/create-password";
import ForgotPasswordPage from "@/pages/auth/forgot-password";
import ResetPasswordPage from "@/pages/auth/reset-password.vue";
import OTPPage from "@/pages/auth/otp";
import VerifyPage from "@/pages/auth/verify-account.vue";
import DashboardHome from "@/pages/home";
import UsersPage from "@/pages/users/users.vue";
import ProfilePage from "@/pages/profile/index.vue";
import NotFoundPage from "@/pages/NotFound";
import AuthLayout from "@/layouts/AuthLayout.vue";

// 
export default [
  {
    path: "/",
    component: DashboardLayout,
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        path: "",
        name: "home",
        component: DashboardHome,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/manage-events",
        name: "manage-events",
        component: () => import("@/pages/events/index.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
        },
      },
      {
        path: "/attendances",
        name: "attendances",
        component: () => import("@/pages/attendances/index.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
        },
      },
      {
        path: "/users",
        name: "users",
        component: UsersPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/profile",
        name: "profile",
        component: ProfilePage,
        meta: {
          requiresAuth: true,
        },
      },
    ],
  },
  {
    path: "/auth",
    component: AuthLayout,
    meta: {
      requiresAuth: false,
      requiresPermission: false,
    },
    children: [
    {
      path: "",  // matches /auth
      redirect: "auth/login",  // redirects to /auth/login
    },
      {
        path: "login",
        name: "login",
        component: LoginPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "register",
        name: "register",
        component: RegisterPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "create-password",
        name: "create-password",
        component: CreatePasswordPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "reset-password",
        name: "reset-password",
        component: ResetPasswordPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "forgot-password",
        name: "forgot-password",
        component: ForgotPasswordPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "otp",
        name: "otp",
        component: OTPPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "verify",
        name: "VerifyAccount",
        component: VerifyPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
    ],
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFoundPage,
  },
];
