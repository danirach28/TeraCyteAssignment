# TeraCyte Live Image Viewer

## Introduction

This repository contains the source code for the TeraCyte Home Assignment. The project is a Windows Presentation Foundation (WPF) application developed with .NET 8, designed to connect to the TeraCyte backend API. It handles JWT authentication, periodically polls for live microscope image data and associated inference results, and displays this information in a clear and interactive user interface, including a scrollable history of all received data.


## Demo

🎥 **Watch the Live Demo**  
[Click here to view the demo video](./Demo/TeraCyte%20Live%20Image%20Viewer%202025-08-02%2011-36-03.mp4)

## Features

- **JWT Authentication with Auto-Refresh**  
  Performs login to acquire JWT access and refresh tokens. Automatically handles token expiration to ensure continuous operation.

- **Resilient Data Polling**  
  A background service combines a polling timer with a retry policy to ensure reliable data fetching, even in the face of network errors.

- **Scrollable History View**  
  All received image/result pairs are stored and shown in chronological order. Users can view previous entries in full detail.

- **Graceful Error Handling**  
  Displays clear user feedback during authentication, data fetching, and error states.


## Setup and Installation

### Prerequisites

* Visual Studio 2022 (or later)
* .NET 8 SDK

### Configuration

This project uses an `appsettings.json` file for configuration, which is not committed to the repository for security reasons. To configure the application, follow these steps:

1.  Find the `appsettings.example.json` file in the main project directory.
2.  Create a copy of this file and rename it to **`appsettings.json`**.
3.  Open the new `appsettings.json` file and replace the placeholder values with the actual credentials provided for the assignment.

### How to Run

1.  Clone this repository to your local machine.
2.  Complete the configuration steps above to create your `appsettings.json` file.
3.  Open the `TeraCyteAssignment.sln` solution file in Visual Studio.
4.  Visual Studio should automatically restore the required NuGet packages. If not, right-click the solution in the Solution Explorer and select "Restore NuGet Packages".
5.  Press `F5` or select "Debug > Start Debugging" from the menu to build and run the application.
6.  Upon launch, click the "Connect" button to initiate the authentication process and begin polling for data.



## Architectural Overview

To ensure the application is robust and easy to maintain, its architecture is based on a clear separation of concerns using the following patterns and principles:

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


### Service Layer

External communication and business logic are abstracted into clearly defined services:

- `IAuthService`:  
  Manages login and token lifecycle.

- `IApiService`:  
  Handles HTTP communication, including 401 Unauthorized responses.

- `IDataPollingService`:  
  Contains polling logic, retry strategies, and data validation.

### Project Structure

```
/TeraCyteAssignment/
│
├── TeraCyteAssignment.csproj
│
├── App.xaml
├── App.xaml.cs
├── Bootstrapper.cs
│
├── appsettings.json          # (Local, not committed) Your private configuration.
├── appsettings.example.json  # (Committed) An example template for configuration.
│
├── Configuration/
│   └── Settings.cs           # C# classes for strongly-typed configuration.
│
├── Converters/
│   └── ... (Value converters for the UI)
│
├── Models/
│   └── DataModels.cs         # C# records for API requests and responses.
│
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs        # Handles authentication logic.
│   ├── IApiService.cs
│   ├── ApiService.cs         # Handles API communication.
│   ├── IDataPollingService.cs
│   └── DataPollingService.cs # Manages the background polling and retry logic.
│
├── ViewModels/
│   ├── MainViewModel.cs      # Main ViewModel for the application.
│   └── ImageResultPairViewModel.cs # ViewModel for a single history item.
│
└── Views/
    └── MainWindow.xaml       # The main application window and its code-behind.
```
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

## NuGet Dependencies

This project utilizes the following third-party libraries:

* **`CommunityToolkit.Mvvm`**: A modern MVVM toolkit providing source-generated observable properties and commands to reduce boilerplate code.
* **`Microsoft.Extensions.DependencyInjection`**: The standard library for implementing Dependency Injection in .NET.
* **`Microsoft.Extensions.Http`**: Provides integration between `HttpClient` and the Dependency Injection container.
* **`Microsoft.Extensions.Configuration.Json`**: Enables loading configuration from JSON files.
* **`Microsoft.Extensions.Configuration.FileExtensions`**: Provides extension methods for file-based configuration providers.
* **`Polly`**: A comprehensive .NET resilience and transient-fault-handling library, used to implement the automatic retry policy.
