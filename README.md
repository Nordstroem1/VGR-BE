# VGR-BE

Ett .NET 9-projekt. Den här guiden visar hur du klonar, startar och konfigurerar projektet med user secrets.

## Kom igång
Lägg till User secrets: 
```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Data Source=app.db;Cache=Shared"
```
Eller Kopiera in (SQLite, d.v.s ingen fara att lägga nyckeln här): 
```json
{
  "ConnectionStrings": {
    "Default": "Data Source=app.db;Cache=Shared"
  }
}
```

# Lägg till en nyckel
dotnet user-secrets set "ConnectionStrings:Default" "Data Source=app.db;Cache=Shared"
### Engångskörning (allt-i-ett från tom miljö)
```bash
git clone https://github.com/Nordstroem1/VGR-BE.git \
&& cd VGR-BE \
&& dotnet restore \
&& dotnet watch run --project API/API.csproj
