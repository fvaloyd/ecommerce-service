# eCommerce service

## Environment variables you need to run this app

```json
{
  "JWT": {
    "ValidIssuer": "",
    "ValidAudience": "",
    "Secret": ""
  },
  "Stripe": {
    "ApiKey": "",
    "PublishableKey": ""
  },
  "Cloudinary": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  },
  "Smtp": {
    "ApiKey": "",
    "Server": "",
    "Port": "",
    "SenderName": "",
    "SenderEmail": "",
    "Password": ""
  }
}
```

You can put it in appsettings.json, dotnet usersecrets, or whatever thing that allows you to admin your environments variables

## Database migrations

If you want to apply migrations you need to provide these variables to the command

* `--project src/Ecommerce.Infrastructure`
* `--startup-project src/Ecommerce.Api`
* `--output-dir Persistence/Migrations`

The command should look like this:

`dotnet migrations add "yourmigrations" --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api --output-dir Persistence/Migrations`

## Hangfire

The application needs two database for hangfire.

```sh
docker exect -it <containerId> /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <yourPassword>
```

```sql
CREATE DATABASE [HangfireDb]
GO

CREATE DATABASE [HangfireTestDb]
GO
```

## Overview

### Core

All domain models are here, this layer cannot depend on any other layer.

### Application

In this layer you find the domain services and some abstractions of the app

### Infrastructure

This layer manages all external dependencies such as stripe, Cloudinary, and so on and also only depends on the core layer

### Contracts

This layer is a shared classlib to be able to communicate with the api from other app, for example a web-client or mobil-client can communicate with the api using this classlib with refit.

### Api

this layer is a web API and depends on core and infrastructure layers

## License

This project is licensed with the [MIT license](https://github.com/Antsy15400/ecommerce-service/blob/main/LICENSE).
