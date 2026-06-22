# Schools League - Frontend

Arabic RTL dashboard for attendance and workforce management. Built with Vue 3 + Vuetify 3.

## Tech Stack

- **Vue 3** (Composition API, `<script setup>`)
- **Vuetify 3** (Material Design, Arabic RTL)
- **Pinia** (state management)
- **Vue Router 4** (file-based routing)
- **Axios** (HTTP client with interceptors)
- **Chart.js** / **vue-chartjs** (charts)
- **Leaflet** (maps via OpenStreetMap)
- **CASL** (role-based access control)
- **Vite 5** (build tool)
- **Sentry** (error tracking, optional)
- **PWA** (auto-update, offline support)

## Project Structure

```
src/
  api/            # API modules (auth, users, locations, attendance, statistics, etc.)
  components/     # Reusable components (charts, maps, dialogs, widgets)
  composables/    # Vue composables (useApiError, usePaginatedFetch, etc.)
  config/         # App configuration (API URLs, endpoints)
  layouts/        # Page layouts (Dashboard, Auth, default)
  pages/          # Route pages (home, users, locations, attendances, profile, auth)
  plugins/        # Axios instance, Vuetify config, CASL
  router/         # Router setup with auth guards
  services/       # AuthService (in-memory token management)
  stores/         # Pinia stores (auth, dashboard, app)
  styles/         # SCSS (variables, themes, global styles)
  types/          # TypeScript models
  utils/          # Helpers, permission maps
```

## Environment Variables

| Variable | Description | Default |
|---|---|---|
| `VITE_API_BASE_URL` | API base URL | `/api/dashboard` |
| `VITE_SENTRY_DSN` | Sentry DSN (optional) | (empty) |
| `VITE_MAPBOX_STYLE_URL` | Mapbox style URL | (fallback in config) |

## Getting Started

```bash
npm install
npm run dev
```

Dev server runs on `http://localhost:3000` with API proxy to `http://localhost:5231`.

## Scripts

| Command | Description |
|---|---|
| `npm run dev` | Start dev server |
| `npm run build` | Production build |
| `npm run build:staging` | Staging build |
| `npm test` | Run unit tests (Vitest) |
| `npm run test:e2e` | Run E2E tests (Playwright) |
| `npm run type-check` | TypeScript type checking |
| `npm run lint` | ESLint with auto-fix |

## Deployment

Deployed on **Vercel**. API requests are proxied to the Railway backend via `vercel.json` rewrites.

```json
{
  "rewrites": [
    { "source": "/api/(.*)", "destination": "https://schools-league-api.up.railway.app/api/$1" },
    { "source": "/(.*)", "destination": "/index.html" }
  ]
}
```

## Key Features

- Arabic RTL full layout with dark/light theme toggle
- Role-based access control (operations_manager, project_manager, supervisor, protocol, organizer)
- Attendance tracking with check-in/check-out
- Location management with map integration
- Dashboard with charts and statistics widgets
- User management with CRUD and Excel export
- JWT authentication with HttpOnly cookie refresh flow
- In-memory token storage (XSS protection)
- Automatic retry on network/5xx errors
