# Telexistence Remote Monitoring and Control API

This project implements a REST API for remote monitoring and control of Telexistence robots, built with .NET 8. It supports user authentication, command issuance, status tracking, and command history, and is designed for deployment to Azure with CI/CD.

## Table of Contents

1.  [Problem Statement](#problem-statement)
2.  [Features](#features)
3.  [Technical Stack](#technical-stack)
4.  [Project Structure](#project-structure)
5.  [API Endpoints](#api-endpoints)
    *   [Auth](#authentication)
    *   [Command](#commands)
    *   [Robot](#robot)
    *   [Health Check](#health-check)
6.  [Setup and Run Instructions](#setup-and-run-instructions)
    *   [Prerequisites](#prerequisites)
    *   [Local Development](#local-development)
    *   [Configuration](#configuration)
    *   [Running the API](#running-the-api)
    *   [Running Tests](#running-tests)
7.  [Database](#database)
8.  [Authentication & Authorization](#authentication--authorization)
9.  [DevOps](#devops)
    *   [Azure Resource Choice](#azure-resource-choice)
    *   [CI/CD Pipeline (GitHub Actions)](#cicd-pipeline-github-actions)
    *   [Infrastructure as Code (Terraform)](#infrastructure-as-code-terraform)
10. [Logging](#logging)
11. [Future Extensibility](#future-extensibility)
    *   [Real-time Communication](#real-time-communication)
    *   [Logging Dashboard](#logging-dashboard)
12. [Technical Decisions & Highlights](#technical-decisions--highlights)
13. [Time Breakdown (Example)](#time-breakdown-example)

## 1. Problem Statement

Telexistence provides remote operation services using robots. The goal is to build an API that will allow users to interact with a robot for remote monitoring and control.

## 2. Features

*   User authentication and authorization using JWT.
*   Robot command issuance (e.g., move, rotate).
*   Retrieval of current robot status.
*   Retrieval of command history.
*   API deployment to Azure.
*   CI/CD pipeline using GitHub Actions.
*   Infrastructure provisioning using Terraform.
*   Structured logging with Serilog and Azure Application Insights.
*   Health check endpoint.

## 3. Technical Stack

*   **Framework:** .NET 8
*   **Language:** C#
*   **Database:** MongoDB
*   **Authentication:** JWT (JSON Web Tokens)
*   **Containerization:** Docker
*   **CI/CD:** GitHub Actions
*   **Infrastructure as Code:** Terraform
*   **Cloud Provider:** Azure
*   **Logging:** Serilog, Azure Application Insights
*   **Testing:** xUnit, FluentAssertions, Testcontainers.MongoDb
*   **Deployment:** Azure App Service

## 4. Project Structure

```text
├── RobotControlService/              # Main ASP.NET Core Web API project
│   ├── Behaviors/                      # Pipeline behaviors (e.g., validation)
│   ├── Data/                           # Database context, seeding logic 
│   ├── Domain/                         # Domain entities, enums, and value objects 
│   ├── Features/                       # Application features (CQRS handlers, DTOs) 
│   ├── Middleware/                     # Custom middleware (e.g., exception handling) 
│   ├── Exceptions/                     # Exception handling and custom exceptions 
│   ├── Kubernetes/                     # Kubernetes configurations and deployment scripts for local developtment with kind 
│   ├── Program.cs                      # Application entry point and configuration 
│   ├── appsettings.json                # Application configuration 
|   ├── docker-compose.yml              # Multi-container orchestration (mongodb and api) 
│   ├── Dockerfile                      # Docker container definition 
│   └── ...                             # Other supporting files 
├── RobotControlService.Tests/        # Test project (xUnit) 
│   ├── AuthControllerTests.cs          # Authentication endpoint tests 
│   ├── RobotControllerTests.cs         # Robot endpoint tests 
│   └── CommandControllerTests          # Command endpoint tests 
├── .github/workflows/                  # GitHub Actions CI/CD workflows 
├── terraform/                          # Infrastructure as Code (Terraform scripts) 
└── README.md                           # Project documentation
```

## 5. API Endpoints

All endpoints requiring authentication except `Auth/Login` must include an `Authorization: Bearer <JWT_TOKEN>` header.

### Auth

- `POST /api/v1/Auth/Login`  
  Authenticate user and receive JWT token.
  
**Request Body:**
```jsonc
{
  "username": "string",
  "password": "string"
}
```

**Response:**
```jsonc
{
  "token": "string"
}
```

- `POST /api/v1/Auth/CreateUser`  
  Create a new user (admin only).
  
**Request Body:**
```jsonc
{
  "username": "string",
  "password": "string",
  "role": "string",
  "robotIds": ["string"]
}  
```

**Response:**
```jsonc
{
  "userId": "string",
  "username": "string",
  "createdDate": "datetime",
  "robotIds": ["string"],
  "role": "string"
}
```

- `GET /api/v1/Auth/GetUser`  
  Retrieve user details (admin only).
  
**Query:** `username=string`
  
**Response:**
```jsonc
{
  "userId": "string",
  "username": "string",
  "createdDate": "datetime",
  "isDeleted": false,
  "role": "string",
  "robotIds": ["string"]
}
```

- `PUT /api/v1/Auth/UpdateUser`  
  Update user details (admin only).
  
**Request Body:**
```jsonc
{
  "username": "string",
  "newPassword": "string",
  "newRole": "string",
  "robotNames": ["string"]
}  
```

**Response:**
```jsonc
{
  "username": "string",
  "userId": "string",
  "role": "string",
  "robotIds": ["string"]
}
```

- `DELETE /api/v1/Auth/DeleteUser`  
  Remove a user (admin only).
  
**Query:** `username=string`
  
**Response:**
```jsonc
{
  "userId": "string",
  "username": "string"
}
```

### Command

- `POST /api/v1/Command/SendCommand`  
  Issue a command to a robot.
  
**Request Body:**
```jsonc
{                
  "username": "string",
  "robotName": "string",
  "commandType": "string", //"Move", "Rotate"
  "commandParameters": { "key": "value" } // {"distance": "10", "direction": "forward"}
}
```

**Response:**
```jsonc
{
  "commandId": "string",
  "robotName": "string",
  "robotStatus": "string",
  "position": { "x": 0, "y": 0, "orientation": 0 },
  "currentCommandId": "string"
}
```

- `PUT /api/v1/Command/UpdateCommandStatus`  
  Update the status of a command (robot role).
  
**Request Body:**
```jsonc
{
  "commandId": "string",
  "newCommandStatus": "string", // "InProgress", "Completed", "Failed"
  "failureReason": "string"
}  
```

**Response:**
```jsonc
{
  "robotName": "string",
  "robotStatus": "string",
  "position": { "x": 0, "y": 0, "orientation": 0 },
  "currentCommandId": "string"
}
```

- `GET /api/v1/Command/GetCommandHistory`  
  Retrieve command history for a robot.
  
**Query:** `robotName=string&pageIndex=1&pageSize=10`
  
**Response:**
```jsonc
{
  "commandList": {
    "items": [
      {
        "commandId": "string",
        "userId": "string",
        "robotId": "string",
        "commandType": "string",
        "commandStatus": "string",
        "createdDate": "datetime",
        "startedDate": "datetime",
        "complatedDate": "datetime",
        "commandParameters": { "key": "value" },
        "failureReason": "string"
      }
    ],
    "pageIndex": 0,
    "totalPages": 0,
    "totalCount": 0
    }
}
```

### Robot

- `POST /api/v1/Robot/CreateRobot`  
  Register a new robot.
  
**Request Body:**
```jsonc
{
  "name": "string",
  "description": "string",
  "position": { "x": 0, "y": 0, "orientation": 0 }
}  
```

**Response:**
```jsonc
{
  "robotId": "string",
  "createdDate": "2024-01-01T00:00:00Z",
  "name": "string",
  "description": "string",
  "isDeleted": false,
  "status": "string",
  "position": { "x": 0, "y": 0, "orientation": 0 }
}
```

- `GET /api/v1/Robot/GetRobotStatus`  
  Get current status of a robot.
  
**Query:** `robotName=string`
  
**Response:**
```jsonc
{
  "robotName": "string",
  "robotStatus": "string",
  "position": { "x": 0, "y": 0, "orientation": 0 },
  "currentCommandId": "string"
}
```

- `PUT /api/v1/Robot/UpdateRobot`  
  Update robot details.

**Request Body:**
```jsonc
{
  "name": "string",
  "newDescription": "string"
}  
```

**Response:**
```jsonc
{
  "name": "string",
  "description": "string",
  "id": "string"
}
```

- `DELETE /api/v1/Robot/DeleteRobot`  
  Remove a robot.
  
**Query:** `robotName=string`
  
**Response:**
```jsonc
{
  "robotId": "string",
  "name": "string",
  "isDeleted": true
}
```

### Health Check

- `GET /health`  
  Returns API health status.

## 6. Setup and Run Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for local development)
- [MongoDB](https://www.mongodb.com/) (local or Docker)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- [Terraform](https://www.terraform.io/downloads.html) (for infrastructure provisioning)
- [Kind](https://kind.sigs.k8s.io/) (for local Kubernetes clusters)

### Local Development

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Admiralkheir/RobotControlService.git
    ```
2.  **Set up MongoDB using Docker (for local development):**
    Comment `api` service in `docker-compose.yml` if you want to use only MongoDB instance.
    ```bash
    docker-compose up -d
    ```
3.  **Restore dependencies and build:**
    ```bash
    dotnet restore
    dotnet build
    ```
 
### Configuration

*   **`appsettings.json` / `appsettings.Development.json`:** Main application configuration.
*   **Environment Variables:** For Azure deployment, connection strings, JWT secrets, and Application Insights keys could be configured via App Service Application Settings or Azure Key Vault.

**Security Note:** For `JwtSettings`, we could use Azure Key Vault

### Running the API

*   **Using .NET CLI:**
    ```bash
    cd RobotControlService
    dotnet run
    ```
    The API will typically be available at `http://localhost:5XXX`.

*   **Using docker-compose:**
    This docker-compose.yaml will start both the API and a MongoDB instance for local development.
    ```bash
    cd RobotControlService
    docker-compose up
    ```
    *(Note: `host.docker.internal` allows the container to connect to services running on the host, like the MongoDB container started with docker-compose.)*

*   **Using kind:**
    1. Create a kind cluster
       ```sh
       kind create cluster --config .\Kubernetes\kind\kubernetes-cluster-config.yaml
       ```
    2. Build docker image
    
       ```sh
       docker build -t robotcontrolservice:0.1 .
       ```
    3. Load image to kind cluster
       ```sh
       kind load docker-image robotcontrolservice:0.1
       ```
    4. Deploy the project
       ```sh
       kubectl apply -f .\Kubernetes\kubernetes-deployment.yaml
       ```
    5. Port forward to access the service
       ```sh
       kubectl port-forward deployment/robotcontrolservice-deployment -n test 8080:8080
       ```
### Running Tests

1.  **Tests:**
    `Testcontainers.MongoDb` package is used for tests with MongoDB. This package will automatically start a MongoDB container for testing purposes.
    ```bash
    cd RobotControlService.Tests
    dotnet test
    ```

## 7. Database

*   **MongoDB** is used for its schema flexibility, which is beneficial for storing command history and evolving robot status data. Also Telexistence's existing infrastructure is based on MongoDB.
*   **Testcontainers.MongoDb** package is used for tests with MongoDB. This package will automatically start a MongoDB container for testing purposes.
*   **Data Models (conceptual):**
    *   `Users`: 
     ```jsonc
        {
           "_id": "ObjectId",            // MongoDB ObjectId
           "createdDate": "datetime",    // User creation date (UTC)
           "username": "string",         // Unique username
           "passwordHash": "string",     // Hashed password
           "isDeleted": false,           // Soft delete flag
           "role": "string",             // "Admin" | "Operator" | "Monitor" | "Robot"
           "robotIds": ["ObjectId"]      // (optional) Array of Robot ObjectIds
        }
     ``` 
    *   `Robots`:
     ```jsonc
        {
            "_id": "ObjectId",                // MongoDB ObjectId
            "createdDate": "datetime",        // Robot creation date (UTC)
            "name": "string",                 // Unique robot name
            "description": "string",          // Description of the robot
            "isDeleted": false,               // Soft delete flag
            "status": "string",               // "Idle" | "Moving" | "Rotating"
            "currentPosition": {              // Current position of the robot
              "x": 0.0,
              "y": 0.0,
              "orientation": 0.0
            },
            "lastSeenDate": "datetime",       // (optional) Last seen timestamp
            "currentCommandId": "ObjectId"    // (optional) Current command ObjectId
        }
     ```
     *   `Commands`:
      ```jsonc
         {
             "_id": "ObjectId",                  // MongoDB ObjectId
             "createdDate": "datetime",          // Command creation date (UTC)
             "startedDate": "datetime",          // (optional) When command started
             "completedDate": "datetime",        // (optional) When command completed
             "robotId": "ObjectId",              // Target robot ObjectId
             "userId": "ObjectId",               // Issuing user ObjectId
             "commandStatus": "string",          // "Queued" | "InProgress" | "Completed" | "Failed"
             "commandType": "string",            // "Move" | "Rotate"
             "commandParameters": {              // Command parameters (key-value pairs)
               "key": "value"
             },
             "failureReason": "string"           // (optional) Reason for failure
         }
      ```

## 8. Authentication & Authorization

*   Authentication is handled via **JWT (JSON Web Tokens)**.
*   The `/login` endpoint accepts username and password, validates them (e.g., against a user store in MongoDB), and if successful, issues a signed JWT.
*   Subsequent requests to protected endpoints must include the JWT in the `Authorization: Bearer <token>` header.
*   ASP.NET Core's built-in JWT Bearer authentication middleware is used to validate tokens.
*   Authorization is role-based (e.g., `[Authorize(Roles = "Operator")]`).

## 9. DevOps

### Azure Resource Choice

*   **Primary Compute Resource: Azure App Service (for Containers)**
    *   **Reasoning:**
        *   **PaaS:** Simplifies deployment and management, handles patching, scaling, and availability.
        *   **.NET Optimized:** Excellent support for .NET applications.
        *   **Container Support:** Can deploy the Dockerized API directly.
        *   **Integration:** Seamless integration with Azure Monitor (Application Insights), Azure DevOps/GitHub Actions for CI/CD, Azure Key Vault for secrets.
        *   **Scalability:** Easy to scale up/out based on demand.
        *   **Deployment Slots:** Allows for staging and testing new versions before swapping to production.
*   **Logging/Monitoring: Azure Application Insights**
    *   **Reasoning:** Deep integration with .NET and Azure App Service for performance monitoring, distributed tracing, and log aggregation.
*   **Other Resources:**
    *   **Azure Resource Group:** To organize all related resources.
    *   **Azure Container Registry (ACR):** To store Docker images (if deploying containers).
    *   **Azure Key Vault:** To securely store secrets like DB connection strings and JWT keys.

### CI/CD Pipeline (GitHub Actions)

A `/.github/workflows/ci-cd.yml` file will define the pipeline.

**Trigger:** On push to `main` branch or on creation of a pull request targeting `main`.

**Pipeline Steps:**
