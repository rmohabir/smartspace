# SmartSpace

SmartSpace is a BIDN meeting-room reservation prototype.

## Local foundation

1. Copy `.env.example` to `.env` and replace the local SQL Server password.
2. Start SQL Server with `docker compose up -d sqlserver`.
3. Build with `dotnet build SmartSpace.sln`.
4. The API foundation uses `http://localhost:5080` and the Blazor UI uses
   `http://localhost:3000` as the exact Development CORS origin.

The repository currently contains setup and foundation work only. Feature work is
not complete, and the training prototype is not production acceptance.
