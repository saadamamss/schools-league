import { LOCAL_STORAGE_USER_KEY } from "@/config";

class AuthService {
  getStoredUser() {
    try {
      const userStr = localStorage.getItem(LOCAL_STORAGE_USER_KEY);
      return userStr ? JSON.parse(userStr) : null;
    } catch (error) {
      console.error("Error parsing stored user:", error);
      return null;
    }
  }

  getToken() {
    const user = this.getStoredUser();
    return user?.token || null;
  }

  setUser(user) {
    if (user) {
      localStorage.setItem(LOCAL_STORAGE_USER_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(LOCAL_STORAGE_USER_KEY);
    }
  }

  logout() {
    localStorage.removeItem(LOCAL_STORAGE_USER_KEY);
  }

  isAuthenticated() {
    return !!this.getToken();
  }
}

export const authService = new AuthService();
