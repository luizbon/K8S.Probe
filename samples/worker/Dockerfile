#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["samples/worker/worker.csproj", "samples/worker/"]
COPY ["src/K8S.Probe/K8S.Probe.csproj", "src/K8S.Probe/"]
RUN dotnet restore "samples/worker/worker.csproj"
COPY . .
WORKDIR "/src/samples/worker"
RUN dotnet build "worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "worker.dll"]