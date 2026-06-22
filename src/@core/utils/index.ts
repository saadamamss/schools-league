export const isEmpty = (value: unknown): boolean => {
  if (value === null || value === undefined || value === '') return true
  return !!(Array.isArray(value) && value.length === 0)
}

export const isNullOrUndefined = (value: unknown): value is null | undefined => {
  return value === null || value === undefined
}

export const isEmptyArray = (arr: unknown): boolean => {
  return Array.isArray(arr) && arr.length === 0
}

export const isObject = (obj: unknown): obj is Record<string, unknown> => {
  return obj !== null && !!obj && typeof obj === 'object' && !Array.isArray(obj)
}

export const isToday = (date: Date): boolean => {
  const today = new Date()
  return (
    date.getDate() === today.getDate() &&
    date.getMonth() === today.getMonth() &&
    date.getFullYear() === today.getFullYear()
  )
}
