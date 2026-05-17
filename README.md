# Instrux — Teacher Classroom Management Tool

Instrux is a WPF desktop application for teachers to manage classroom data — classes, students, attendance, assessments, grades, content resources, calendar events, and tasks — backed by a local SQL Server (LocalDB) database.

---

## Architecture

The project follows **Clean Architecture** with 4 layers + tests:

```
┌──────────────────────────────────────────────┐
│           Instrux.App (WPF)                  │  UI Layer
│   Views, ViewModels, DI, AppDataStore        │
├──────────────────────────────────────────────┤
│        Instrux.Services (Logic)              │  Application Layer
│   DTOs, Validators, Service implementations  │
├──────────────────────────────────────────────┤
│      Instrux.Infrastructure (Data)           │  Infrastructure Layer
│   DbContext, Repositories, EF Configs        │
├──────────────────────────────────────────────┤
│         Instrux.Domain (Core)                │  Domain Layer
│   Models, Interfaces                         │
├──────────────────────────────────────────────┤
│        Instrux.App.Tests (xUnit)             │  Tests
│   12 tests (Service CRUD, Seed data)         │
└──────────────────────────────────────────────┘
```

### Layers

**Domain** — Inner core with zero dependencies. Contains domain models (entities), repository interfaces (`IRepository<T>`), unit of work interface (`IUnitOfWork`), and shared abstractions.

**Services** — Application logic layer. Depends only on Domain. Contains 9 service implementations with full CRUD, 16 DTO classes (read + create variants), 9 FluentValidation validators, manual entity-to-DTO mapping, and `Result<T>` pattern for consistent operation outcomes.

**Infrastructure** — Data access layer. Depends on Domain and Services. Contains EF Core `AppDbContext` with 9 `DbSet<T>` properties, Fluent API entity configurations, `Repository<T>` / `UnitOfWork` implementations, `AuditableEntityInterceptor` for audit fields, EF Core migrations, and `DbInitializer` for seeding demo data on first run.

**App** — WPF UI layer. Depends on Services and Infrastructure. Uses `Microsoft.Extensions.Hosting` for DI and Serilog for logging. Key components:
- `AppDataStore` singleton — all data loaded once from DB into `ObservableCollection`s at startup, shared across all ViewModels
- `MainWindow` with dark sidebar navigation (Dashboard, My Classes, Calendar, To-Do, Settings)
- Class detail view with sub-tab navigation (Roster, Attendance, Materials, Grades)
- `CalendarViewModel` / `TodoViewModel` for full-page card-based views
- `OnboardingWindow` shown on first launch to create a teacher profile
- `SettingsProfileView` for editing teacher info
- Syncfusion `Windows11Light` theme via SfSkinManager

**Tests** — xUnit test project using EF Core InMemory database. 12 tests covering all service CRUD operations and seed data verification.

---

## Patterns Used

| Pattern | Where |
|---|---|
| **Clean Architecture** | 4-project solution with strict dependency flow |
| **Repository Pattern** | `IRepository<T>` / `Repository<T>` abstracts EF Core |
| **Unit of Work** | `IUnitOfWork` wraps `SaveChangesAsync` |
| **Result Pattern** | `Result<T>` / `Result` classes avoid exceptions for expected failures |
| **MVVM** | WPF pattern with `CommunityToolkit.Mvvm` source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| **Dependency Injection** | `IHost` / `IServiceCollection` via `Microsoft.Extensions.Hosting` |
| **DTO Pattern** | Separate read DTOs and create DTOs per entity |
| **Manual Mapping** | Private static `MapToDto` / `MapToEntity` / `ApplyDto` methods per service |
| **Singleton Data Store** | `AppDataStore` — in-memory shared `ObservableCollection`s, single DB load at startup |
| **FluentValidation** | 9 validators for create operations |
| **Fluent API** | EF Core entity configurations (separate `IEntityTypeConfiguration<T>` classes) |
| **Interceptor Pattern** | EF Core `SaveChangesInterceptor` for audit fields |
| **Auto Migration** | `context.Database.MigrateAsync()` runs on every startup |
| **Seed Data** | `DbInitializer` seeds demo data on first run |

---

## Tech Stack

| Technology | Version |
|---|---|
| .NET SDK | 10.0.203 |
| Target Framework | `net10.0` / `net10.0-windows` |
| WPF | Windows Presentation Foundation |
| Entity Framework Core | 10.0.0-preview.7 |
| SQL Server | LocalDB (`MSSQLLocalDB`) |
| CommunityToolkit.Mvvm | 8.4.2 |
| FluentValidation | 12.1.1 |
| Serilog | 8.0+ |
| Microsoft.Extensions.Hosting | 10.0.8 |
| Syncfusion SfSkinManager | 33.2.6 |
| xUnit | 2.9.3 |

---

## Project Structure

```
Instrux.App/
├── .gitignore
├── Directory.Build.props
├── Instrux.App.slnx
│
├── Instrux.Domain/
│   ├── Interfaces/         # IRepository<T>, IUnitOfWork, IAuditableEntity
│   └── Models/             # 9 entity classes
│
├── Instrux.Services/
│   ├── ApplicationLogic/   # 9 service implementations
│   ├── Common/             # Result<T>
│   ├── DTOs/               # 16 DTO classes
│   ├── Interfaces/         # 9 service interfaces
│   └── Validators/         # 9 FluentValidation validators
│
├── Instrux.Infrastructure/
│   ├── Data/
│   │   ├── Configurations/ # 9 EF Core entity configs
│   │   ├── Interceptors/   # AuditableEntityInterceptor
│   │   └── DbInitializer.cs
│   ├── Migrations/         # EF Core migrations
│   └── Repositories/       # Repository<T>, UnitOfWork
│
├── Instrux.App/
│   ├── Data/
│   │   └── AppDataStore.cs
│   ├── DI/                 # Bootstrapper
│   ├── Services/           # NavigationService
│   ├── Styles/             # Colors.xaml, SidebarStyles.xaml
│   ├── ViewModels/         # Main, ClassDetail, Calendar, Todo, Settings, etc.
│   │   └── Tabs/           # Sub-tab ViewModels
│   └── Views/              # MainWindow, OnboardingWindow, SettingsProfileView
│       └── Tabs/           # Sub-tab UserControls
│
└── Instrux.App.Tests/      # xUnit tests
    ├── Data/
    ├── Services/
    └── SeedDataTests.cs
```

---

## Getting Started

### Prerequisites
- .NET SDK 10.0+
- SQL Server LocalDB (comes with Visual Studio)

### Run the app
```bash
dotnet run --project Instrux.App
```

On first run:
1. EF Core migrations apply automatically
2. Demo seed data is inserted (3 classes, 1 teacher, 3 students, etc.)
3. Onboarding wizard appears to create your teacher profile
4. App opens to the Dashboard

### Run the tests
```bash
dotnet test
```
12 tests using EF Core InMemory database (no SQL Server needed).

### Add a migration
```bash
dotnet ef migrations add MigrationName --project Instrux.Infrastructure --startup-project Instrux.App
```

---

## Domain Entities

| Entity | Description | Key Relationships |
|---|---|---|
| **TeacherProfile** | Teacher identity | Independent |
| **SchoolClass** | A class/section taught | Has many Students, ContentItems, Assessments |
| **Student** | A student enrolled in a class | Belongs to SchoolClass; has AttendanceRecords & Grades |
| **AttendanceRecord** | Daily attendance per student | References Student and Class |
| **ContentItem** | Shared resource (link/file) | Belongs to SchoolClass |
| **Assessment** | An exam or assignment | Belongs to SchoolClass; has many Grades |
| **Grade** | Score for a student on an assessment | References Assessment and Student |
| **CalendarEvent** | Schedule entry (class, meeting) | Optional ClassId |
| **TodoItem** | Personal task item | Independent |

---

## Database Schema

```
SchoolClasses ──┬── Students ──┬── AttendanceRecords
                │              └── Grades
                ├── ContentItems
                └── Assessments ──┐
                                  └── Grades
CalendarEvents    (optional ClassId)
TeacherProfiles   (standalone)
TodoItems         (standalone)
```

Unique constraints on: `StudentIdentifier`, `TeacherProfiles.Email`, `AttendanceRecords(ClassId, StudentId, Date)`, `Grades(AssessmentId, StudentId)`.
