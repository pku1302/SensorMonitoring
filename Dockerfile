# .NET 10 빌드 환경
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# 프로젝트 전체 복사
COPY . .

# API 및 참조 프로젝트 복원
RUN dotnet restore \
    SensorMonitoring.Api/SensorMonitoring.Api.csproj

# API 게시
RUN dotnet publish \
    SensorMonitoring.Api/SensorMonitoring.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# .NET 10 실행 환경
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "SensorMonitoring.Api.dll"]