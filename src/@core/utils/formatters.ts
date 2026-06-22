import { isToday } from './index'
import dayjs from 'dayjs'
import ar from 'dayjs/locale/ar-sa'

dayjs.locale(ar)

export const avatarText = (value: string): string => {
  if (!value) return ''
  const nameArray = value.split(' ')
  return nameArray.map(word => word?.charAt(0)?.toUpperCase())?.join('')
}

export const kFormatter = (num: number): string => {
  const regex = /\B(?=(\d{3})+(?!\d))/g
  return Math.abs(num) > 9999
    ? `${Math.sign(num) * +(Math.abs(num) / 1000).toFixed(1)}k`
    : Math.abs(num).toFixed(0).replace(regex, ',')
}

export const formatDate = (
  value: string | number | Date,
  formatting: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric', year: 'numeric' }
): string => {
  if (!value) return String(value)
  return new Intl.DateTimeFormat('ar-EG', formatting).format(new Date(value))
}

export const formatDateToMonthShort = (value: string, toTimeForCurrentDay = true): string => {
  const date = new Date(value)
  let formatting: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric' }
  if (toTimeForCurrentDay && isToday(date)) formatting = { hour: 'numeric', minute: 'numeric' }
  return new Intl.DateTimeFormat('ar-EG', formatting).format(new Date(value))
}

export const format = (date: string | number | Date, formatStr = 'DD MMM YYYY'): string => {
  if (!date) return ''
  return dayjs(date).format(formatStr)
}

export const formatTime = (stringDate: string): string => {
  if (!stringDate) return ''
  const date = new Date(stringDate)
  const options: Intl.DateTimeFormatOptions = {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  }
  const formatter = new Intl.DateTimeFormat('en-US', options)
  return formatter.format(date).replace('AM', 'ص').replace('PM', 'م').trim()
}

export function formatTimeTo12Hour (time24?: string | null): string {
  if (!time24) return 'لم يسجل'
  const [hours, minutes] = time24.split(':')
  const hoursNum = parseInt(hours, 10)
  const period = hoursNum >= 12 ? 'م' : 'ص'
  const hours12 = hoursNum % 12 || 12
  return `${hours12}:${minutes} ${period}`
}
