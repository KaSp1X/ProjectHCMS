# Healthcare Microservices Platform

A healthcare-oriented backend project built with .NET and microservice architecture principles.

This project demonstrates:

* Microservices architecture
* REST + gRPC communication
* Event-driven architecture with Kafka
* Outbox Pattern for reliable event publishing
* CQRS-style application structure
* JWT authentication & authorization
* MongoDB + PostgreSQL persistence
* Dockerized infrastructure
* API Gateway

---

# Architecture Overview

| Service                 | Responsibility               | Database   |
| ----------------------- | ---------------------------- | ---------- |
| Identity Service        | Authentication & JWT issuing | PostgreSQL |
| Appointment Service     | Appointment scheduling       | PostgreSQL |
| Doctor Service          | Doctor availability          | PostgreSQL |
| Patient Service         | Patient profiles             | PostgreSQL |
| Medical Records Service | Medical records & files      | MongoDB    |
| Notification Service    | Kafka event consumers        | None       |
| API Gateway             | Routing + JWT auth           | None       |

---

# Main Technologies

## Backend

* .NET 10
* ASP.NET Core Web API
* gRPC
* Entity Framework Core
* MongoDB Driver
* Confluent.Kafka

## Databases

* PostgreSQL
* MongoDB

## Infrastructure

* Docker
* Docker Compose
* Kafka
* Zookeeper

## Security

* JWT Authentication
* Role-based access control
* Resource-based authorization

---

# Core Features

## Authentication

* JWT-based authentication
* Identity Service
* Gateway authentication validation
* Role support:

  * Patient
  * Doctor
  * Admin

## Appointment Management

* Create appointments
* Cancel appointments
* Time overlap validation
* Doctor availability validation via gRPC
* CQRS-style handlers

## Medical Records

* Create medical records
* Upload medical documents
* Download medical documents
* MongoDB GridFS file storage
* Secure file streaming
* Appointment existence validation via gRPC
* One medical record per appointment constraint

## Event-Driven Architecture

Kafka-based communication between services.

### Events

* appointment-created
* appointment-canceled
* appointment-completed
* medical-record-created

## gRPC Communication

### Doctor Availability

AppointmentService validates doctor availability using gRPC.

### Appointment Existence Validation

MedicalRecordsService validates appointment existence using gRPC.

## Reliability

### Outbox Pattern

* Events are first saved in MongoDB
* Background worker publishes events to Kafka
* Retry support implemented
* Prevents event loss

## Security Model

### Patients

Can:

* Create own appointments
* Cancel own appointments
* Access own medical records

### Doctors

Can:

* Cancel assigned appointments
* Complete assigned appointments
* Create own availability time slot
* Create medical records for assigned appointments
* Access assigned patient records

### Admins

Full access.

---

# Installation and CLI

## Clone repository

```bash
git clone https://github.com/KaSp1X/ProjectHCMS.git
cd ProjectHCMS
```

## Docker

```bash
docker compose up --build
```

## Apply migrations

Run inside services using PostgreSQL:

```bash
cd Services/<service-folder>
dotnet ef database update
```

Services:

* IdentityService
* AppointmentService
* DoctorService
* PatientService

## Create Kafka topics

Run inside terminal:

```bash
docker exec kafka-topics --bootstrap-server kafka:9092 --topic <topic-name> --create
```

Alternatively, run inside Kafka docker container terminal:

```bash
kafka-topics --bootstrap-server kafka:9092 --topic <topic-name> --create
```

Topics:

* appointment-created
* appointment-canceled
* appointment-completed
* medical-record-created

---

# Authentication Flow

## Register

```http
POST /api/identity/register
```

## Login

```http
POST /api/identity/login
```

Response:

```json
"jwt-token"
```

Use JWT:

```http
Authorization: Bearer <token>
```

---

# Example Workflow

## 1. Login

Get JWT token from Identity Service.

## 2. Create Appointment

```http
POST /api/appointments
```

Patient creates appointment.

## 3. Create Medical Record

```http
POST /api/records
```

Doctor creates medical record only if assigned to appointment.

## 4. Upload File

```http
POST /api/records/{recordId}/files
```

Stored in MongoDB GridFS.

## 5. Download File

```http
GET /api/records/files/{fileId}
```

Secure streaming with access validation.
