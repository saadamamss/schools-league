import { defineStore } from "pinia";
import { LOCAL_STORAGE_USER_KEY } from "@/config";
import Cookies from "js-cookie";
import {
  getFromStorage,
  saveToStorage,
  removeFromStorage,
} from "@/@core/utils/helpers";
import { authService } from "@/services/auth.service";
import axiosIns from "@/plugins/axios";
import axios from "axios";

const apiBase = "https://api.maidan.events/api";
export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: authService.getToken(),
    user: getFromStorage(LOCAL_STORAGE_USER_KEY),
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    setToken(token) {
      this.token = token;
      const currentUser = authService.getStoredUser() || {};
      authService.setUser({ ...currentUser, token });
    },

    setUser(user) {
      this.user = user;
      authService.setUser(user);
    },

    async logout() {
      try {
        await axiosIns.post("/auth/logout");
      } finally {
        this.token = null;
        this.user = null;
        authService.logout();
      }
    },

    async login(credentials) {
      try {
        const response = await axiosIns.post("auth/login", credentials);
        const data = response?.data;

        console.log(data);
        
        if (!data) {
          throw new Error("No data received from server");
        }

        // Handle both possible response structures
        const token = data.token || data.data?.token;
        const user = data.user || data.data;

        if (!token) {
          throw new Error("No token received from server");
        }

        this.setToken(token);
        this.setUser(user);

        return { token, user };
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },

    async register(registData) {
      try {
        const response = await axios.post(
          apiBase + "/auth/register",
          registData
        );
        const data = response?.data;

        if (data?.status?.success) {
          const expiresInMs = data.data?.expires_in * 1000; // 300 seconds in milliseconds
          const expiryTime = new Date().getTime() + expiresInMs;

          //
          Cookies.set("verf-mail", data?.data?.email, {
            expires: data.data?.expires_in / 86400,
          });
          localStorage.setItem("verf-mail-expiry", expiryTime.toString());
          //
        }
        return data;
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },

    resetPassword(email) {
      return axios.post(apiBase + "/auth/password/forgot", {
        email: email,
      });
    },

    verifyOTP(data) {
      return axios.post(apiBase + "/auth/password/verify-otp", data);
    },

    async verifyAccount(verData) {
      try {
        const response = await axios.post(
          apiBase + "/auth/verify-registration",
          verData
        );

        const data = response?.data;

        if (!data) {
          throw new Error("No data received from server");
        }

        // Handle both possible response structures
        const token = data.token || data.data?.token;
        const user = data.user || data.data;

        if (!token) {
          throw new Error("No token received from server");
        }

        this.setToken(token);
        this.setUser(user);

        return { token, user };
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },

    updatePassword(data) {
      return axiosIns.post(apiBase + "/auth/password/reset", data);
    },

    resendVerficationOTP(data) {
      return axiosIns.post(apiBase + "/auth/resend-registration-otp", {
        email: data,
      });
    },
  },
});
