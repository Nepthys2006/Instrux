# Instrux — Teacher Classroom Management Tool

Instrux is a WPF desktop application for teachers to manage their classroom data — students, attendance, assessments, grades, content resources, calendar events, and tasks — all backed by a local SQL Server (LocalDB) database.

---

## Architecture

The project follows **Clean Architecture** with 4 layers:

```
┌─────────────────────────────────────────┐
│           Instrux.App (WPF)             │  UI Layer
│   Views, ViewModels, DI, Services       │
├─────────────────────────────────────────┤
│        Instrux.Services (Logic)         │  Application Layer
│   DTOs, Validators, Mappings, Services  │
├─────────────────────────────────────────┤
│      Instrux.Infrastructure (Data)      │  Infrastructure Layer
│   DbContext, Repositories, EF Configs   │
├─────────────────────────────────────────┤
│         Instrux.Domain (Core)           │  Domain Layer
│   Models, Interfaces, Common            │
└─────────────────────────────────────────┘
```

### Layers

**Domain** — Inner core with zero dependencies. Contains domain models (entities), repository interfaces, unit of work interface, and shared abstractions (`BaseEntity`, `ValueObject`).

**Services** — Application logic layer. Depends only on Domain. Contains:
- 9 service implementations (one per entity) with full CRUD
- 16 DTOs (read + create variants)
- 9 FluentValidation validators
- AutoMapper mapping profile
- `Result<T>` pattern for consistent operation outcomes

**Infrastructure** — Data access layer. Depends on Domain and Services. Contains:
- EF Core `AppDbContext` with 9 `DbSet<T>` properties
- Fluent API configurations (max lengths, indexes, cascade rules, unique constraints)
- `Repository<T>` implementing `IRepository<T>`
- `UnitOfWork` implementing `IUnitOfWork`
- `AuditableEntityInterceptor` for automatic audit fields
- EF Core migrations

**App** — WPF UI layer. Depends on Services and Infrastructure. Contains:
- 9 tab ViewModels using `CommunityToolkit.Mvvm` source generators
- 9 tab UserControls (XAML)
- `MainWindow` with a `TabControl` for all 9 entities
- Serilog logging (console + rolling file)
- Microsoft.Extensions.Hosting integration

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
| **AutoMapper** | Entity-to-DTO mapping in `MappingProfile.cs` |
| **FluentValidation** | 9 validators for create operations |
| **Fluent API** | EF Core entity configurations (separate `IEntityTypeConfiguration<T>` classes) |
| **Interceptor Pattern** | EF Core `SaveChangesInterceptor` for audit fields |
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
| AutoMapper | 12.0.1 |
| FluentValidation | 12.1.1 |
| Serilog | 8.0+ |
| Microsoft.Extensions.Hosting | 10.0.8 |

---

## Project Structure

```
Instrux.App/
├── .gitignore
├── Directory.Build.props              # Shared MSBuild properties
├── Instrux.App.slnx
│
├── Instrux.Domain/                    # Domain models & interfaces
│   ├── Common/                        #   BaseEntity, ValueObject, IDomainEvent
│   ├── Interfaces/                    #   IRepository<T>, IUnitOfWork, IAuditableEntity
│   └── Models/                        #   9 entity classes
│
├── Instrux.Services/                  # Application logic
│   ├── ApplicationLogic/              #   9 service implementations
│   ├── Common/                        #   Result<T>, exceptions
│   ├── DTOs/                          #   16 DTO classes
│   ├── Interfaces/                    #   9 service interfaces
│   ├── Mapping/                       #   AutoMapper profile
│   └── Validators/                    #   9 FluentValidation validators
│
├── Instrux.Infrastructure/            # Data access
│   ├── Data/
│   │   ├── Configurations/            #   9 EF Core entity configs
│   │   └── Interceptors/              #   AuditableEntityInterceptor
│   ├── Migrations/                    #   EF Core migrations
│   └── Repositories/                 #   Repository<T>, UnitOfWork
│
└── Instrux.App/                       # WPF UI
    ├── DI/                            #   Bootstrapper (IHost setup)
    ├── Services/                      #   INavigationService, NavigationService
    ├── Styles/                        #   Colors.xaml, Typography.xaml
    ├── ViewModels/
    │   ├── MainViewModel.cs
    │   └── Tabs/                      #   9 tab ViewModels
    └── Views/
        ├── MainWindow.xaml
        └── Tabs/                      #   9 tab UserControls
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

## Features by Tab

Each tab provides full CRUD with an inline form panel below a read-only DataGrid:

- **Teacher Profiles** — Name, nickname, email management
- **School Classes** — Create classes with name, subject, section, term, color
- **Students** — Enroll students in classes with student IDs and email
- **Attendance** — Daily attendance with status (Present/Absent/Late/Escused); student dropdown filters by selected class
- **Content Items** — Share URLs/resources per class with type tags
- **Assessments** — Define assessments with max scores per class
- **Grades** — Record scores per student per assessment
- **Calendar Events** — Schedule events with date, time range, category
- **Todo Items** — Personal task list with priority and due dates

---

## Getting Started

### Prerequisites

- .NET SDK 10.0 or later
- SQL Server LocalDB (comes with Visual Studio or SQL Server Express)

### Setup & Run

```bash
# Clone the repository
git clone https://github.com/Nepthys2006/Instrux.git
cd Instrux

# Create and apply the database migration
dotnet ef database update -p Instrux.Infrastructure -s Instrux.App

# Launch the application
dotnet run --project Instrux.App
```

The database `InstruxDb` is automatically created in LocalDB with all tables, indexes, and foreign keys.

### Configuration

Connection string is in `Instrux.App/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InstruxDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

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
