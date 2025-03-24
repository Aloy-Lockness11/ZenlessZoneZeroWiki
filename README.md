# ZenlessZoneZero Wiki

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#)

## Project Overview
ZenlessZoneZero Wiki is an interactive platform dedicated to providing detailed information on characters, weapons, and user preferences from the ZenlessZoneZero universe.

## Features

### Current
- **Characters**: Detailed character profiles.
- **Weapons**: Comprehensive weapon database.
- **Favorites**: Save and manage your favorite characters and weapons (login required).

### Upcoming
- **Popularity Index**: See character popularity rankings.
- **Total Output Calculator**: Calculate performance metrics for characters.

## Getting Started

### 1. Clone Repository

Clone the repository to your local machine:
```bash
git clone https://github.com/Aloy-Lockness11/ZenlessZoneZeroWiki.git
```

### 2. Docker Setup

Ensure Docker Desktop is installed and updated.

Navigate to the project's root directory:
```bash
cd ZenlessZoneZeroWiki
```

Start Docker containers:
```bash
docker compose up
```

### 3. Database Migration

- Open project in Visual Studio.
- Open NuGet Package Manager Console (`Tools → NuGet Package Manager → Package Manager Console`).
- To create new migrations, use:
```powershell
Add-Migration [MigrationName]
```
- Apply migrations to the database:
```powershell
Update-Database
```

### 4. Firebase Authentication

- Obtain Firebase authentication file .
- Add the file to the project's root folder.

### 5. Running the Application

- Ensure the project is configured for HTTPS in Visual Studio.
- Start the project (`F5` or use `IIS Express`).
- The application opens securely in your browser.

---

You're all set to contribute to ZenlessZoneZero Wiki!

