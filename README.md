# SmartSpace

SmartSpace is a BIDN meeting-room reservation prototype.

## Local foundation

1. Copy `.env.example` to `.env` and replace the local SQL Server password.
2. Start SQL Server with `docker compose up -d sqlserver`.
3. Build with `dotnet build SmartSpace.sln`.
4. The API foundation uses `http://localhost:5080` and the Blazor UI uses
   `http://localhost:3000` as the exact Development CORS origin.

Local development uses `Authentication:Mode=Development` with the development-only
identity handler. Demo identity headers are accepted only when the API runs in the
Development environment; startup fails if this mode is enabled elsewhere.

Shared Azure dev/prod environments should use `Authentication:Mode=EntraId` with
`EntraId:TenantId`, `EntraId:ClientId`, and optionally `EntraId:Audience` supplied
from environment or app settings. Entra setup requires a SmartSpace API app
registration, an exposed API scope such as `access_as_user`, and an app role named
`Administrator` assigned to authorized administrators. Do not commit real tenant,
client, or secret values.

The administrator room page at `http://localhost:3000/beheer/ruimtes` uses readable
location names in a select; location IDs are not entered manually. Administrators
can add and edit locations from the same page. Room and location changes use
version checks, and locations are not hard-deleted while rooms or history refer to
them.

The repository currently contains setup and foundation work only. Feature work is
not complete, and the training prototype is not production acceptance.
