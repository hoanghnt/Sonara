# Sonara

Full-stack music platform: upload tracks, playlists, search, and streaming. Backend uses ASP.NET Core; frontend uses Vite, React, and TypeScript.

## Stack

| Layer | Technology |
|--------|------------|
| API | .NET 10, EF Core, Npgsql, JWT (Bearer) |
| Database | PostgreSQL — local (Docker) or **Supabase** in production |
| Files | Local `upload/` when Supabase Storage is **not** configured; **Supabase Storage** when `SupabaseStorage__Url` + `SupabaseStorage__ServiceRoleKey` are set (DB paths prefixed with `sb:`; stream uses signed URL redirect) |
| UI | React 19, Vite, Tailwind CSS v4, Zustand, Axios |
| CI | GitHub Actions — `.github/workflows/ci.yml` |

## Repository layout

```
backend/Sonara/          # Sonara.API, Application, Domain, Infrastructure
frontend/sonara-ui/      # React SPA
docker-compose.yml       # Postgres, pgAdmin, API, frontend (development)
.github/workflows/       # CI
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20+ (CI uses 24)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for Compose)

## Quick start with Docker Compose

1. In the repo root, create `.env`:

   ```env
   JWT_SECRET=your-long-random-secret-at-least-32-characters
   ```

2. Start the stack:

   ```bash
   docker compose up --build
   ```

3. URLs:

   - Frontend: http://localhost:3000  
   - API: http://localhost:8080  
   - pgAdmin: http://localhost:5050  

4. Apply the database schema the first time (EF migration SQL in Supabase SQL Editor, or `dotnet ef database update` with a **direct** Postgres connection string — poolers can break the EF CLI).

Uploads in Compose are persisted via the `sonara_uploads` volume mounted at `/app/upload` for the API container.

## Local development (API + UI without full Compose)

1. Run PostgreSQL and set `ConnectionStrings:DefaultConnection` in `backend/Sonara/Sonara.API/appsettings.Development.json`.

2. Set **`JWT_SECRET`** in the environment (required for auth).

3. API (default URL from launch profile: http://localhost:5078):

   ```bash
   cd backend/Sonara
   dotnet run --project Sonara.API/Sonara.API.csproj
   ```

4. Frontend:

   ```bash
   cd frontend/sonara-ui
   npm ci
   npm run dev
   ```

   `vite.config.ts` proxies `/api` to `http://localhost:5078`. Use `VITE_API_BASE_URL=/api` for this setup.

## Environment variables

### API

| Variable | Purpose |
|----------|---------|
| `ConnectionStrings__DefaultConnection` | Npgsql connection string |
| `JWT_SECRET` | JWT signing secret (use a strong value in production) |
| `SupabaseStorage__Url` | Project URL, e.g. `https://<ref>.supabase.co` |
| `SupabaseStorage__ServiceRoleKey` | Supabase **service_role** key (server only; never expose to the browser) |
| `SupabaseStorage__Bucket` | Bucket name (default in appsettings: `sonara`) |
| `SupabaseStorage__SignedUrlExpirySeconds` | Signed URL lifetime (default `3600`) |

Defaults are in `backend/Sonara/Sonara.API/appsettings.json`. Override via environment or user secrets.

### Frontend

| Variable | Purpose |
|----------|---------|
| `VITE_API_BASE_URL` | Axios base URL. **Production (e.g. Vercel):** full API URL **including** `/api`, e.g. `https://<your-api>.onrender.com/api`. A bare `/api` only works when the dev proxy or same-origin API serves `/api`. |

## Production (typical setup)

- **API:** Render — build context **`backend`**, `Dockerfile` at `backend/Dockerfile`, listen on **8080**. Set Postgres and JWT (and Supabase Storage if used).
- **Frontend:** Vercel — root **`frontend/sonara-ui`**, output **`dist`**, set **`VITE_API_BASE_URL`** to the public API base ending in `/api`.
- **Database:** Supabase PostgreSQL (or any Postgres reachable by the API).
- **Object storage:** Create the Storage bucket (name must match config). Add **CORS** allowed origins for your frontend so `<audio>` can load signed Supabase URLs.

Cloud runtimes often have **ephemeral disk**; production uploads should use **Supabase Storage** (or another object store), not only local `upload/`.

## Database migrations

- Migrations live in `Sonara.Infrastructure`.
- Prefer applying SQL (e.g. from `dotnet ef migrations script`) in the Supabase SQL editor, or run `dotnet ef database update` against a **direct** connection if the pooler causes issues.

## CI

On **push** and **pull_request** to **`main`**, CI restores/builds the API project and runs `npm ci` + `npm run build` under `frontend/sonara-ui`.  
`VITE_API_BASE_URL` in CI is set to `https://example.com/api` so the Vite build succeeds without production secrets.

## Useful commands

| Command | Location |
|---------|----------|
| `dotnet build backend/Sonara/Sonara.API/Sonara.API.csproj -c Release` | repo root |
| `npm run dev` | `frontend/sonara-ui` |
| `npm run build` | `frontend/sonara-ui` |
| `docker compose up --build` | repo root |

## License

Add a license file or line here if you publish the repository.
