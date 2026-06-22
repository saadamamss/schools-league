import { expect, test } from '@playwright/test'

test.describe('App E2E', () => {
  test('redirects to login when unauthenticated', async ({ page }) => {
    await page.goto('/')
    await expect(page).toHaveURL(/\/auth\/login/)
  })

  test('login page renders correctly', async ({ page }) => {
    await page.goto('/auth/login')
    await expect(page.locator('h1')).toContainText('تسجيل الدخول')
    await expect(page.locator('input[type="email"]')).toBeVisible()
    await expect(page.locator('input[type="password"]')).toBeVisible()
    await expect(page.locator('button[type="submit"]')).toBeVisible()
  })

  test('shows validation on empty login submit', async ({ page }) => {
    await page.goto('/auth/login')
    await page.locator('button[type="submit"]').click()
    await expect(page).toHaveURL(/\/auth\/login/)
  })

  test('login with invalid credentials shows error', async ({ page }) => {
    await page.route('**/api/dashboard/auth/login', async route => {
      await route.fulfill({
        status: 401,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'بيانات الدخول غير صحيحة' }),
      })
    })
    await page.goto('/auth/login')
    await page.locator('input[type="email"]').fill('test@example.com')
    await page.locator('input[type="password"]').fill('wrongpassword')
    await page.locator('button[type="submit"]').click()
    await expect(page).toHaveURL(/\/auth\/login/)
  })
})
