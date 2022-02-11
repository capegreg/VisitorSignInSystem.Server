TODO: move this file out of project.

## do this first time (example)
# 
# PM> dotnet user-secrets init
# Set UserSecretsId to '77e9683f-d9a0-443a-b57b-00bb003742c1' for MSBuild project 'C:\source\VsisServerAPI\VsisServerAPI.csproj'.

## do this to update (example)
# 
# dotnet user-secrets set ConnectionStrings:VsisdataAlias "server=Poseidon;database=vsisdata;user=root;password=CaptainJeanLucPicard2305;treattinyasboolean=true"
# 

dotnet user-secrets set ConnectionStrings:VsisdataAlias "server=127.0.0.1;database=vsisdata;user=root;password=CaptainJeanLucPicard2305;treattinyasboolean=true"
PM> dotnet user-secrets set ConnectionStrings:VsisdataAlias "server=127.0.0.1;database=vsisdata;user=root;password=CaptainJeanLucPicard2305;treattinyasboolean=true"
Successfully saved ConnectionStrings:VsisdataAlias = server=127.0.0.1;database=vsisdata;user=root;password=CaptainJeanLucPicard2305;treattinyasboolean=true to the secret store.

PM> dotnet ef dbcontext scaffold Name=ConnectionStrings:VsisdataAlias "Pomelo.EntityFrameworkCore.MySql"
Build started...
Build succeeded.
Using ServerVersion '5.7.22-mysql'.


The Entity Framework tools version '5.0.4' is older than that of the runtime '5.0.5'. Update the tools for the latest features and bug fixes.
Using ServerVersion '5.7.22-mysql'.
PM> dotnet tool update --global dotnet-ef
Tool 'dotnet-ef' was successfully updated from version '5.0.4' to version '5.0.5'.

remove this from vsisdataContext.js

        public vsisdataContext()
        {
        }

was fixed in 5.0.0.2-beta
change this

    modelBuilder.HasCharSet(new object[] { "utf8mb4" })
        .UseCollation(new object[] { "utf8mb4_general_ci" });

to

    modelBuilder.HasCharSet("utf8mb4")
        .UseCollation("utf8mb4_general_ci");




