# VGR-BE

## 🚀 Snabbstart (absolut enklast)
Har du redan klonat repot och står i rotmappen?
```bash
dotnet watch run --project API/API.csproj
```
Det startar API:t med hot reload. Kolla konsolutskriften för URL (t.ex. http://localhost:5070) och besök ev. /swagger.

## 🧪 Engångskörning (allt-i-ett från tom miljö)
```bash
git clone https://github.com/Nordstroem1/VGR-BE.git \
&& cd VGR-BE \
&& dotnet restore \
&& dotnet watch run --project API/API.csproj
```

---

## Innehåll / Arkitektur
- API – Exponerar HTTP-endpoints
- Application – Affärslogik / use cases
- Domain – Domänmodeller och kärnregler
- Infrastructure – Persistens / externa integrationer
- Test – (för testprojekt)
- `VGR-BE.sln` – Lösningsfil

SQLite (`API/app.db`) används för utveckling (kan tas bort för nystart).

Struktur:
```
VGR-BE.sln
API/
  Program.cs
  appsettings.json
  appsettings.Development.json
  app.db
Application/
Domain/
Infrastructure/
Test/
```

---

## Förutsättningar
Installera innan du börjar:
- .NET SDK (senaste LTS, t.ex. .NET 8) – verifiera: `dotnet --version`
- Git

Valfritt (Entity Framework Core migrations, om det används):
```bash
dotnet tool install --global dotnet-ef
```

---

## Detaljerad start (alternativ)
```bash
git clone https://github.com/Nordstroem1/VGR-BE.git
cd VGR-BE
dotnet restore
dotnet run --project API/API.csproj
# eller med hot reload:
# dotnet watch run --project API/API.csproj
```

Starta om med tom databas:
```bash
rm API/app.db        # macOS/Linux
del API\app.db       # Windows
```

---

## Konfiguration
Justera:
- `API/appsettings.json`
- `API/appsettings.Development.json`

User secrets (för hemliga nycklar):
```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "KeyName" "Value"
```

---

## Vanliga kommandon
| Syfte | Kommando |
|-------|----------|
| Restore paket | `dotnet restore` |
| Bygga | `dotnet build` |
| Köra API | `dotnet run --project API/API.csproj` |
| Hot reload | `dotnet watch run --project API/API.csproj` |
| Publicera | `dotnet publish API/API.csproj -c Release -o publish` |

---

## Databas & Migrationer (exempel, om EF Core finns)
```bash
dotnet ef migrations add Init
dotnet ef database update
```
Installera verktyg om saknas:
```bash
dotnet tool install --global dotnet-ef
```

---

## Felsökning
| Problem | Åtgärd |
|---------|--------|
| Saknade paket | `dotnet restore` |
| Fel .NET-version | Installera rätt LTS / uppdatera PATH |
| Databasfel | Ta bort `API/app.db` och kör igen |
| Port upptagen | `ASPNETCORE_URLS=http://localhost:5099 dotnet run --project API/API.csproj` |
| Ingen hot reload | Använd `dotnet watch run` |

---

## Testning
När testprojekt finns i `Test/`:
```bash
dotnet test
```

---

## Distribution (översikt)
```bash
dotnet publish API/API.csproj -c Release -o publish
cd publish
DOTNET_ENVIRONMENT=Production dotnet API.dll
```
Sätt nödvändiga miljövariabler innan start.

---

## Kontribution
1. Skapa branch: `git checkout -b feature/min-funktion`
2. Implementera + (ev.) tester
3. `dotnet build && dotnet test`
4. Öppna Pull Request

---

## Förslag på vidare förbättringar
- CI/CD (GitHub Actions)
- Arkitekturdiagram
- Domändokumentation
- Testprojekt + coverage
- Swagger-konfiguration dokumenterad
- Kodstil / Analyzers (t.ex. StyleCop, Roslyn analyzers)

---

Lycka till! Säg till om du vill ha:
- Minimal README-version
- Engelskspråkig variant
- Sektion för endpoints / exempel-requests
