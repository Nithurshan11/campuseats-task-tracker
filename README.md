# CampusEats Task Tracker

Full-stack CampusEats lab project: React/Vite frontend + .NET 8 Web API backend with PostgreSQL, JWT auth, Docker, and CI.

## Project structure

```
frontend/     React + Vite (Netlify-ready)
backend/      .NET 8 API + Dockerfile (Render-ready)
.github/      GitHub Actions CI
```

## Prerequisites

- Node.js 20+
- .NET 8 SDK
- PostgreSQL (local) or Neon PostgreSQL (cloud)

## Local setup

### 1. Backend environment

Never commit real secrets. Use one of:

- `backend/appsettings.Development.json` (gitignored), or
- `dotnet user-secrets` in `backend/`, or
- environment variables:

| Variable | Purpose |
|---|---|
| `ConnectionStrings__Default` | PostgreSQL connection string |
| `Jwt__Key` | Secure JWT signing key (32+ chars) |
| `Jwt__Issuer` | JWT issuer (e.g. `CampusEats`) |
| `Jwt__Audience` | JWT audience (e.g. `CampusEatsUsers`) |
| `DATABASE_URL` | Optional Neon/Render `postgresql://...` URI |

Example connection string:

```
Host=localhost;Port=5432;Database=campuseats;Username=<user>;Password=<password>
```

Run:

```bash
cd backend
dotnet restore
dotnet run --launch-profile http
```

API: `http://localhost:5206`  
Swagger: `http://localhost:5206/swagger`  
Menu: `http://localhost:5206/api/menu`

### 2. Frontend environment

Copy `frontend/.env.example` to `frontend/.env`:

```
VITE_API_URL=http://localhost:5206
```

```bash
cd frontend
npm install
npm run dev
```

## Deployment

### Frontend → Netlify

1. Base directory: `frontend`
2. Build command: `npm run build`
3. Publish directory: `dist`
4. Set env var `VITE_API_URL` to your Render API URL (no trailing slash)
5. SPA redirects are in `public/_redirects` and `netlify.toml`

### Backend → Render

1. Connect this repo, set root to `backend`
2. Use the included `Dockerfile`
3. Set env vars: `ConnectionStrings__Default` (or `DATABASE_URL`), `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`
4. Use Neon PostgreSQL for the database (SSL required; URI format is supported)

### Neon PostgreSQL

Create a Neon project, copy the connection string, and set it as `ConnectionStrings__Default` or `DATABASE_URL` on Render. Do not commit it.

## CI

GitHub Actions (`.github/workflows/ci.yml`) runs on pushes and pull requests to `main`.

## Security

- Do not commit `.env`, `appsettings.Development.json`, or real JWT/DB credentials
- Rotate any secrets that were ever shared in docs or chat
