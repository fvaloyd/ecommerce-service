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

## Overview

### Core

All domain logic is here, this layer cant depend on any other layer.

### Infrastructure

This layer manages all external dependencies such as stripe, Cloudinary, and so on and also only depends on the core layer

### Api

this layer is a web API and depends on core and infrastructure layers

## License

This project is licensed with the [MIT license](https://github.com/Antsy15400/ecommerce-service/blob/main/LICENSE).