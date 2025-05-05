# Fitness App

A Blazor WebAssembly application for tracking health vitals and fitness progress.

## Features

- **Vitals Tracker**: Record and monitor blood sugar levels and blood pressure
- **Fitness Tracker**: Log exercises with sets, reps, and weights

## Technology Stack

- **Frontend**: Blazor WebAssembly (.NET 8)
- **Backend**: ASP.NET Core Web API (.NET 8)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core with Npgsql provider

## Project Structure

The solution follows Clean Architecture principles with three main projects:

1. **FitnessApp.Client**: Blazor WebAssembly frontend
2. **FitnessApp.Server**: ASP.NET Core Web API backend
3. **FitnessApp.Shared**: Shared models and DTOs

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- PostgreSQL database server

### Database Setup

1. Install PostgreSQL if not already installed
2. Create a database named `fitnessapp`
3. Update the connection string in `FitnessApp.Server/appsettings.json` if needed

### Running the Application

1. Clone the repository
2. Navigate to the solution directory
3. Run the following commands:

```bash
# Restore dependencies
dotnet restore

# Apply database migrations
cd FitnessApp.Server
dotnet ef database update

# Run the application
cd ..
dotnet run --project FitnessApp.Server
```

4. Open your browser and navigate to `https://localhost:7001` or the URL shown in the console

## Usage

### Vitals Tracker

- Navigate to the Vitals Tracker page
- Enter blood sugar level (with unit selection: mg/dL or mmol/L)
- Enter blood pressure (systolic and diastolic values)
- View your vitals history in the table

### Fitness Tracker

- Navigate to the Fitness Tracker page
- Add a new exercise by entering its name
- For each exercise, add sets with the following details:
  - Set number
  - Number of repetitions
  - Weight used
- View and manage your exercises and sets
