# StriveUp

[![CI](https://github.com/calKU0/StriveUp/actions/workflows/master_striveupapi.yml/badge.svg)](https://github.com/calKU0/StriveUp/actions/workflows/master_striveupapi.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

StriveUp is a cross-platform activity‐tracking app built with Blazor Hybrid and .NET. Track workouts, earn medals, connect with friends, and view rich activity charts (heart rate, speed, elevation, and more). The solution includes:

- **Blazor Hybrid Web App** (`StriveUp.Web`)
- **.NET MAUI Native App** (`StriveUp.MAUI`)
- **ASP.NET Core API** with Entity Framework Core (`StriveUp.API`)
- **Shared Models/Services** (`StriveUp.Shared`)
- **Infrastructure**: EF Core migrations and database-related tooling (`StriveUp.Infrastructure`)
- **Azure Functions**: External integrations and background jobs (`StriveUp.Functions`)

All services and apps are deployed to Azure App Service, Azure SQL Database, and Azure Functions.

---

## Table of Contents

1. [Features](#features)
2. [Architecture](#architecture)
3. [Tech Stack](#tech-stack)
4. [Repository Structure](#repository-structure)
5. [Prerequisites](#prerequisites)
6. [Getting Started](#getting-started)
7. [Configuration](#configuration)
8. [Running Locally](#running-locally)
9. [External Integrations](#external-integrations)
10. [Deployment](#deployment)
11. [Contributing](#contributing)
12. [License](#license)

---

## Features

- **Activity Tracking**: Record workouts, runs, rides, and more.
- **Medals & Achievements**: Earn badges for milestones and weekly challenges.
- **Social**: Connect with friends, like/comment their activities, and challenge each other.
- **Rich Charts**: View detailed charts for heart rate, speed, elevation, cadence, etc.
- **Multi-Platform**: Accessible via web browser or native mobile app (iOS, Android, Windows).
- **External Sync**: Import data from Garmin Connect, ZEPP OS, and other providers via Azure Functions.

---

## Architecture

```
┌──────────────┐       	┌───────────────┐        ┌────────────────────┐
│ StriveUp.Web │ ◀─────	│  StriveUp.API │ ◀───── │ Azure SQL Database │
│   (Blazor)   │       	│ (ASP.NET Core)│        └────────────────────┘
└──────────────┘       	└───────────────┘       		 ▲
       ▲                        ▲                        │
       │                        │                        │
┌──────────────┐        ┌────────────────┐        ┌───────────────┐
│StriveUp.MAUI │        │ Azure Function │ ─────▶ │ External APIs │
│ (.NET MAUI)  │        │ (Integrations) │        └───────────────┘
└──────────────┘        └────────────────┘
```

---

## Tech Stack

- **Frontend**: Blazor Hybrid (WebAssembly + server), .NET MAUI
- **Backend/API**: ASP.NET Core 9, Entity Framework Core
- **Database**: Azure SQL (Microsoft SQL Server)
- **Serverless**: Azure Functions (.NET 7)
- **CI/CD & Hosting**: Azure DevOps / GitHub Actions → Azure App Service & Functions
- **External APIs**: Unofficial Garmin Connect, ZEPP OS, etc.
- **Cloud Image Management**: User activity images and avatars are stored and served via Cloudinary.
- **Authentication**: Secure API access with JWT Bearer authentication for all endpoints.

---

## Repository Structure

```
StriveUp/
├── StriveUp.sln
├── .gitignore
├── package.json              # for Blazor Web client assets
├── package-lock.json
├── StriveUp.API/             # ASP.NET Core Web API
├── StriveUp.Infrastructure/  # EF Core migrations, domain models
├── StriveUp.Functions/       # Azure Functions
├── StriveUp.Shared/          # DTOs, shared services, shared components
├── StriveUp.Web/             # Blazor Hybrid web project
└── StriveUp.MAUI/            # .NET MAUI native app
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com) or VS Code + C# extensions
- Azure subscription with:
  - App Service Plan
  - Function App
  - Azure SQL Database
- External API credentials (Garmin, ZEPP, etc.)

---

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/calKU0/StriveUp.git
   cd StriveUp
   ```

2. **Restore & Build**

   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the solution**
   - **API & Functions**:
     ```bash
     cd StriveUp.API
     dotnet run
     ```
     In another terminal:
     ```bash
     cd StriveUp.Functions
     func start
     ```
   - **Web App**:
     ```bash
     cd StriveUp.Web
     dotnet run
     ```
   - **Native App**:  
     Open `StriveUp.sln` in Visual Studio, set `StriveUp.MAUI` as startup, and run on emulator/device.

---

## Configuration

All connection strings and secrets are managed in **`appsettings.json`** (local) or in Azure App Service / Function App configuration:

```jsonc
// StriveUp.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=StriveUp;User Id=...;Password=..."
  },
  "Jwt": {
    "Issuer": "...",
    "Audience": "...",
    "Key": "..."
  },
  "ExternalProviders": {
    "Cloudinary": {
      "CloudName": "...",
      "ApiKey": "...",
      "ApiSecret": "..."
    },
    "ZEPP": {
      "ApiKey": "...",
      "ApiSecret": "..."
    }
  }
}
```

- **Azure Functions**:  
  Set `FUNCTIONS_WORKER_RUNTIME`, `AzureWebJobsStorage`, and the same `ConnectionStrings` / provider secrets in your Function App settings.

---

## External Integrations

Azure Functions in `StriveUp.Functions` poll external APIs on schedule or webhook trigger, fetch user activity data, and push it into your app’s database via the API’s endpoints. Supported providers:

- **Garmin Connect**
- **ZEPP OS**
- _...more to come!_

---

## Deployment

1. **Publish API & Web**
   ```bash
   dotnet publish StriveUp.API -c Release
   dotnet publish StriveUp.Web -c Release
   ```
2. **Deploy to Azure App Service**

   - Create two App Services (API & Web).
   - Deploy via CLI, Azure DevOps, or GitHub Actions.

3. **Deploy Azure Functions**

   ```bash
   func azure functionapp publish <YourFunctionAppName>
   ```

4. **Set Configuration**  
   In the Azure Portal, under each App/Function → Configuration, add your connection strings and secrets.

5. **Point Domain & SSL** (optional)

---

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feat/my-feature`)
3. Commit your changes (`git commit -m "Add my feature"`)
4. Push to your branch (`git push origin feat/my-feature`)
5. Open a Pull Request

Please follow the existing code style and include tests where applicable.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

> Built with ❤️ using .NET and Azure
