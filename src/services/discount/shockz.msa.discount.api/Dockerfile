#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/discount/shockz.msa.discount.api/shockz.msa.discount.api.csproj", "services/discount/shockz.msa.discount.api/"]
RUN dotnet restore "services/discount/shockz.msa.discount.api/shockz.msa.discount.api.csproj"
COPY . .
WORKDIR "/src/services/discount/shockz.msa.discount.api"
RUN dotnet build "shockz.msa.discount.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shockz.msa.discount.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shockz.msa.discount.api.dll"]
