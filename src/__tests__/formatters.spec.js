import { describe, expect, it } from 'vitest'
import { avatarText, formatTime, formatTimeTo12Hour, kFormatter } from '@/@core/utils/formatters'

describe('avatarText', () => {
  it('returns empty string for null/undefined', () => {
    expect(avatarText(null)).toBe('')
    expect(avatarText(undefined)).toBe('')
    expect(avatarText('')).toBe('')
  })

  it('returns initials from full name', () => {
    expect(avatarText('Ahmed Mohamed')).toBe('AM')
  })

  it('returns single initial for single name', () => {
    expect(avatarText('Ahmed')).toBe('A')
  })
})

describe('kFormatter', () => {
  it('formats numbers under 10000 with commas', () => {
    expect(kFormatter(1000)).toBe('1,000')
    expect(kFormatter(9999)).toBe('9,999')
  })

  it('formats numbers over 9999 with k suffix', () => {
    expect(kFormatter(10000)).toBe('10k')
    expect(kFormatter(15000)).toBe('15k')
    expect(kFormatter(10500)).toBe('10.5k')
  })

  it('handles zero', () => {
    expect(kFormatter(0)).toBe('0')
  })
})

describe('formatTimeTo12Hour', () => {
  it('converts 24h to 12h format', () => {
    expect(formatTimeTo12Hour('09:30:00')).toBe('9:30 ص')
    expect(formatTimeTo12Hour('15:45:00')).toBe('3:45 م')
    expect(formatTimeTo12Hour('00:15:00')).toBe('12:15 ص')
    expect(formatTimeTo12Hour('12:00:00')).toBe('12:00 م')
  })
})

describe('formatTime', () => {
  it('returns empty for null/undefined', () => {
    expect(formatTime(null)).toBe('')
    expect(formatTime(undefined)).toBe('')
    expect(formatTime('')).toBe('')
  })

  it('formats valid date string to time', () => {
    const result = formatTime('2024-01-15T14:30:00')
    expect(result).toContain('م')
    expect(result).toContain('30')
  })
})
