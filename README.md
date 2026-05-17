# Instrux — Teacher Classroom Management Tool

Instrux is a WPF desktop application for teachers to manage their classroom data — students, attendance, assessments, grades, content resources, calendar events, and tasks — all backed by a local SQL Server (LocalDB) database.

---

## Architecture

The project follows **Clean Architecture** with 4 layers + tests:

```
┌─────────────────────────────────────────┐
│           Instrux.App (WPF)             │  UI Layer
│   Views, ViewModels, DI, AppDataStore  │
├─────────────────────────────────────────┤
│        Instrux.Services (Logic)         │  Application Layer
│   DTOs, Validators, Services           │
├─────────────────────────────────────────┤
│      Instrux.Infrastructure (Data)      │  Infrastructure Layer
│   DbContext, Repositories, EF Configs  │
├─────────────────────────────────────────┤
│         Instrux.Domain (Core)           │  Domain Layer
│   Models, Interfaces                   │
├─────────────────────────────────────────┤
│        Instrux.App.Tests (xUnit)        │  Tests
│   Service CRUD, Seed data, Data loading │
└─────────────────────────────────────────┘
```

### Layers

**Domain** — Inner core with zero dependencies. Contains domain models (entities), repository interfaces, unit of work interface, and shared abstractions.

**Services** — Application logic layer. Depends only on Domain. Contains:
- 9 service implementations (one per entity) with full CRUD
- 16 DTOs (read + create variants)
- 9 FluentValidation validators
- Manual entity-to-DTO mapping (private static methods per service)
- `Result<T>` pattern for consistent operation outcomes

**Infrastructure** — Data access layer. Depends on Domain and Services. Contains:
- EF Core `AppDbContext` with 9 `DbSet<T>` properties
- Fluent API configurations (max lengths, indexes, cascade rules, unique constraints)
- `Repository<T>` implementing `IRepository<T>`
- `UnitOfWork` implementing `IUnitOfWork`
- `AuditableEntityInterceptor` for automatic audit fields
- EF Core migrations
- `DbInitializer` for seeding mock data on first run

**App** — WPF UI layer. Depends on Services and Infrastructure. Contains:
- `AppDataStore` singleton — all data loaded once into memory at startup, shared across all tabs via `ObservableCollection`
- 9 tab ViewModels using `CommunityToolkit.Mvvm` source generators
- 9 tab UserControls (XAML)
- `MainWindow` with a `TabControl` for all 9 entities
- Serilog logging (console + rolling file)
- Microsoft.Extensions.Hosting integration
- Auto-migration (`Database.Migrate()`) on startup

**Tests** — xUnit test project using EF Core InMemory database:
- 12 tests covering all service CRUD operations, seed data verification, and data loading

---

## Patterns Used

| Pattern | Where |
|---|---|
| **Clean Architecture** | 4-project solution with strict dependency flow |
| **Repository Pattern** | `IRepository<T>` / `Repository<T>` abstracts EF Core |
| **Unit of Work** | `IUnitOfWork` wraps `SaveChangesAsync` |
| **Result Pattern** | `Result<T>` / `Result` classes avoid exceptions for expected failures |
| **MVVM** | WPF pattern with `ObservableObject`, source-generated `[ObservableProperty]` and `[RelayCommand]` |
| **Dependency Injection** | `IHost` / `IServiceCollection` via `Microsoft.Extensions.Hosting` |
| **DTO Pattern** | Separate read DTOs and create DTOs per entity |
| **Manual Mapping** | Private static `MapToDto` / `MapToEntity` / `ApplyDto` methods per service |
| **Singleton Data Store** | `AppDataStore` — in-memory shared `ObservableCollection`s, one DB load at startup |
| **FluentValidation** | 9 validators for create operations |
| **Fluent API** | EF Core entity configurations (separate `IEntityTypeConfiguration<T>` classes) |
| **Interceptor Pattern** | EF Core `SaveChangesInterceptor` for audit fields |
| **Auto Migration** | `context.Database.Migrate()` runs on every startup |
| **Seed Data** | `DbInitializer` seeds mock data on first run |
| **Inline Form Editing** | CRUD forms embedded below data grid (no popup dialogs) |

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
| xUnit | 2.9.3 |
| EF Core InMemory | 10.0.0-preview.7 (tests only) |

---

## Project Structure

```
Instrux.App/
├── .gitignore
├── Directory.Build.props              # Shared MSBuild properties
├── Instrux.App.slnx
│
├── Instrux.Domain/                    # Domain models & interfaces
│   ├── Interfaces/                    #   IRepository<T>, IUnitOfWork, IAuditableEntity
│   └── Models/                        #   9 entity classes
│
├── Instrux.Services/                  # Application logic
│   ├── ApplicationLogic/              #   9 service implementations (manual mapping)
│   ├── Common/                        #   Result<T>
│   ├── DTOs/                          #   16 DTO classes
│   ├── Interfaces/                    #   9 service interfaces
│   └── Validators/                    #   9 FluentValidation validators
│
├── Instrux.Infrastructure/            # Data access
│   ├── Data/
│   │   ├── Configurations/            #   9 EF Core entity configs
│   │   ├── Interceptors/              #   AuditableEntityInterceptor
│   │   └── DbInitializer.cs           #   Seed data
│   ├── Migrations/                    #   EF Core migrations
│   └── Repositories/                  #   Repository<T>, UnitOfWork
│
├── Instrux.App/                       # WPF UI
│   ├── Data/
│   │   └── AppDataStore.cs            #   Shared in-memory data store (singleton)
│   ├── DI/                            #   Bootstrapper (IHost setup)
│   ├── Services/                      #   INavigationService, NavigationService
│   ├── Styles/                        #   Colors.xaml, Typography.xaml
│   ├── ViewModels/
│   │   ├── MainViewModel.cs
│   │   └── Tabs/                      #   9 tab ViewModels
│   └── Views/
│       ├── MainWindow.xaml
│       └── Tabs/                      #   9 tab UserControls
│
└── Instrux.App.Tests/                 # xUnit tests
    ├── Data/                          #   Data loading tests
    ├── Services/                      #   Service CRUD tests
    └── SeedDataTests.cs               #   DbInitializer verification
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
On first run, EF Core migrations apply automatically and mock seed data is inserted. The app opens with pre-populated data across all 9 tabs.

### Run the tests
```bash
dotnet test
```
12 tests using EF Core InMemory database (no SQL Server needed).

### Add a new entity migration
```bash
dotnet ef migrations add MigrationName --project Instrux.Infrastructure --startup-project Instrux.App
```
Migrations apply automatically when the app starts next.

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

Nine tables with cascade deletes on parent-child relationships:

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
