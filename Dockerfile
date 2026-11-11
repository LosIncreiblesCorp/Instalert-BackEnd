FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InstAlertPlatform.API/InstAlertPlatform.API.csproj InstAlertPlatform.API/
RUN dotnet restore InstAlertPlatform.API/InstAlertPlatform.API.csproj

COPY InstAlertPlatform.API/ InstAlertPlatform.API/
RUN dotnet publish InstAlertPlatform.API/InstAlertPlatform.API.csproj \
    --configuration Release --no-restore --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=10000
EXPOSE 10000
USER $APP_UID:1000

CMD ["sh", "-c", "exec dotnet InstAlertPlatform.API.dll --urls http://0.0.0.0:${PORT:-10000}"]
