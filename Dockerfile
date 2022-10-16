FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./src/Ecommerce.Api/Ecommerce.Api.csproj" --disable-parallel
RUN dotnet publish "./src/Ecommerce.Api/Ecommerce.Api.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]