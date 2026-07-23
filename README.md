# Helpdesk Platform

Containerized Helpdesk application built with ASP.NET Core, PostgreSQL and Docker.

## Architecture
Frontend
|
|
ASP.NET Core API
|
|
PostgreSQL Database

## Technologies

Backend:
- C#
- ASP.NET Core
- Entity Framework Core

Database:
- PostgreSQL

Frontend:
- HTML
- CSS
- JavaScript

Infrastructure:
- Docker
- Docker Compose

## Start

Create .env: POSTGRES_PASSWORD=password
Start: docker compose up -d

Application:

Frontend:
http://localhost

API:
http://localhost:8080

Database:
PostgreSQL

## Features

- Ticket management
- Kanban board
- Drag and drop status changes
- CRUD operations
- Persistent database storage