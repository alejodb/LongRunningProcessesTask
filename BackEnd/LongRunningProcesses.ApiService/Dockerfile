FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BackEnd/LongRunningProcesses.ApiService/LongRunningProcesses.ApiService.csproj", "LongRunningProcesses.ApiService/"]
RUN dotnet restore "LongRunningProcesses.ApiService/LongRunningProcesses.ApiService.csproj"

COPY BackEnd/LongRunningProcesses.ApiService/. LongRunningProcesses.ApiService/.
COPY BackEnd/Shared/. Shared/.

RUN dotnet build "LongRunningProcesses.ApiService/LongRunningProcesses.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LongRunningProcesses.ApiService/LongRunningProcesses.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LongRunningProcesses.ApiService.dll"]