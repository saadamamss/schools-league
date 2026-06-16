import { authService } from "@/services/auth.service";

export const hasPermission = (permissionLabel, hasSome = false) => {
  const userPermissions = authService.getStoredUser()?.user?.permissions || [];

  if (!userPermissions) return false;

  if (Array.isArray(permissionLabel) && permissionLabel.length) {
    if (hasSome) {
      return permissionLabel.some((permission) =>
        userPermissions?.includes(permission)
      );
    }

    return permissionLabel.every((permission) =>
      userPermissions?.includes(permission)
    );
  }

  return userPermissions?.includes(permissionLabel);
};

export const canAccessObserverInfo = () => {
  const user = authService.getStoredUser()?.user || null;

  return Array.isArray(user?.parent_companies)
    ? user.parent_companies.length === 0
    : true;
};

export const hasRole = (roleId) => {
  const user = authService.getStoredUser()?.user || null;

  return user ? user.role?.id == roleId : false;
};
