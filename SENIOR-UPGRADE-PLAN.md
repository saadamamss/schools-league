# Frontend Senior-Level Upgrade Plan

## Context

The Schools League frontend (Vue 3 + Vuetify + Pinia + TypeScript) is functional but has mid-level quality issues: security bugs in auth, excessive `any` types, duplicated widget components, weak tests, global CSS leaks, and inconsistent patterns. This plan upgrades it to senior-level production quality across 6 phases.

---

## Phase 1: Critical Security & Auth Fixes

**Goal:** Fix production-blocking bugs first.

### 1.1 Fix token refresh bug
- **File:** `src/plugins/axios.ts:94`
- The refresh interceptor sends `authService.getToken()` (the access token) as the refresh payload. The backend's `AuthService` stores a separate `RefreshToken` field.
- Add `getRefreshToken()` to `AuthService` class in `src/services/auth.service.ts` that reads a `refreshToken` field from stored user data.
- Update `setUser()` to also persist `refreshToken` from login/verify responses.
- Update the axios 401 interceptor to call `authService.getRefreshToken()` instead of `getToken()`.
- Update `stores/Auth.ts` login/verifyAccount to store `refreshToken` from response data.

### 1.2 Fix `v-can` directive — remove element instead of hiding
- **File:** `src/main.ts:33-51`
- Replace `el.style.display = hasPermission ? '' : 'none'` with proper element removal using a comment placeholder pattern, so restricted elements aren't accessible via DevTools.

### 1.3 Fix `window.open()!` crash
- **File:** `src/@core/utils/helpers.ts:31`
- Add null check: `if (!printWindow) return` before writing to it.

### 1.4 Add `.env` to `.gitignore`
- **File:** `front/.gitignore`
- Add `.env` (not `.env.example`) to gitignore. The `.env` file should not be tracked.

---

## Phase 2: TypeScript Strictness — Eliminate `any`

**Goal:** Replace all `any` with proper types. ~179 `any` occurrences across the codebase.

### 2.1 Extend model types
- **File:** `src/types/models.ts`
- Add missing interfaces: `EventDetailData` (with `supervisor`, `inspectors`, `latitude`, `longitude`, `max_users`, etc.), `FinancialTransaction`, `BankInfo`, `UserWithBankInfo`, `AttendanceGroupItem`.
- Remove the duplicate `LoginResponse` from `stores/Auth.ts` — import from `types/models.ts`.

### 2.2 Type all components
Apply typed `defineProps<{...}>()` to components currently using `defineProps(['...'])`:
- `src/components/Widgets.vue` — `defineProps<{ isLoading: boolean; data: DashboardStats | null }>()`
- `src/components/AttendeWidgets.vue` — same pattern
- `src/components/UsersWidgets.vue` — same pattern
- `src/components/SitesWidgets.vue` — typed props with `SitesWidgetData`

### 2.3 Type composables and stores
- `src/composables/useUserDetails.ts:14` — `ref<UserWithLocation | null>` instead of `Record<string, any>`
- `src/stores/Dashboard.ts` — remove `catch (error: any)` → `catch (error: unknown)`
- `src/stores/Auth.ts` — remove duplicate `LoginResponse`, use proper error typing
- `src/components/EventDetailsDialog.vue` — type `selectedEvent`, `selectedEventSupervisor`, `selectedEventInspectors` with proper interfaces
- `src/components/UserDialogDetails.vue` — type all `ref<any[]>` with proper interfaces

### 2.4 Type DataTable component
- **File:** `src/components/DataTable.vue`
- Replace `items: any[]` and `headers: any[]` with generic or Vuetify-compatible types.

---

## Phase 3: Architecture & DRY

**Goal:** Eliminate duplication, clean up dead code, normalize patterns.

### 3.1 Unify widget components into one configurable component
Four widget components are nearly identical: `Widgets.vue`, `SitesWidgets.vue`, `AttendeWidgets.vue`, `UsersWidgets.vue`. They all wrap `StatsWidget` with different SVG icons and data keys.

Create `src/components/StatsWidgetGrid.vue`:
```
Props: { isLoading: boolean, widgets: Array<{ title: string, value: number, icon: string }> }
```
Each page passes its own widget config. The 4 existing files become thin wrappers or are replaced entirely.

### 3.2 Extract inline SVGs
- The massive inline SVGs in widget components should move to dedicated icon components under `src/components/icons/` (stats-users.vue, stats-attendance.vue, stats-absence.vue, stats-hours.vue, stats-events.vue).
- Reference them by component name in the widget config.

### 3.3 Remove dead code
- Delete `src/stores/Settings.ts` (unused store)
- Remove unused `search` ref in `src/layouts/DashboardLayout.vue:20`
- Remove unused `onBeforeMount` import in `src/pages/auth/login.vue`
- Remove `auth/index` empty redirect (dead route) in `src/router/routes.ts:63-65`

### 3.4 Normalize naming conventions
- Rename store files to consistent lowercase: `Auth.ts` → `auth.ts`, `Dashboard.ts` → `dashboard.ts`, `Settings.ts` → (deleted)
- Update all imports accordingly.

### 3.5 Remove redundant `meta.requiresAuth` from child routes
- **File:** `src/router/routes.ts`
- Children inherit meta from parent. Remove `requiresAuth: true` from every child under the `/` route.
- Add `guest: true` to auth child routes so the guard's guest redirect works.

### 3.6 Centralized error handling
- Add a `useApiError` composable or extend the axios interceptor to handle common error patterns (422 validation, 500 server errors, network errors) in one place instead of repeating try/catch + showSnackbar in every store action and page.

---

## Phase 4: Request Lifecycle & Robustness

**Goal:** Handle unmount, race conditions, and edge cases properly.

### 4.1 Add AbortController to `usePaginatedFetch`
- **File:** `src/composables/usePaginatedFetch.ts`
- Create an AbortController per fetch call, abort the previous one on new fetch. This prevents stale responses from overwriting newer data.
- Return a `cleanup` function or use `onUnmounted` internally to abort on component teardown.

### 4.2 Add AbortController to `useUserDetails`
- **File:** `src/composables/useUserDetails.ts`
- Same pattern: abort previous request when `showDetails` is called again.

### 4.3 Fix `usePaginatedFetch` side effects
- The composable calls `fetchData()` during composition if `fetchOnMount` is true. Wrap this in `onMounted` to avoid side effects during setup phase.

---

## Phase 5: Testing — Quality Over Quantity

**Goal:** Replace shallow/fake tests with meaningful ones. Target: critical paths actually tested.

### 5.1 Rewrite `axios.spec.js`
- **File:** `src/plugins/__tests__/axios.spec.js`
- Actually test the interceptor by creating an axios instance, attaching the interceptors, and verifying:
  - Bearer token is attached to requests
  - 401 triggers refresh flow
  - Concurrent 401s queue properly
  - Failed refresh redirects to login

### 5.2 Rewrite `guard.spec.js`
- **File:** `src/router/__tests__/guard.spec.js`
- Test the actual router guard function instead of re-implementing the logic inline.
- Use `createRouter` + `createMemoryHistory` for proper integration tests.

### 5.3 Add missing tests
- `src/services/__tests__/auth.service.spec.js` — test `AuthService` class methods (getToken, setUser, logout, isAuthenticated, getRefreshToken).
- `src/composables/__tests__/useUserDetails.spec.js` — test fetch, loading states, error handling.

### 5.4 Add coverage threshold to CI
- **File:** `front/vitest.config.ts`
- Add `coverage: { thresholds: { statements: 60 } }` (start achievable, raise over time).
- **File:** `front/.github/workflows/ci.yml`
- Add `npm run test:coverage` step.

---

## Phase 6: CSS & Style Cleanup

**Goal:** Scope styles, remove `!important` hacks, fix leaks.

### 6.1 Scope `App.vue` styles
- **File:** `src/App.vue`
- Move global utility classes that must remain global into `src/styles/styles.scss`.
- Scope the rest with `<style scoped>` in `App.vue`.
- Remove the duplicated `.text-caption { font-size: 16px !important; }` from `users.vue`, `events/index.vue`, `attendances/index.vue`, `SitesWidgets.vue`, `UsersWidgets.vue`.

### 6.2 Fix `watchDebounced` with `debounce: 0`
- **File:** `src/components/Admin.vue:22`
- Replace `watchDebounced(isDark, ..., { debounce: 0 })` with a plain `watch()`.

### 6.3 Remove redundant `!important`
- Audit and remove unnecessary `!important` declarations where scoped styles or higher specificity selectors work.

---

## Execution Order

1. **Phase 1** first — security fixes are non-negotiable
2. **Phase 2** next — types make everything else safer to refactor
3. **Phase 3** — architecture cleanup with type safety in place
4. **Phase 4** — robustness improvements
5. **Phase 5** — tests covering the new code
6. **Phase 6** — CSS polish last (lowest risk)

Each phase is a separate commit so changes are reviewable and revertable.

---

## Verification

After each phase:
1. `npm run lint:ci` — zero warnings
2. `npm run type-check` — zero errors
3. `npm test` — all tests pass
4. `npm run build` — production build succeeds
5. Manual smoke test: login → dashboard → users → events → attendances → profile → logout
