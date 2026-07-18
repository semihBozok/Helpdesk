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