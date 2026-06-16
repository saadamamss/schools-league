import axios from "@axios";

export const ShiftsService = {
  // Get all shifts with pagination and search
  getShifts(params) {
    return axios.get("/shifts", { params });
  },

  // Get shift details by ID
  getShiftById(id) {
    return axios.get(`/shifts/${id}`);
  },

  // Create new shift
  createShift(data) {
    return axios.post("/shifts", data);
  },

  // Update shift
  updateShift(id, data) {
    return axios.put(`/shifts/${id}`, data);
  },

  // Delete shift
  deleteShift(id) {
    return axios.delete(`/shifts/${id}`);
  },
};
