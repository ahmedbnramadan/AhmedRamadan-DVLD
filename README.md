# DVLD — Driving & Vehicle Licensing Department

A desktop management system for organizing driving and vehicle licensing operations, built with C#, Windows Forms, and Microsoft SQL Server.

The project focuses on applying object-oriented programming and layered application architecture to a practical management-system scenario, covering people, users, drivers, applications, tests, and driving licenses.

**Repository:** [AhmedRamadan-DVLD](https://github.com/ahmedbnramadan/AhmedRamadan-DVLD)

---

## Table of Contents

- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Application Architecture](#application-architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Technical Highlights](#technical-highlights)
- [Development Notes](#development-notes)
- [Project Purpose](#project-purpose)

---

## Project Overview

DVLD is a desktop application designed to simulate the core operations of a Driving and Vehicle Licensing Department.

It brings together several related workflows in one system, including people management, user administration, driving license applications, testing procedures, and license services.

The application is developed using C# and Windows Forms, with Microsoft SQL Server as its database platform. Its codebase separates data access and business operations into dedicated layers to improve organization, maintainability, and separation of responsibilities.

This is a personal software development project built as part of my practical learning journey in C# and .NET development.

## Key Features

### 1. People Management
- Manage people records.
- Search for and retrieve people using available identification information.
- View detailed person information.
- Reuse person-related UI components across different forms.

### 2. User Management
- User authentication before accessing the main application.
- Manage system users.
- View current user information.
- Change account passwords.
- Log out and return to the login screen.

### 3. Driving License Applications
- Manage local driving license applications.
- Manage international driving license applications.
- Handle new local and international license application workflows.
- Manage application types.

### 4. Testing Management
- Manage test types.
- Support testing-related workflows associated with local driving license applications.
- Handle test retake workflows.

### 5. Driving License Services
- Renew local driving licenses.
- Replace lost or damaged driving licenses.
- Detain driving licenses.
- Release detained driving licenses.
- Manage detained license records.

### 6. Main Application Interface
- MDI-based main application window.
- Organized menu navigation.
- Centralized access to application modules.
- Reuse of already-open MDI child forms where applicable.

---

## Application Architecture

The application follows a layered architecture that separates responsibilities between the user interface, business operations, and database communication.

```text
+----------------------------------+
|       Presentation Layer         |
|          Windows Forms           |
|                                  |
|  Forms, User Controls, UI Events  |
+----------------+-----------------+
                 |
                 v
+----------------------------------+
|          Business Layer          |
|                                  |
|  Business Entities and Rules     |
|  Application Operations          |
+----------------+-----------------+
                 |
                 v
+----------------------------------+
|        Data Access Layer         |
|                                  |
|  SQL Queries and Data Operations |
|  Database Communication          |
+----------------+-----------------+
                 |
                 v
+----------------------------------+
|          Database                |
|                                  |
|         Microsoft SQL Server     |
+----------------------------------+
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| Presentation | Handles the user interface, user input, navigation, and interaction with the application. |
| Business | Encapsulates business operations and coordinates between the presentation and data access layers. |
| Data Access | Handles database communication and data retrieval and modification. |
| Database | Stores and manages the application's persistent data. |

This separation helps keep responsibilities organized and makes individual parts of the application easier to maintain and extend.

---

## Technology Stack

| Category | Technologies |
|---|---|
| Programming Language | C# |
| Desktop UI | Windows Forms |
| Framework | .NET 5.0 (project configuration) |
| Database | Microsoft SQL Server |
| Data Access | Microsoft.Data.SqlClient |
| Architecture | Layered Architecture (Three-Tier) |
| Programming Concepts | Object-Oriented Programming (OOP) |
| Version Control | Git and GitHub |

---

## Project Structure

The repository contains separate projects for data access, business logic, and console-based interaction, alongside the Windows Forms application source.

```text
AhmedRamadan-DVLD/
│
├── Business/
│   └── Business.csproj
│
├── DataAccess/
│   └── DataAccess.csproj
│
├── DVLD/
│   └── Windows Forms application source
│
├── DVLD_ConsoleUI/
│   └── DVLD_ConsoleUI.csproj
│
├── DVLD.sln
│
└── Documentation.md
```

**Note:** The structure above represents the main project directories. The current solution file does not yet register the Windows Forms project, so the solution configuration needs to be aligned with the application structure.

---

## Technical Highlights

The project demonstrates practical application of several software development concepts:

- **Separation of Concerns:** Dividing responsibilities between the presentation, business, and data access layers.
- **Object-Oriented Programming:** Organizing application functionality around classes, objects, and reusable components.
- **Reusable User Controls:** Building reusable UI components to reduce duplication and maintain consistent interactions.
- **Event-Driven Programming:** Handling user interactions through Windows Forms events.
- **Database Operations:** Separating SQL-related operations from presentation code.
- **Application Navigation:** Organizing multiple application modules through an MDI interface.
- **Input Handling and Validation:** Applying validation and controlled user interactions within application workflows.

---

## Development Notes

This repository reflects an ongoing learning and development process.

The project is being developed incrementally, with attention to code organization, reusable components, maintainability, and practical application of C# and .NET concepts.

Some configuration and documentation details may evolve as development continues.

---

## Project Purpose

The main objective of this project is to strengthen practical software development skills by building a multi-module desktop application rather than focusing only on isolated programming exercises.

It provides hands-on experience with:

- C# application development.
- Windows Forms development.
- Object-oriented programming.
- SQL Server database interaction.
- Layered application architecture.
- Designing and implementing business-oriented workflows.

---

**Developed by Ahmed Ramadan**

[GitHub Profile](https://github.com/ahmedbnramadan)
