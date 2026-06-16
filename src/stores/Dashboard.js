import axiosIns from "@/plugins/axios";
import axios from "axios";
import { mockApi } from "@/services/mockData";
import { defineStore } from "pinia";

const USE_MOCK_DATA = import.meta.env.VITE_USE_MOCK_DATA === "true";

export const useDashboardStore = defineStore("DashboardStore", {
  state: () => ({
    userTypes: [],
    city: [],
    nationlities: [],
    events: [],
  }),
  actions: {
    // 👉 Fetch all user types
    async fetchUserTypes() {
      try {
        const response = await axiosIns.get("user-types");
        if (response.data) {
          this.userTypes = response.data?.data;
        }
      } catch (error) {
        console.log("faild to get user types");
      }
    },
    // 👉 Fetch all cities
    async fetchCities(search = "") {
      try {
        const seacrhFilter = !search.trim() ? {} : { search: search.trim() };
        const response = await axiosIns.get("cities", {
          params: { per_page: 15, ...seacrhFilter },
        });
        if (response.data) {
          this.city = response.data?.data;
        }
      } catch (error) {
        console.log("faild to get cities");
      }
    },
    // 👉 Fetch all Nationalities
    async fetchNationalities(search = "") {
      try {
        let response;
        if (USE_MOCK_DATA) {
          response = await mockApi.get("nationalities", {
            params: { per_page: 15, search },
          });
        } else {
          const seacrhFilter = !search.trim() ? {} : { search: search.trim() };
          response = await axios.get(
            "https://api.maidan.events/api/constants/nationalities",
            {
              params: { per_page: 15, ...seacrhFilter },
            }
          );
        }
        if (response.data) {
          this.nationlities = response.data?.data;
        }
      } catch (error) {
        console.log("faild to get nationalities");
      }
    },
    // 👉 Fetch all Nationalities
    async fetchEvents(search = "", city_id = undefined) {
      try {
        let response;
        if (USE_MOCK_DATA) {
          response = await mockApi.get("locations", {
            params: { per_page: 15, search, city_id },
          });
        } else {
          const seacrhFilter = !search.trim() ? {} : { search: search.trim() };
          response = await axios.get(
            "https://api.maidan.events/api/constants/locations",
            {
              params: { per_page: 15, ...seacrhFilter, city_id },
            }
          );
        }
        if (response.data) {
          this.events = response.data?.data;
        }
      } catch (error) {
        console.log("faild to get Locations");
      }
    },
  },
});
