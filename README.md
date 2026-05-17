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
