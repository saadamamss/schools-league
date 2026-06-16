import Attendance from "@/components/icons/airplane.vue";
import HomeIcon from "@/components/icons/home.vue";
import Realstate from "@/components/icons/realstate.vue";
import Usergroup from "@/components/icons/usergroup.vue";
export default [
  {
    title: "الصفحة الرئيسية",
    to: { name: "home" },
    icon: HomeIcon,
    permission: true,
  },
  {
    title: "المستخدمون",
    to: { name: "users" },
    icon: Usergroup,
    permission: true,
  },
  {
    title: "الفعاليات",
    to: { name: "manage-events" },
    icon: Realstate,
    permission: true,
  },
  {
    title: "سجلات الحضور",
    to: { name: "attendances" },
    icon: Attendance,
    permission: true,
  },
];
