# TeraCyte Live Image Viewer

## Introduction

This repository contains the source code for the **TeraCyte Home Assignment**.  
The project is a **Windows Presentation Foundation (WPF)** application developed with **.NET 8**, designed to connect to the TeraCyte backend API.

It handles **JWT authentication**, periodically polls for **live microscope image data** and associated inference results, and displays this information in a clear and interactive user interface, including a **scrollable history** of all received data.

---

## Demo

🎥 **Watch the Live Demo**  
[Click here to view the demo video](./Demo/TeraCyte%20Live%20Image%20Viewer%202025-08-02%2011-36-03.mp4)

---

## Features

- **JWT Authentication with Auto-Refresh**  
  Performs login to acquire JWT access and refresh tokens. Automatically handles token expiration to ensure continuous operation.

- **Resilient Data Polling**  
  A background service combines a polling timer with a retry policy to ensure reliable data fetching, even in the face of network errors.

- **Scrollable History View**  
  All received image/result pairs are stored and shown in chronological order. Users can view previous entries in full detail.

- **Graceful Error Handling**  
  Displays clear user feedback during authentication, data fetching, and error states.

---

## Architectural Overview

### MVVM (Model-View-ViewModel)

- **Model**: C# `record` types define the structure of API requests and responses.
- **View**: XAML files with no business logic, fully bound to ViewModels.
- **ViewModel**:
  - `MainViewModel`: Manages application state and operations.
  - `ImageResultPairViewModel`: Handles each history item, including image asset logic.

### Dependency Injection (DI) and Singleton Services

- Configured via `Microsoft.Extensions.DependencyInjection`.
- Managed in a central `Bootstrapper` class.

#### Singleton Lifetime

All services and the `HttpClient` are registered as **singletons**:
- Ensures consistent shared state (e.g., `IAuthService` token management).
- Improves performance by reusing the same `HttpClient` instance (socket reuse).

---

## Service Layer

External communication and business logic are abstracted into clearly defined services:

- `IAuthService`:  
  Manages login and token lifecycle.

- `IApiService`:  
  Handles HTTP communication, including 401 Unauthorized responses.

- `IDataPollingService`:  
  Contains polling logic, retry strategies, and data validation.

---

## Data Polling and Retry Logic

The polling system follows a **two-stage resilience strategy**:

### 1. Polling Timer

A `DispatcherTimer` initiates a polling cycle every **3 seconds**, serving as the app's heartbeat.

### 2. Resilience with Polly

Once a cycle starts:
- The **Polly** library applies a retry policy.
- Up to **3 retries** are attempted with a **1-second delay** between each.
- Covers:
  - API request failures
  - Invalid or missing `image_id`
  - Image decode errors

This approach ensures recovery from transient network/server issues without manual retry handling.

---

## Configuration-Driven

Settings are externalized in `appsettings.json`, including:

- API endpoints  
- User credentials  

---

## Setup and Installation

### Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (or later)  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### How to Run

1. Clone this repository to your local machine.
2. Open `TeraCyteAssignment.sln` in Visual Studio.
3. Restore NuGet packages (if not done automatically):  
   - Right-click the solution → **Restore NuGet Packages**
4. Press `F5` or go to **Debug > Start Debugging** to launch the app.
5. Click **Connect** to authenticate and begin polling for data.

---

## NuGet Dependencies

This project uses the following third-party libraries:

- [`CommunityToolkit.Mvvm`](https://www.nuget.org/packages/CommunityToolkit.Mvvm)  
  Modern MVVM utilities with source-generated boilerplate.

- [`Microsoft.Extensions.DependencyInjection`](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)  
  Built-in .NET Dependency Injection.

- [`Microsoft.Extensions.Http`](https://www.nuget.org/packages/Microsoft.Extensions.Http)  
  DI support for `HttpClient`.

- [`Microsoft.Extensions.Configuration.Json`](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json)  
  JSON-based configuration loading.

- [`Microsoft.Extensions.Configuration.FileExtensions`](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.FileExtensions)  
  Support for file-based config providers.

- [`Polly`](https://www.nuget.org/packages/Polly)  
  Resilience and transient-fault-handling for .NET — used for automatic retries.
