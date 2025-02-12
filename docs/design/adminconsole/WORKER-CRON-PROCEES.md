# Running Cron process inside of Docker container

___

## Summary

This technical design proposes a solution for running the Health worker and Instance worker on a schedule using the `cron` process. The schedule is configurable using a `crontab` file and allows the workers to log and execute in specified intervals, all from within a container.

## Background

The Admin Console is a web client that provides an interface for users and admins to manage instances. In order to provide a quality  experience in the console app, instance health and status collection is delegated to specialized service workers.

The [Health Check Worker](./HEALTH-CHECK-WORKER.md) requires a periodic update on the health status, to alert the user if an instance is unreachable or in a failure state.

The [Instance Management Worker](./INSTANCE-MANAGEMENT.md) registers and manages instances from a connected ODS/API

For the purpose of the design, we assume that each of these workers can publish a production ready executable that, once running, calls a series endpoints defined in the execution environment.

## Design Overview

This design starts by stating clear assumptions and considerations about the problem statement and environment when determining the solution.

A recommendation follows, outlining suggested steps for running the workers on a schedule in a container environment using crontab. This involves modifying a Docker file to support cron, as well as adding logs and environment variables to inject into the container.

Following the recommendation is a testing strategy for ensuring the crontab task is running and logging.

## Design Details

### Assumptions

In order to scope the investigation, the following assumptions about the user's environment were made:

* The parameters needed by the executable are configured to be read from the execution environment.
* The user has access to the infrastructure needed to support execution of the application using the configured address.
* We assume the application is meant to deploy in a container environment, levering docker-compose for debugging and development.

### Considerations

When determining a solution, the following approaches were considered.

* Create the schedule in the executable itself and copy into image
* Control schedule using external host / orchestrator
* Managing the schedule within the Docker Image itself

Below are quick notes on each approach and the recommended approach.

#### Schedule within the executable

Scheduling within the executable means managing the schedule from within the application itself. While this makes setup locally simple, this approach that has some immediate obvious downsides. The application owner must manage the scheduler and must do so within the execution context of the application. Also, the container would have to run continuously, so the user would need to exercise care managing resources for a long running application process.

#### Schedule controlled by the host system / orchestrator

This approach is more common in container orchestration environments where a control plane can manage execution schedules. The ephemeral nature of containers make them a great environment for tasks that need to short execution times. The downside is that the configuration for this requires a bit more overhead and configuration beyond the host system and executable, which might not be ideal in scenarios where simplicity and speed of deployment are highly valued.

#### Schedule controlled within container

This approach is a hybrid of the two previous approaches, taking advantages and benefits from both. The advantage here is that the Docker image running in a container environment, can be built using a multi-stage docker file, which allows the entry point of the image to be defined at build time. This allows us to take advantage of the `crontab` process, which provides a configuration for 1) defining a process and 2) scheduling execution of that process. In this case, this process is the published console application, and can be configured to run on a schedule using a `crontab` file.

### Recommendation

After careful consideration, the approach of managing Cron within the Docker Image itself appears to be the best fit for our use case. This will provide customizable scheduling via a configurable file while allowing the execution to remain in the container environment, increasing security and testability.

The steps for implementing the recommendation are provided below:

#### Prepare the App Context

The Docker file uses `WORKDIR /source` to reference the root of the application. That is to say `/source/Application/EdFi.AdminConsole.HealthCheck.Service` points to `/[your_local_repo_dir]/Application/EdFi.AdminConsole.HealthCheck.Service`

The Docker file will also need to support exposing the configuration for the following environment variables:

```
{
    "AdminApiSettings": {
      "ApiUrl": "https://localhost:7218/",
      "AdminConsoleInstancesURI": "adminconsole/instances",
      "AdminConsoleHealthCheckURI": "adminconsole/healthcheck",
      "AccessTokenUrl": "https://localhost:7218/connect/token"
  }
}
```

These can be exposed either by using the `AppSettingVariable__NestedKey=VALUE` accessor pattern to expose the values to the container.

The `ClientId` and `ClientSecret` values will need to be retrieved from upon instance creation.

#### Create a Cron file

A cron file will need to be created at the root of your working dir. The file should contain the following:

```c
* * * * * root dotnet /app/EdFi.AdminConsole.HealthCheckService.dll >> /var/log/achealthsvc.log

```

This configuration is set to run every 1-min for testing.

**Ensure that a new line is added to the end of the crontab file or the job will not run!**

#### Update the Docker File

The Docker File is updated to do the following:

* Install `cron`
* Copy over the `crontab` file to `/etc/cron.d/container_cronjob
* Change permissions on the `container_cronjob` file
* Run `crontab` application

An example of the Health Check Worker Dockerfile:

```Dockerfile

# Image based on .NET SDK to compile and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0.401-alpine3.20@sha256:658c93223111638f9bb54746679e554b2cf0453d8fb7b9fed32c3c0726c210fe AS build
  
WORKDIR /source

# Copy source code and compile the application
COPY Application/EdFi.AdminConsole.HealthCheckService/. ./EdFi.AdminConsole.HealthCheckService/

WORKDIR /source/EdFi.AdminConsole.HealthCheckService

# Restore dependencies, Then build and publish release
RUN dotnet restore &&\
  dotnet publish -c Release -o /app

# .NET Runtime image to execute the application
FROM mcr.microsoft.com/dotnet/runtime:8.0.11 AS runtime

# Install Cron.
RUN apt-get update && apt-get install -y cron

# Add cron from file and adjust permissions
COPY crontab /etc/cron.d/container_cronjob
RUN chmod 0644 /etc/cron.d/container_cronjob
RUN crontab /etc/cron.d/container_cronjob  
WORKDIR /app

# Add Published executable
COPY --from=build /app .

# Execute the app via chron
CMD ["cron", "-f"]
```

### Add Docker compose for Development

The `health-check-svs.yml` docker compose file provides a simple way to test building the image and logging the output.

This might be moved or modified in the future to better support a multi-container environment.

```yml
version: "3.9"

services:
  health-check-service:
    build:
      context: ../
      dockerfile: Docker/Dockerfile
    volumes:
      - service-logs:/var/log/achealthsvc.log
    env_file:
      - .env

volumes:
  service-logs:
    name: health-check-service-logs

```

## Testing Strategy

The Docker file takes advantage of crontab file by piping the output of th cron executable to a log file. To ensure the executable is running, this log file is inspected to ensure the output from running the application is piped to the `achealthsvc.log` log file.

To verify the schedule, a user can build the image using the Dockerfile and docker-compose included in the `/Docker/` directory.

`docker compose -f .\Docker\health-check-svc.yml up`

Can check that the job is in the table of jobs to run
`$ crontab -l`

To verify the environment variables can be seen, in the container, once can use the `env` command from a connected shell, and inspect the values returned.

## Additional Notes

This holds any additional questions or comments that do not fit into the design spec above.

### Outstanding Questions

Will The Instance management worker need to be executed on a cron schedule? What would be executed?

What is the difference between Admin API Client and Admin API Caller?

How will the container image call the other services when hosted on localhost in other execution contexts?

What is the expected deployment / execution environment for these services?

* Docker compose okay when developing
* How will the workers be incorporated into the deployment

Is Multi-tenancy something that needs to be accounted for?

Can the crontab file read an environment variable?
