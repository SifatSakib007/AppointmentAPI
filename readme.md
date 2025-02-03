# Appointment Management API Documentation

This document provides a comprehensive guide to the **Appointment Management API**, a RESTful service built using **.NET Core**. 
The API is designed to manage patient appointments for a healthcare clinic, featuring **JWT-based authentication** to ensure secure access to protected endpoints.

---

## Table of Contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Technologies Used](#technologies-used)
4. [API Endpoints](#api-endpoints)
5. [Authentication](#authentication)
6. [Database Schema](#database-schema)
7. [Setup Instructions](#setup-instructions)
8. [Testing the API](#testing-the-api)
9. [Error Handling](#error-handling)
10. [Bonus Features](#bonus-features)
11. [Submission Details](#submission-details)
12. [FAQs](#faqs)
13. [Support](#support)

---

## Introduction
The **Appointment Management API** is a robust solution for managing patient appointments in a healthcare clinic. It provides endpoints for user authentication and appointment management, 
ensuring secure and efficient operations. The API is built with scalability and security in mind, leveraging modern technologies and best practices.

---

## Features
- **User Authentication**:
  - Register new users.
  - Login and generate JWT tokens.
- **Appointment Management**:
  - Create, read, update, and delete appointments.
  - Validate appointment dates (must be in the future).
- **Secure Access**:
  - All appointment-related endpoints require a valid JWT token.
- **Database Integration**:
  - Uses **Entity Framework Core** with **MSSQL** for data persistence.
- **Error Handling**:
  - Proper error messages for invalid inputs, missing data, or unauthorized access.

---

## Technologies Used
- **Backend Framework**: ASP.NET Core WebAPI
- **Database**: MSSQL with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.Net
- **Testing**: NUnit, Moq, and Postman
- **Logging**: Console logging for debugging

---

Here’s the modified **API Endpoints** section with clearer instructions on how to register, log in, and use the JWT token to access protected endpoints. 
The steps are explicitly outlined for better clarity and usability.

---

## API Endpoints

### 1. User Authentication

#### Register a User
- **Endpoint**: `POST /api/Auth/register`
- **Request Body**:
  ```json
  {
    "username": "testuser",
    "password": "Password123"
  }
  ```
- **Response**:
  ```json
  {
    "message": "User registered successfully"
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `POST`.
  3. Enter the URL: `http://localhost:5087/api/Auth/register`.
  4. Go to the **Body** tab, select **raw**, and choose **JSON**.
  5. Paste the request body JSON (as shown above).
  6. Click **Send**.
  7. If successful, you will receive a confirmation message.

---

#### Login
- **Endpoint**: `POST /api/Auth/login`
- **Request Body**:
  ```json
  {
    "username": "testuser",
    "password": "Password123"
  }
  ```
- **Response**:
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "message": "JWT Token generated successfully"
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `POST`.
  3. Enter the URL: `http://localhost:5087/api/Auth/login`.
  4. Go to the **Body** tab, select **raw**, and choose **JSON**.
  5. Paste the request body JSON (as shown above).
  6. Click **Send**.
  7. If successful, you will receive a JWT token in the response. Copy this token for future requests.

---

### 2. Appointment Management

#### Create an Appointment
- **Endpoint**: `POST /api/Appointment`
- **Headers**:
  - `Authorization: Bearer <token>`
- **Request Body**:
  ```json
  {
    "patientName": "John Doe",
    "patentContact": "john@example.com",
    "appointmentDateTime": "2025-01-15T10:30:00",
    "doctorId": 1
  }
  ```
- **Response**:
  ```json
  {
    "id": 1,
    "patientName": "John Doe",
    "patentContact": "john@example.com",
    "appointmentDateTime": "2025-01-15T10:30:00",
    "doctorId": 1
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `POST`.
  3. Enter the URL: `http://localhost:5087/api/Appointment`.
  4. Go to the **Headers** tab.
  5. Add a new header:
     - **Key**: `Authorization`
     - **Value**: `Bearer <paste-your-token-here>`
  6. Go to the **Body** tab, select **raw**, and choose **JSON**.
  7. Paste the request body JSON (as shown above).
  8. Click **Send**.
  9. If successful, you will receive the created appointment details.

---

#### Get All Appointments
- **Endpoint**: `GET /api/Appointment`
- **Headers**:
  - `Authorization: Bearer <token>`
- **Response**:
  ```json
  [
    {
      "id": 1,
      "patientName": "John Doe",
      "patentContact": "john@example.com",
      "appointmentDateTime": "2025-01-15T10:30:00",
      "doctorId": 1
    },
    {
      "id": 2,
      "patientName": "Jane Smith",
      "patentContact": "jane@example.com",
      "appointmentDateTime": "2025-01-16T11:00:00",
      "doctorId": 2
    }
  ]
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `GET`.
  3. Enter the URL: `http://localhost:5087/api/Appointment`.
  4. Go to the **Headers** tab.
  5. Add a new header:
     - **Key**: `Authorization`
     - **Value**: `Bearer <paste-your-token-here>`
  6. Click **Send**.
  7. If successful, you will receive a list of all appointments.

---

#### Get Appointment by ID
- **Endpoint**: `GET /api/Appointment/{id}`
- **Headers**:
  - `Authorization: Bearer <token>`
- **Response**:
  ```json
  {
    "id": 1,
    "patientName": "John Doe",
    "patentContact": "john@example.com",
    "appointmentDateTime": "2025-01-15T10:30:00",
    "doctorId": 1
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `GET`.
  3. Enter the URL: `http://localhost:5087/api/Appointment/1` (replace `1` with the desired appointment ID).
  4. Go to the **Headers** tab.
  5. Add a new header:
     - **Key**: `Authorization`
     - **Value**: `Bearer <paste-your-token-here>`
  6. Click **Send**.
  7. If successful, you will receive the details of the specified appointment.

---

#### Update an Appointment
- **Endpoint**: `PUT /api/Appointment/{id}`
- **Headers**:
  - `Authorization: Bearer <token>`
- **Request Body**:
  ```json
  {
    "patientName": "John Doe",
    "patentContact": "john@example.com",
    "appointmentDateTime": "2025-01-15T12:00:00",
    "doctorId": 2
  }
  ```
- **Response**:
  ```json
  {
    "id": 1,
    "patientName": "John Doe",
    "patentContact": "john@example.com",
    "appointmentDateTime": "2025-01-15T12:00:00",
    "doctorId": 2
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `PUT`.
  3. Enter the URL: `http://localhost:5087/api/Appointment/1` (replace `1` with the desired appointment ID).
  4. Go to the **Headers** tab.
  5. Add a new header:
     - **Key**: `Authorization`
     - **Value**: `Bearer <paste-your-token-here>`
  6. Go to the **Body** tab, select **raw**, and choose **JSON**.
  7. Paste the request body JSON (as shown above).
  8. Click **Send**.
  9. If successful, you will receive the updated appointment details.

---

#### Delete an Appointment
- **Endpoint**: `DELETE /api/Appointment/{id}`
- **Headers**:
  - `Authorization: Bearer <token>`
- **Response**:
  ```json
  {
    "message": "Appointment deleted successfully"
  }
  ```
- **Steps**:
  1. Open **Postman**.
  2. Set the request type to `DELETE`.
  3. Enter the URL: `http://localhost:5087/api/Appointment/1` (replace `1` with the desired appointment ID).
  4. Go to the **Headers** tab.
  5. Add a new header:
     - **Key**: `Authorization`
     - **Value**: `Bearer <paste-your-token-here>`
  6. Click **Send**.
  7. If successful, you will receive a confirmation message.

---
## Database Schema
### Tables
1. **Users**:
   - `UserId` (Primary Key)
   - `Username`
   - `PasswordHash` (hashed using BCrypt)
2. **Doctors**:
   - `DoctorId` (Primary Key)
   - `DoctorName`
3. **Appointments**:
   - `AppointmentId` (Primary Key)
   - `PatientName`
   - `PatientContact`
   - `AppointmentDateTime`
   - `DoctorId` (Foreign Key)

---

## Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd AppointmentAPI
   ```
2. **Install Dependencies**:
   ```bash
   dotnet restore
   ```
3. **Configure Database**:
   - Update the `DefaultConnection` in `appsettings.json` with your MSSQL connection string.
   - Run migrations:
     ```bash
     dotnet ef database update
     ```
4. **Run the Application**:
   ```bash
   dotnet run
   ```
5. **Access the API**:
   - The API will be available at `http://localhost:5087`.

---

Here’s the **Testing the API** section rewritten in detail, incorporating your NUnit test code and explaining how the tests are structured and executed. This section is designed to be clear, concise, and informative for developers who want to understand and run the tests.

---

## Testing the API

The API includes a comprehensive suite of **unit tests** written using **NUnit** and **Moq**. These tests ensure the functionality of the API endpoints, including user authentication and appointment management. The tests use an **in-memory database** for isolated and efficient testing.

---

### Test Setup

#### Test Project Structure
- The test project is named `Test`.
- It contains two test classes:
  1. **`AppointmentControllerTest`**: Tests the appointment-related endpoints.
  2. **`AuthControllerTest`**: Tests the authentication-related endpoints.

#### Key Dependencies
- **NUnit**: For writing and running unit tests.
- **Moq**: For mocking dependencies like `IAuthService`.
- **Entity Framework Core In-Memory Database**: For simulating database operations without requiring a real database.

---

### Running the Tests

To run the tests, use the following command in the terminal:

```bash
dotnet test
```

This command executes all the unit tests in the `Test` project and provides a summary of the results.

---

### Best Practices
- **Isolation**: Each test is isolated and does not depend on the state of other tests.
- **Cleanup**: The `TearDown` method ensures the in-memory database is reset after each test.
- **Readability**: Test methods are named descriptively to indicate their purpose.

---

This detailed **Testing the API** section provides a clear guide for running, understanding, and extending the unit tests. It aligns with industry standards and ensures the documentation is both informative and actionable.

---

## Error Handling
- **400 Bad Request**: Invalid input (e.g., past appointment date).
- **401 Unauthorized**: Missing or invalid JWT token.
- **404 Not Found**: Resource not found (e.g., invalid appointment ID).
- **500 Internal Server Error**: Server-side errors.

---

## Bonus Features
- **Input Validation**:
  - Appointment dates must be in the future.
- **Password Hashing**:
  - Passwords are securely hashed using BCrypt.
- **Automated Tests**:
  - Unit tests for controllers and services.
- **Error Handling**:
  - Custom error messages for invalid inputs or missing data.

