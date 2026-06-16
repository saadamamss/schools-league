export interface UserCity {
  id: number
  name: { ar: string; en: string }
}

export interface UserType {
  id: number
  code: string
  name: { ar: string; en: string } | string
  key: string
  is_active: boolean
}

export interface User {
  id: number
  full_name: string
  first_name: string
  middle_name?: string
  last_name: string
  email: string
  phone: string
  sa_id: string
  gender: 'male' | 'female'
  birth_date?: string
  profile_image: string | null
  is_active: boolean
  city: UserCity
  user_type: UserType
  nationality_id?: number | null
  today_attendance_status: 'present' | 'absent' | 'late' | null
  today_check_in: string | null
  today_check_out: string | null
  working_days_count: number
  monthly_working_hours: number
  daily_rate: number
  created_at: string
  updated_at: string
  permissions: string[]
}

export interface LoginResponse {
  token: string
  user: User
}

export interface RegisterData {
  email: string
  phone: string
  password: string
  password_confirmation: string
  first_name: string
  middle_name?: string
  last_name: string
  sa_id: string
  gender: 'male' | 'female'
  city_id?: number
  user_type_id?: number
}

export interface Location {
  id: number
  site_name: string
  site_type: string
  license?: string
  phone?: string
  address?: string
  latitude?: number
  longitude?: number
  city: UserCity
  is_active: boolean
  image?: string
  joined_employee?: number
  work_hours?: string
  work_type?: string
  current_events_count: number
  total_working_hours: number
}

export type AttendanceStatusType = 'attendance' | 'absent' | 'departed'

export interface AttendanceRecord {
  id: number
  user_id: number
  location_id: number
  status: AttendanceStatusType
  check_in: string | null
  check_out: string | null
  date: string
  user: User
  location: Location
}

export interface AttendanceStatus {
  status: AttendanceStatusType
  check_in_time: string | null
  check_out_time: string | null
  date: string
  user: Pick<User, 'id' | 'full_name' | 'sa_id' | 'profile_image'> & {
    city: UserCity
    user_type: { name: { ar: string } }
  }
  location?: { id: number; name: string; image: string | null }
}

export interface DashboardStats {
  total_users: number
  active_users: number
  total_attendance: number
  total_absence: number
  total_hours: number
  daily_attendance: number
  weekly_hours: number
  active_locations: number
  attendance_rate: number
}

export interface SitesWidgetData {
  total_locations: number
  total_attendance: number
  total_absence: number
  total_users: number
}

export interface Nationality {
  id: number
  name: { ar: string; en: string }
  code: string
}

export interface Pagination {
  i_per_page: number
  i_total_objects: number
  i_current_page: number
}

export interface ApiResponse<T> {
  data: T
  status?: { message: string; code: number; success?: boolean }
  pagination?: Pagination
  statistics?: Record<string, number>
}

export interface SnackbarState {
  show: boolean
  message: string
  color: string
  timeout: number
}

export interface Tag {
  id: number
  name: string
  color?: string
}

export interface Shift {
  id: number
  name: string
  start_time: string
  end_time: string
  location_id: number
  is_active: boolean
}
