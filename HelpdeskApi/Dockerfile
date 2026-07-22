FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HelpdeskApi.csproj", "./"]
RUN dotnet restore "HelpdeskApi.csproj"

COPY . .
RUN dotnet publish "HelpdeskApi.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM runtime AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "HelpdeskApi.dll"]


INSERT INTO "Tickets"
(
    "Title",
    "Description",
    "Status",
    "Priority",
    "CreatedAt"
)
VALUES
(
    'VPN funktioniert nicht',
    'Verbindung zum Firmennetzwerk schlägt fehl.',
    'Open',
    'High',
    NOW()
),
(
    'Drucker offline',
    'Der Drucker im zweiten Stock reagiert nicht.',
    'Open',
    'Medium',
    NOW() - INTERVAL '1 day'
),
(
    'Passwort zurücksetzen',
    'Benutzer hat sein Passwort vergessen.',
    'Resolved',
    'Low',
    NOW() - INTERVAL '2 days'
),
(
    'Outlook startet nicht',
    'Outlook bleibt beim Start hängen.',
    'InProgress',
    'High',
    NOW() - INTERVAL '3 hours'
),
(
    'Laptop sehr langsam',
    'System reagiert seit dem letzten Update langsam.',
    'Open',
    'Medium',
    NOW() - INTERVAL '5 hours'
);