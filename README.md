# dockerfile:
right-click in passin.api and add it with Rider dockerfile template
## changes:
Add the tools to the PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

Install EF Core CLI tools
RUN dotnet tool install --global dotnet-ef --version 8.*
RUN dotnet ef --version

* those are need to execute the dotnet run command inside the container

RUN dotnet ef migrations bundle -v --force --project PassIn.Infrastructure

COPY --from=build /src/efbundle .

* needed to be able to run the migrations in the container, after both services (app and sqlserver) are up

# docker compose file:
right click on the PassIn solution and add docker compose file from Rider template.
### important: deactivate fast run from Rider to execute all steps from docker file, which includes the efbundle.
environment variables and network configuration from https://www.youtube.com/watch?v=hpLvXNASyTI

# Applying migrations
used the bundle idea from https://paraspatidar.medium.com/running-ef-database-migrations-from-devops-perspective-entity-framwork-migration-deployment-3512ba1b18eb

But I runned the ./efbundle on the docker ui, exec tab, app folder with command: ./efbundle
To properly connect the app service to its server I had to stop setting the environment variables:

// Environment.SetEnvironmentVariable("DB_HOST", "localhost"); => this is wrong because the app service need to 
connecto to the database using host = passin.db

localhost is used for you to connect to the containerazed sqlserver

# Seeding the database 
seeding the database on the docker ui, exec tab, app folder, with the command: dotnet PassIn.Api.dll seed
