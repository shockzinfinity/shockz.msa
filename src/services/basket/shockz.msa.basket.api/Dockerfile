#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/basket/shockz.msa.basket.api/shockz.msa.basket.api.csproj", "services/basket/shockz.msa.basket.api/"]
RUN dotnet restore "services/basket/shockz.msa.basket.api/shockz.msa.basket.api.csproj"
COPY . .
WORKDIR "/src/services/basket/shockz.msa.basket.api"
RUN dotnet build "shockz.msa.basket.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shockz.msa.basket.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shockz.msa.basket.api.dll"]
