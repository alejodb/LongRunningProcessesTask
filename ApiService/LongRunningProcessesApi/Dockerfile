FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ApiService/LongRunningProcessesApi/LongRunningProcessesApi.csproj", "ApiService/LongRunningProcessesApi/"]
RUN dotnet restore "ApiService/LongRunningProcessesApi/LongRunningProcessesApi.csproj"

COPY ApiService/. ApiService/.
COPY Shared/. Shared/.

RUN dotnet build "ApiService/LongRunningProcessesApi/LongRunningProcessesApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiService/LongRunningProcessesApi/LongRunningProcessesApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LongRunningProcessesApi.dll"]