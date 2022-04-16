#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["webApps/WebStatus/WebStatus.csproj", "webApps/WebStatus/"]
COPY ["buildingBlocks/shockz.msa.commonLogging/shockz.msa.commonLogging.csproj", "buildingBlocks/shockz.msa.commonLogging/"]
RUN dotnet restore "webApps/WebStatus/WebStatus.csproj"
COPY . .
WORKDIR "/src/webApps/WebStatus"
RUN dotnet build "WebStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebStatus.dll"]
