#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/ordering/shockz.msa.ordering.api/shockz.msa.ordering.api.csproj", "services/ordering/shockz.msa.ordering.api/"]
COPY ["services/ordering/shockz.msa.ordering.infrastructure/shockz.msa.ordering.infrastructure.csproj", "services/ordering/shockz.msa.ordering.infrastructure/"]
COPY ["services/ordering/shockz.msa.ordering.application/shockz.msa.ordering.application.csproj", "services/ordering/shockz.msa.ordering.application/"]
COPY ["services/ordering/shockz.msa.ordering.domain/shockz.msa.ordering.domain.csproj", "services/ordering/shockz.msa.ordering.domain/"]
RUN dotnet restore "services/ordering/shockz.msa.ordering.api/shockz.msa.ordering.api.csproj"
COPY . .
WORKDIR "/src/services/ordering/shockz.msa.ordering.api"
RUN dotnet build "shockz.msa.ordering.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shockz.msa.ordering.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shockz.msa.ordering.api.dll"]
