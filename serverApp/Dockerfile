FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OkoBereServer.csproj", "./"]
RUN dotnet restore "OkoBereServer.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "OkoBereServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OkoBereServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OkoBereServer.dll"]
