#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["apiGateways/shockz.msa.shopping.Aggregator/shockz.msa.shopping.Aggregator.csproj", "apiGateways/shockz.msa.shopping.Aggregator/"]
RUN dotnet restore "apiGateways/shockz.msa.shopping.Aggregator/shockz.msa.shopping.Aggregator.csproj"
COPY . .
WORKDIR "/src/apiGateways/shockz.msa.shopping.Aggregator"
RUN dotnet build "shockz.msa.shopping.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shockz.msa.shopping.Aggregator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shockz.msa.shopping.Aggregator.dll"]
