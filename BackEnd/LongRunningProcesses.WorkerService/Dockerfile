FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BackEnd/LongRunningProcesses.WorkerService/LongRunningProcesses.WorkerService.csproj", "LongRunningProcesses.WorkerService/"]
RUN dotnet restore "LongRunningProcesses.WorkerService/LongRunningProcesses.WorkerService.csproj"

COPY BackEnd/LongRunningProcesses.WorkerService/. LongRunningProcesses.WorkerService/.
COPY BackEnd/Shared/. Shared/.

RUN dotnet build "LongRunningProcesses.WorkerService/LongRunningProcesses.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LongRunningProcesses.WorkerService/LongRunningProcesses.WorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LongRunningProcesses.WorkerService.dll"]