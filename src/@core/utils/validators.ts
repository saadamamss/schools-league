import { isEmpty, isEmptyArray, isNullOrUndefined } from './index'

export const requiredValidator = (value: unknown): string | true => {
  if (isNullOrUndefined(value) || isEmptyArray(value) || value === false) return 'هذا الحقل مطلوب'
  return !!String(value).trim().length || 'هذا الحقل مطلوب'
}

export const phoneValidator = (value: unknown): string | true => {
  if (isNullOrUndefined(value)) return 'هذا الحقل مطلوب'
  return (
    (String(value).trim().length > 1 && String(value)?.startsWith('05')) ||
    'رقم الجوال يجب أن يبدأ ب 05'
  )
}

export const emailValidator = (value: unknown): string | true => {
  if (isEmpty(value)) return true
  // eslint-disable-next-line no-useless-escape
  const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z0-9]+\.)+[a-zA-Z]{2,}))$/
  if (Array.isArray(value)) {
    return (
      value.every(val => re.test(String(val))) ||
      'The Email field must be a valid email'
    )
  }
  return re.test(String(value)) || 'ادخل قيمة صالحة للبريد الإلكتروني'
}

export const passwordValidator = (password: unknown): string | true => {
  const regExp = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*()]).{8,}/
  const validPassword = regExp.test(String(password))
  return (
    validPassword ||
    'يجب أن يحتوي الحقل على حرف كبير وصغير وحرف خاص ورقم بحد أدنى 8 أحرف'
  )
}

export const confirmPasswordValidator = (value: unknown, target: unknown): string | true =>
  value === target || 'تأكيد حقل تأكيد كلمة المرور غير متطابق'

export const betweenValidator = (value: unknown, min: number, max: number): string | true => {
  const valueAsNumber = Number(value)
  return (
    (Number(min) <= valueAsNumber && Number(max) >= valueAsNumber) ||
    `أدخل الرقم بين ${min} و ${max}`
  )
}

export const integerValidator = (value: unknown): string | true => {
  if (isEmpty(value)) return true
  if (Array.isArray(value)) {
    return value.every((val: unknown) => /^-?[0-9]+$/.test(String(val)))
      ? true
      : 'This field must be an integer'
  }
  return /^-?[0-9]+$/.test(String(value))
    ? true
    : 'يجب أن يكون هذا الحقل عددًا صحيحًا'
}

export const regexValidator = (value: unknown, regex: string | RegExp): string | true => {
  if (isEmpty(value)) return true
  let regeX = regex
  if (typeof regeX === 'string') regeX = new RegExp(regeX)
  if (Array.isArray(value)) {
    return value.every(val => regexValidator(val, regeX)) ? true : 'تنسيق حقل Regex غير صالح'
  }
  return regeX.test(String(value)) ? true : 'تنسيق حقل Regex غير صالح'
}

export const alphaValidator = (value: unknown): string | true => {
  if (isEmpty(value)) return true
  return /^[A-Z]*$/i.test(String(value))
    ? true
    : 'قد يحتوي حقل Alpha على أحرف أبجدية فقط'
}

export const urlValidator = (value: unknown): string | true => {
  if (isEmpty(value)) return true
  const re =
    /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9.-]+\.[a-zA-Z]{2,5}\.{0,1}/
  return re.test(String(value)) || 'عنوان URL غير صالح'
}

export const lengthValidator = (value: unknown, length: number): string | true => {
  if (isEmpty(value)) return true
  return (
    String(value).length === length ||
    ` يجب أن يحتوي الحقل على الأقل علي${length}حروف `
  )
}

export const alphaDashValidator = (value: unknown): string | true => {
  if (isEmpty(value)) return true
  const valueAsString = String(value)
  return /^[0-9A-Z_-]*$/i.test(valueAsString) || 'جميع الحروف غير صالحة'
}

export const validationRules: Record<string, (v: unknown, target?: unknown) => string | true> = {
  required: v => !!v || 'هذا الحقل مطلوب',
  email: v => {
    if (!v) return true
    return /.+@.+\..+/.test(String(v)) || 'البريد الإلكتروني غير صالح'
  },
  phone: v => {
    if (!v) return true
    return /^[0-9]{10}$/.test(String(v)) || 'رقم الهاتف يجب أن يكون 10 أرقام'
  },
  password: v => {
    if (!v) return true
    return String(v).length >= 8 || 'كلمة المرور يجب أن تكون 8 أحرف على الأقل'
  },
  passwordMatch: (passwordConfirmation, password) => {
    if (!passwordConfirmation) return true
    return passwordConfirmation === password || 'كلمة المرور غير متطابقة'
  },
  saId: v => {
    if (!v) return true
    return /^[0-9]{10}$/.test(String(v)) || 'رقم الهوية يجب أن يكون 10 أرقام'
  },
}
