#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/fileManagement/shockz.msa.fileManagement/shockz.msa.fileManagement.csproj", "services/fileManagement/shockz.msa.fileManagement/"]
COPY ["buildingBlocks/shockz.msa.commonLogging/shockz.msa.commonLogging.csproj", "buildingBlocks/shockz.msa.commonLogging/"]
RUN dotnet restore "services/fileManagement/shockz.msa.fileManagement/shockz.msa.fileManagement.csproj"
COPY . .
WORKDIR "/src/services/fileManagement/shockz.msa.fileManagement"
RUN dotnet build "shockz.msa.fileManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shockz.msa.fileManagement.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shockz.msa.fileManagement.dll"]
