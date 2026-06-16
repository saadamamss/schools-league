import { authService } from "@/services/auth.service";
export const userTypes = {
  operations_manager: [
    "home",
    "users",
    "manage-events",
    "attendances",
    "profile",
  ],
  project_manager: ["home", "users", "manage-events", "attendances", "profile"],
  supervisor: ["home", "users", "manage-events", "attendances", "profile"],
  protocol: ["home", "users", "manage-events", "attendances", "profile"],
  //
  organizer: ["home", "profile", "logout"],
};

export const userHasRoutePermission = (route) => {
  const user = authService.getStoredUser();
  console.log(user);
  
  return userTypes[user?.user.user_type?.code]?.includes(route);
};
