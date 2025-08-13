# Long Running Processes Task

## Requirements
- .NET Core 9.0
- Node 22
- Docker Desktop or Podman

## Instructions

1. Compile the project running ``dotnet build``.
2. Install npm packages in ``long-running-processes-client`` folder, running the command ``npm install``.
3. Run the Aspire Host: ``dotnet run --project LongRunningProcesses.AppHost``.
4. After running the host it will open a new browser tab where you can test the application.
5. You can open new tabs or browser for testing multiples process running.