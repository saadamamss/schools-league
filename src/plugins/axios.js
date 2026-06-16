import axios from "axios";
import { API_BASE_URL } from "@/config";
import { authService } from "@/services/auth.service";
import { toast } from "vue3-toastify";
import { mockApi } from "@/services/mockData";

const USE_MOCK_DATA = import.meta.env.VITE_USE_MOCK_DATA === "true";

// Create custom adapter for mock data
const mockAdapter = async (config) => {
  
  const method = config.method?.toLowerCase();
  const endpoint = config.url;
  const data = config.data;
  const params = config.params;

  let mockResponse;
  
  try {
    switch (method) {
      case "get":
        mockResponse = await mockApi.get(endpoint, { params });
        break;
      case "post":
        mockResponse = await mockApi.post(endpoint, data);
        break;
      case "put":
        mockResponse = await mockApi.put(endpoint, data);
        break;
      case "delete":
        mockResponse = await mockApi.delete(endpoint);
        break;
      default:
        throw new Error(`Method ${method} not supported in mock adapter`);
    }
    
    // Return proper axios response structure
    console.log(mockResponse);
    
    return {
      data: mockResponse,
      status: 200,
      statusText: "OK",
      headers: {},
      config: config,
      request: {}
    };
  } catch (error) {

    throw error;
  }
};

const axiosIns = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    Accept: "application/json",
  },
});

// Replace the adapter when in mock mode
if (USE_MOCK_DATA) {
  // Override the adapter
  axiosIns.defaults.adapter = mockAdapter;
}

// Request interceptor for adding auth token (only for real API calls)
axiosIns.interceptors.request.use(
  (config) => {
    // Only add token if not in mock mode (mock adapter handles mock requests)
    if (!USE_MOCK_DATA) {
      const token = authService.getToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    
    console.log("Request config:", config);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for API calls
axiosIns.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Handle 401 Unauthorized errors
    if (error.response?.status === 401 && !originalRequest?._retry) {
      originalRequest._retry = true;

      authService.logout();
      window.location.href = "/auth/login";

      return Promise.reject("Unauthorized");
    }

    if (error.response?.status === 422) {
      const errorsObj = error.response?.data?.errors;

      if (errorsObj) {
        for (const key in errorsObj) {
          if (Object.prototype.hasOwnProperty.call(errorsObj, key)) {
            errorsObj[key].forEach((value) => {
              toast.error(value);
            });
          }
        }
      }
    }

    return Promise.reject(error);
  }
);

export default axiosIns;