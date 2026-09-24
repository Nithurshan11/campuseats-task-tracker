# CampusEats Task Tracker

A small shared repository for the CampusEats team to track work using GitHub Flow.

## Workflow

- `main` stays deployable.
- Each change is made on a short-lived branch (`feature/`, `fix/`, or `chore/`).
- Changes reach `main` only through a reviewed pull request.
- GitHub Actions runs a CI check on every push and pull request. A failing check blocks the merge.

## Layout

```
README.md
.gitignore
src/tasks.js
.github/workflows/ci.yml
```

SE3090 Lab 08 — Git, Collaborative Development, CI/CD, Security and Code Quality.
