export const mockUsers = [
  {
    id: 1,
    name: "Ahmed Hassan",
    email: "ahmed.hassan@example.com",
    phone: "+20 100 123 4567",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Cairo",
    status: "active",
    created_at: "2025-01-15T10:30:00Z",
    avatar: null,
  },
  {
    id: 2,
    name: "Sarah Mohamed",
    email: "sarah.mohamed@example.com",
    phone: "+20 100 234 5678",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Alexandria",
    status: "active",
    created_at: "2025-01-20T14:20:00Z",
    avatar: null,
  },
  {
    id: 3,
    name: "Mohamed Ali",
    email: "mohamed.ali@example.com",
    phone: "+20 100 345 6789",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Giza",
    status: "active",
    created_at: "2025-02-01T09:15:00Z",
    avatar: null,
  },
  {
    id: 4,
    name: "Fatma Ibrahim",
    email: "fatma.ibrahim@example.com",
    phone: "+20 100 456 7890",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Cairo",
    status: "inactive",
    created_at: "2025-02-10T11:45:00Z",
    avatar: null,
  },
  {
    id: 5,
    name: "Omar Khaled",
    email: "omar.khaled@example.com",
    phone: "+20 100 567 8901",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Mansoura",
    status: "active",
    created_at: "2025-02-15T16:00:00Z",
    avatar: null,
  },
  {
    id: 6,
    name: "Layla Ahmed",
    email: "layla.ahmed@example.com",
    phone: "+20 100 678 9012",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Alexandria",
    status: "active",
    created_at: "2025-02-20T08:30:00Z",
    avatar: null,
  },
  {
    id: 7,
    name: "Youssef Samir",
    email: "youssef.samir@example.com",
    phone: "+20 100 789 0123",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Cairo",
    status: "active",
    created_at: "2025-03-01T13:00:00Z",
    avatar: null,
  },
  {
    id: 8,
    name: "Nour Hassan",
    email: "nour.hassan@example.com",
    phone: "+20 100 890 1234",
    user_type: {
      name: "Operations Manager",
      code: "operations_manager",
    },
    role: "operations_manager",
    city: "Giza",
    status: "active",
    created_at: "2025-03-05T10:00:00Z",
    avatar: null,
  },
];

export const mockUserTypes = [
  {
    id: 1,
    name: "operations_manager",
    key: "operations_manager",
    description: "Full system access",
  },
  {
    id: 2,
    name: "Supervisor",
    key: "supervisor",
    description: "Can manage volunteers",
  },
  { id: 3, name: "Volunteer", key: "volunteer", description: "Basic access" },
];

export const mockCities = [
  { id: 1, name: "Cairo", country: "Egypt" },
  { id: 2, name: "Alexandria", country: "Egypt" },
  { id: 3, name: "Giza", country: "Egypt" },
  { id: 4, name: "Mansoura", country: "Egypt" },
  { id: 5, name: "Tanta", country: "Egypt" },
  { id: 6, name: "Port Said", country: "Egypt" },
  { id: 7, name: "Suez", country: "Egypt" },
  { id: 8, name: "Luxor", country: "Egypt" },
  { id: 9, name: "Aswan", country: "Egypt" },
  { id: 10, name: "Faiyum", country: "Egypt" },
];

export const mockNationalities = [
  { id: 1, name: "Egyptian" },
  { id: 2, name: "Saudi Arabian" },
  { id: 3, name: "Emirati" },
  { id: 4, name: "Kuwaiti" },
  { id: 5, name: "Qatari" },
  { id: 6, name: "Bahraini" },
  { id: 7, name: "Omani" },
  { id: 8, name: "Jordanian" },
  { id: 9, name: "Lebanese" },
  { id: 10, name: "Syrian" },
];

export const mockLocations = [
  {
    id: 1,
    name: "Cairo Stadium",
    type: "stadium",
    city: "Cairo",
    address: "Al-Mokkatam, Cairo",
    latitude: 30.0444,
    longitude: 31.2357,
  },
  {
    id: 2,
    name: "Alexandria Library",
    type: "library",
    city: "Alexandria",
    address: "Shatby, Alexandria",
    latitude: 31.2001,
    longitude: 29.9187,
  },
  {
    id: 3,
    name: "Giza Sports Club",
    type: "club",
    city: "Giza",
    address: "Dokki, Giza",
    latitude: 30.0444,
    longitude: 31.2124,
  },
  {
    id: 4,
    name: "Mansoura Youth Center",
    type: "center",
    city: "Mansoura",
    address: "El-Mansoura",
    latitude: 31.0409,
    longitude: 31.3785,
  },
  {
    id: 5,
    name: "Tanta Sports Hall",
    type: "hall",
    city: "Tanta",
    address: "El-Mansoura Road, Tanta",
    latitude: 30.7869,
    longitude: 31.0009,
  },
];

export const mockLocationTypes = [
  { id: 1, name: "Stadium", key: "stadium" },
  { id: 2, name: "Library", key: "library" },
  { id: 3, name: "Sports Club", key: "club" },
  { id: 4, name: "Youth Center", key: "center" },
  { id: 5, name: "Sports Hall", key: "hall" },
];

export const mockLocationSubTypes = [
  { id: 1, name: "Football Stadium", parent_key: "stadium" },
  { id: 2, name: "Basketball Stadium", parent_key: "stadium" },
  { id: 3, name: "Public Library", parent_key: "library" },
  { id: 4, name: "School Library", parent_key: "library" },
  { id: 5, name: "Private Club", parent_key: "club" },
  { id: 6, name: "Government Center", parent_key: "center" },
];

export const mockShifts = [
  {
    id: 1,
    name: "Morning Shift - Cairo Stadium",
    date: "2025-03-20",
    start_time: "08:00",
    end_time: "14:00",
    location_id: 1,
    location_name: "Cairo Stadium",
    supervisor_id: 2,
    supervisor_name: "Sarah Mohamed",
    volunteers_needed: 10,
    volunteers_assigned: 8,
    status: "active",
  },
  {
    id: 2,
    name: "Evening Shift - Alexandria Library",
    date: "2025-03-20",
    start_time: "16:00",
    end_time: "22:00",
    location_id: 2,
    location_name: "Alexandria Library",
    supervisor_id: 5,
    supervisor_name: "Omar Khaled",
    volunteers_needed: 6,
    volunteers_assigned: 6,
    status: "active",
  },
  {
    id: 3,
    name: "Morning Shift - Giza Sports Club",
    date: "2025-03-21",
    start_time: "08:00",
    end_time: "14:00",
    location_id: 3,
    location_name: "Giza Sports Club",
    supervisor_id: 8,
    supervisor_name: "Nour Hassan",
    volunteers_needed: 8,
    volunteers_assigned: 5,
    status: "pending",
  },
  {
    id: 4,
    name: "Full Day - Mansoura Youth Center",
    date: "2025-03-22",
    start_time: "08:00",
    end_time: "18:00",
    location_id: 4,
    location_name: "Mansoura Youth Center",
    supervisor_id: 2,
    supervisor_name: "Sarah Mohamed",
    volunteers_needed: 15,
    volunteers_assigned: 12,
    status: "active",
  },
  {
    id: 5,
    name: "Morning Shift - Tanta Sports Hall",
    date: "2025-03-23",
    start_time: "09:00",
    end_time: "15:00",
    location_id: 5,
    location_name: "Tanta Sports Hall",
    supervisor_id: 5,
    supervisor_name: "Omar Khaled",
    volunteers_needed: 8,
    volunteers_assigned: 4,
    status: "pending",
  },
];

export const mockAttendance = [
  {
    id: 1,
    user_id: 3,
    shift_id: 1,
    status: "present",
    check_in: "07:55",
    check_out: "14:05",
    date: "2025-03-20",
  },
  {
    id: 2,
    user_id: 6,
    shift_id: 1,
    status: "present",
    check_in: "07:58",
    check_out: "14:00",
    date: "2025-03-20",
  },
  {
    id: 3,
    user_id: 7,
    shift_id: 1,
    status: "absent",
    check_in: null,
    check_out: null,
    date: "2025-03-20",
  },
  {
    id: 4,
    user_id: 4,
    shift_id: 2,
    status: "present",
    check_in: "15:50",
    check_out: "22:10",
    date: "2025-03-20",
  },
  {
    id: 5,
    user_id: 5,
    shift_id: 2,
    status: "late",
    check_in: "16:15",
    check_out: "22:00",
    date: "2025-03-20",
  },
  {
    id: 6,
    user_id: 8,
    shift_id: 2,
    status: "present",
    check_in: "15:55",
    check_out: "22:05",
    date: "2025-03-20",
  },
];

export const mockSites = [
  {
    id: 1,
    name: "Downtown Cairo Site",
    location_id: 1,
    city: "Cairo",
    address: "Tahrir Square",
    type: "stadium",
    status: "active",
    total_volunteers: 25,
  },
  {
    id: 2,
    name: "Alexandria Coastal Site",
    location_id: 2,
    city: "Alexandria",
    address: "Corniche",
    type: "library",
    status: "active",
    total_volunteers: 15,
  },
  {
    id: 3,
    name: "Giza Plateau Site",
    location_id: 3,
    city: "Giza",
    address: "Pyramids Road",
    type: "club",
    status: "inactive",
    total_volunteers: 0,
  },
];

export const mockStatistics = {
  total_users: 156,
  active_users: 124,
  total_shifts: 89,
  completed_shifts: 75,
  total_attendance: 1245,
  attendance_rate: 92.5,
  cities_count: 10,
  locations_count: 15,
  volunteers_by_city: [
    { city: "Cairo", count: 45 },
    { city: "Alexandria", count: 30 },
    { city: "Giza", count: 25 },
    { city: "Mansoura", count: 20 },
    { city: "Others", count: 36 },
  ],
  shifts_by_month: [
    { month: "Jan", count: 12 },
    { month: "Feb", count: 15 },
    { month: "Mar", count: 18 },
  ],
};

export const mockUserDetails = {
  id: 1,
  name: "Ahmed Hassan",
  email: "ahmed.hassan@example.com",
  phone: "+20 100 123 4567",
  user_type: {
    name: "Operations Manager",
    code: "operations_manager",
  },
  role: "admin",
  city: "Cairo",
  nationality: "Egyptian",
  birth_date: "1990-05-15",
  status: "active",
  created_at: "2025-01-15T10:30:00Z",
  last_login: "2025-03-25T08:30:00Z",
  shifts_completed: 45,
  hours_volunteered: 320,
};

export const mockProfile = {
  id: 1,
  name: "Ahmed Hassan",
  email: "ahmed.hassan@example.com",
  phone: "+20 100 123 4567",
  avatar: null,
  user_type: {
    name: "Operations Manager",
    code: "operations_manager",
  },
  role: "admin",
  city: "Cairo",
  nationality: "Egyptian",
  birth_date: "1990-05-15",
};

const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

export const mockApi = {
  async get(endpoint, config = {}) {
    await delay(300);

    const params = config?.params || {};
    const perPage = params.per_page || 10;
    const page = params.page || 1;
    const search = params.search || "";

    const getPaginatedData = (data) => {
      let filtered = data;
      if (search) {
        filtered = data.filter(
          (item) =>
            item.name?.toLowerCase().includes(search.toLowerCase()) ||
            item.email?.toLowerCase().includes(search.toLowerCase()) ||
            item.phone?.includes(search),
        );
      }
      const start = (page - 1) * perPage;
      const end = start + perPage;
      const paginated = filtered.slice(start, end);

      return {
        data: paginated,
        pagination: {
          i_total_objects: filtered.length,
          i_per_page: perPage,
          i_current_page: page,
          total_pages: Math.ceil(filtered.length / perPage),
        },
      };
    };

    if (endpoint === "user-types") {
      return { data: { data: mockUserTypes } };
    }

    if (endpoint === "cities") {
      return { data: getPaginatedData(mockCities) };
    }

    if (endpoint === "locations") {
      let filtered = mockLocations;
      if (params.city_id) {
        filtered = filtered.filter((l) => l.city_id === params.city_id);
      }
      if (search) {
        filtered = filtered.filter((l) =>
          l.name.toLowerCase().includes(search.toLowerCase()),
        );
      }
      const start = (page - 1) * perPage;
      const end = start + perPage;
      return {
        data: {
          data: filtered.slice(start, end),
          pagination: {
            i_total_objects: filtered.length,
            i_per_page: perPage,
            i_current_page: page,
          },
        },
      };
    }

    if (endpoint === "location-types") {
      return { data: { data: mockLocationTypes } };
    }

    if (endpoint === "location-sub-types") {
      return { data: { data: mockLocationSubTypes } };
    }

    if (endpoint === "nationalities") {
      let filtered = mockNationalities;
      if (search) {
        filtered = filtered.filter((n) =>
          n.name.toLowerCase().includes(search.toLowerCase()),
        );
      }
      const start = (page - 1) * perPage;
      const end = start + perPage;
      return {
        data: {
          data: filtered.slice(start, end),
          pagination: {
            i_total_objects: filtered.length,
            i_per_page: perPage,
            i_current_page: page,
          },
        },
      };
    }

    if (endpoint === "shifts") {
      let filtered = mockShifts;
      if (params.status) {
        filtered = filtered.filter((s) => s.status === params.status);
      }
      const start = (page - 1) * perPage;
      const end = start + perPage;
      return {
        data: {
          data: filtered.slice(start, end),
          pagination: {
            i_total_objects: filtered.length,
            i_per_page: perPage,
            i_current_page: page,
          },
        },
      };
    }

    if (endpoint.startsWith("shifts/")) {
      const id = parseInt(endpoint.split("/")[1]);
      const shift = mockShifts.find((s) => s.id === id);
      return { data: { data: shift } };
    }

    if (endpoint === "users") {
      return { data: getPaginatedData(mockUsers) };
    }

    if (endpoint.startsWith("users/")) {
      const id = parseInt(endpoint.split("/")[1]);
      const user = mockUsers.find((u) => u.id === id);
      return { data: { data: user } };
    }

    if (endpoint === "attendance") {
      const start = (page - 1) * perPage;
      const end = start + perPage;
      const enriched = mockAttendance.map((a) => ({
        ...a,
        user_name: mockUsers.find((u) => u.id === a.user_id)?.name,
        shift_name: mockShifts.find((s) => s.id === a.shift_id)?.name,
      }));
      return {
        data: {
          data: enriched.slice(start, end),
          pagination: {
            i_total_objects: enriched.length,
            i_per_page: perPage,
            i_current_page: page,
          },
        },
      };
    }

    if (endpoint === "sites") {
      return { data: getPaginatedData(mockSites) };
    }

    if (endpoint === "statistics") {
      return { data: { data: mockStatistics } };
    }

    if (endpoint === "auth/profile") {
      return { data: { data: mockProfile } };
    }

    console.warn(`Mock not implemented for endpoint: ${endpoint}`);
    return { data: { data: [] } };
  },

  async post(endpoint, data) {
    await delay(300);

    if (endpoint === "auth/login") {
      return {
        data: {
          token: "mock_jwt_token_" + Date.now(),
          user: mockProfile,
        },
      };
    }

    if (endpoint === "auth/change-password") {
      return { data: { message: "Password changed successfully" } };
    }

    if (endpoint === "shifts") {
      const newShift = {
        id: mockShifts.length + 1,
        ...data,
        status: "pending",
      };
      return { data: { data: newShift } };
    }

    if (endpoint === "users") {
      const newUser = {
        id: mockUsers.length + 1,
        ...data,
        status: "active",
        created_at: new Date().toISOString(),
      };
      mockUsers.push(newUser);
      return { data: { data: newUser } };
    }

    console.warn(`Mock POST not implemented for endpoint: ${endpoint}`);
    return { data: { success: true } };
  },

  async put(endpoint, data) {
    await delay(300);

    if (endpoint.startsWith("shifts/")) {
      const id = parseInt(endpoint.split("/")[1]);
      const index = mockShifts.findIndex((s) => s.id === id);
      if (index !== -1) {
        mockShifts[index] = { ...mockShifts[index], ...data };
        return { data: { data: mockShifts[index] } };
      }
    }

    if (endpoint.startsWith("users/")) {
      const id = parseInt(endpoint.split("/")[1]);
      const index = mockUsers.findIndex((u) => u.id === id);
      if (index !== -1) {
        mockUsers[index] = { ...mockUsers[index], ...data };
        return { data: { data: mockUsers[index] } };
      }
    }

    console.warn(`Mock PUT not implemented for endpoint: ${endpoint}`);
    return { data: { success: true } };
  },

  async delete(endpoint) {
    await delay(300);

    if (endpoint.startsWith("shifts/")) {
      const id = parseInt(endpoint.split("/")[1]);
      return { data: { message: "Shift deleted successfully" } };
    }

    if (endpoint.startsWith("users/")) {
      const id = parseInt(endpoint.split("/")[1]);
      return { data: { message: "User deleted successfully" } };
    }

    console.warn(`Mock DELETE not implemented for endpoint: ${endpoint}`);
    return { data: { success: true } };
  },
};
