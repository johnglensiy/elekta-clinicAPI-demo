# Clinic API

A simple ASP.NET Core Minimal API for managing patient records in a clinic. Supports CRUD operations on patients

---

## Features

- Add new patients
- Retrieve all patients or a single patient
- Update patient details (PATCH)
- Delete patients with no diagnosis
- Health check endpoint for database connection

---

## Endpoints

| Method | Endpoint                  | Description                                               |
|--------|---------------------------|-----------------------------------------------------------|
| GET    | `/patients`               | Get all patients                                          |
| GET    | `/patients/{medRecordNo}` | Get a single patient by medical record number             |
| POST   | `/patients`               | Add a new patient                                         |
| PATCH  | `/patients/{id}`          | Update patient details (partial update)                   |
| DELETE | `/patients/{id}`          | Delete patient if no admitting diagnosis                  |
| GET    | `/`                       | Health check / database connection test                   |

---
