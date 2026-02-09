# WorkPlanner - Aplikacja do śledzenia czasu pracy

Aplikacja do śledzenia godzin, zadań oraz tworzenia podsumowań pracy i godzin.

## Technologie
- **Backend**: ASP.NET Core Web API (.NET 10) + SQLite + EF Core
- **Frontend**: Blazor WebAssembly (.NET 10)

## Struktura projektu
```
WorkPlanner/
├── WorkPlanner.Api/           # ASP.NET Core Web API
│   ├── Controllers/           # Kontrolery API
│   ├── Data/                  # DbContext
│   ├── Migrations/            # Migracje EF Core
│   ├── Models/                # Encje
│   └── workplanner.db         # Baza SQLite
├── WorkPlanner.Client/        # Blazor WebAssembly
│   ├── Models/                # Modele klienta
│   ├── Services/              # Serwisy HTTP
│   └── Pages/                 # Komponenty Blazor
└── WorkPlanner.sln            # Solution
```

## Uruchomienie

### 1. Uruchomienie API (port 7191 HTTPS)
```bash
cd WorkPlanner.Api
dotnet run
```

### 2. Uruchomienie Klienta (port 7127 HTTPS)
```bash
cd WorkPlanner.Client
dotnet run
```

### 3. Uruchomienie obu jednocześnie
```bash
dotnet run --project WorkPlanner.Api &
dotnet run --project WorkPlanner.Client
```

## API Endpoints

### Zadania
- `GET /api/tasks` - Lista zadań
- `GET /api/tasks/{id}` - Szczegóły zadania
- `POST /api/tasks` - Utwórz zadanie
- `PUT /api/tasks/{id}` - Aktualizuj zadanie
- `DELETE /api/tasks/{id}` - Usuń zadanie

### Wpisy czasu
- `GET /api/workentries` - Lista wpisów
- `GET /api/workentries/by-task/{taskId}` - Wpisy dla zadania
- `POST /api/workentries` - Dodaj wpis czasu
- `PUT /api/workentries/{id}` - Aktualizuj wpis
- `DELETE /api/workentries/{id}` - Usuń wpis

### Podsumowania
- `GET /api/summaries/daily?date=YYYY-MM-DD` - Podsumowanie dnia
- `GET /api/summaries/weekly?weekStart=YYYY-MM-DD` - Podsumowanie tygodnia

## Baza danych
Baza SQLite (`workplanner.db`) jest tworzona automatycznie przy pierwszym uruchomieniu API.

## Migracje
```bash
# Dodaj nową migrację
dotnet ef migrations add NazwaMigracji --project WorkPlanner.Api

# Zaktualizuj bazę
dotnet ef database update --project WorkPlanner.Api
```
