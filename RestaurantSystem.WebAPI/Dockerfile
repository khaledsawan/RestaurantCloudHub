FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RestaurantSystem.slnx ./
COPY RestaurantSystem.Application/RestaurantSystem.Application.csproj RestaurantSystem.Application/
COPY RestaurantSystem.Domain/RestaurantSystem.Domain.csproj RestaurantSystem.Domain/
COPY RestaurantSystem.Infrastructure/RestaurantSystem.Infrastructure.csproj RestaurantSystem.Infrastructure/
COPY RestaurantSystem.WebAPI/RestaurantSystem.WebAPI.csproj RestaurantSystem.WebAPI/

RUN dotnet restore RestaurantSystem.WebAPI/RestaurantSystem.WebAPI.csproj

COPY . .
WORKDIR /src/RestaurantSystem.WebAPI
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RestaurantSystem.WebAPI.dll"]
