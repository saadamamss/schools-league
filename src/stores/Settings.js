import { defineStore } from "pinia";
import axios from "@axios";

export const useSettingsStore = defineStore("SettingsStore", {
  state: () => {
    return {
      isAlertShow: false,
      alertColor: "",
      alertMessage: "",
    };
  },
  actions: {
    fetchProductTags(params) {
      return axios.get("tags", { params });
    },
  },
});
