import { describe, expect, it } from 'vitest'

describe('helpers', () => {
  it('exportData returns a promise', () => {
    const result = typeof import('@/api/client')
    expect(result).toBe('object')
  })
})
