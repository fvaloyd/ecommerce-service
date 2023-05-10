FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /sln

COPY ./*.sln ./

COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

RUN dotnet restore --disable-parallel

COPY . .

RUN dotnet test ./tests/Architecture.Tests/Architecture.Tests.csproj
RUN dotnet test ./tests/Ecommerce.Core.UnitTests/Ecommerce.Core.UnitTests.csproj
RUN dotnet test ./tests/Ecommerce.Application.UnitTests/Ecommerce.Application.UnitTests.csproj
RUN dotnet test ./tests/Ecommerce.Infrastructure.UnitTests/Ecommerce.Infrastructure.UnitTests.csproj
## RUN dotnet test ./tests/Ecommerce.Api.IntegrationTests/Ecommerce.Api.IntegrationTests.csproj

RUN dotnet publish "./src/Ecommerce.Api/Ecommerce.Api.csproj" -c release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /publish

COPY --from=build /publish .

EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS="http://+:80;https://+:443"

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
