TODO: apply style to this document

![Alt Manatee County Property Appraiser - Logo](mcpao-logo.png)

# Entity Framework 6 usage in this project

## This is a code first entity model. 
## Do all database changes in code and update database through ef.

## The database is MySQL  (NOT SQL SERVER!)
### You must use the Pomelo Nuget packages

- Pomelo.EntityFrameworkCore.MySql
- Pomelo.EntityFrameworkCore.MySql.Json.Microsoft
- Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft
- Pomelo.EntityFrameworkCore.MySql.NetTopologySuite

# Create the scaffold if starting from scratch

## Database first scaffold. connection is in secret keys

### see here
### https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql

dotnet ef dbcontext scaffold Name=ConnectionStrings:VsisdataAlias "Pomelo.EntityFrameworkCore.MySql" --output-dir Models --force

## Note:

### if vsisdataContext.cs does not appear in Models, make sure the file was not excluded from project.
### if any errors about 

### After scaffolding, move vsisdataContext.cs to DbContext folder and remove duplicate constructor.

    public vsisdataContext()
    {
    }

# Build solution

# Create a first migration

### Reference

### https://www.entityframeworktutorial.net/efcore/entity-framework-core-migration.aspx


## create a migration file before you change any code
## run again after you change code. check the up\down methods
## create \ edit model
## add \ edit OnModelCreating

## dotnet ef migrations add <the model to add>

## PM> dotnet ef migrations add first-migration
### Build started...
### Build succeeded.
### warn: Microsoft.EntityFrameworkCore.Model.Validation[10400]
###       Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development.
### Done. To undo this action, use 'ef migrations remove'

PM> dotnet ef dbcontext scaffold Name=ConnectionStrings:VsisdataAlias "Pomelo.EntityFrameworkCore.MySql" --output-dir Models --force
Build started...
Build succeeded.
The Entity Framework tools version '5.0.5' is older than that of the runtime '5.0.6'. Update the tools for the latest features and bug fixes.
Using ServerVersion '5.7.28-mysql'.
The column 'categories.active' would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
The column 'locations.open' would normally be mapped to a non-nullable bool property, but it has a default constraint. Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
PM> 

# Remove the last migration if it is not applied to the database
## PM> dotnet ef migrations remove

### delete up\down in first migration if db already exists, and don't do an update until you change something in models
### Update database to run the migration

dotnet ef database update

### Reference

https://docs.microsoft.com/en-us/ef/core/cli/powershell
https://docs.microsoft.com/en-us/ef/core/cli/dotnet

# Misc

### output the migration to file

dotnet ef migrations script -i -o migrations.sql

## shows errors

dotnet build

## Misc troubleshooting

### If complains about EF tools newer than project, run command here

PM> dotnet tool update --global dotnet-ef
Tool 'dotnet-ef' was successfully updated from version '5.0.5' to version '5.0.6'.
PM> dotnet ef dbcontext scaffold Name=ConnectionStrings:VsisdataAlias "Pomelo.EntityFrameworkCore.MySql" --output-dir Models --force
Build started...
Build succeeded.
Using ServerVersion '5.7.28-mysql'.
The column 'categories.active' would normally be mapped to a non-nullable bool property, but it has a default constraint. 
    Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and 
    invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.
The column 'locations.open' would normally be mapped to a non-nullable bool property, but it has a default constraint. 
    Such a column is mapped to a nullable bool property to allow a difference between setting the property to false and 
    invoking the default constraint. See https://go.microsoft.com/fwlink/?linkid=851278 for details.

#
### History
### 2021-08-19 gwb,

PM> dotnet tool update --global dotnet-ef
Tool 'dotnet-ef' was successfully updated from version '5.0.6' to version '5.0.9'.



