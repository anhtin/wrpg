# wrpg

## Local Development

### Prerequisites

- Docker Compose

### Running API

Ensure that the PostgreSQL database is running: `docker-compose up -d`.

### Secrets

Secrets are required to make authorized requests through Swagger and/or run the smoke tests. Ensure that secrets are
set up correctly on your machine by running the following command.
- **Windows**: `type .\secrets.json | dotnet user-secrets set --project .\src\Api\`
- **Unix**: `cat ./secrets.json | dotnet usersecrets set --project ./src/Api/`

#### NOTE
- The `secret.json` file is ignored by git (`.gitignore`).
- Remove secrets from machine by running `dotnet user-secrets clear`.