# Logitar.Master.EntityFrameworkCore.PostgreSQL

Provides an implementation of a relational event store to be used with Master project management system, Entity Framework Core and PostgreSQL.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --context MasterContext --project src/Logitar.Master.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Master`

### Remove a migration

To remove the latest unapplied migration, execute the following command.

`dotnet ef migrations remove --context MasterContext --project src/Logitar.Master.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Master`

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

`dotnet ef migrations script <SOURCE_MIGRATION> --context MasterContext --project src/Logitar.Master.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Master`
