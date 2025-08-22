FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["WorkerService/LongRunningProcessesWorker/LongRunningProcessesWorker.csproj", "WorkerService/LongRunningProcessesWorker/"]
RUN dotnet restore "WorkerService/LongRunningProcessesWorker/LongRunningProcessesWorker.csproj"

COPY WorkerService/. WorkerService/.
COPY Shared/. Shared/.

RUN dotnet build "WorkerService/LongRunningProcessesWorker/LongRunningProcessesWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerService/LongRunningProcessesWorker/LongRunningProcessesWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LongRunningProcessesWorker.dll"]