# task-EF lesson 1

## Run

1. Open folder `AutoSalonEfLesson1`
2. Update connection string in `appsettings.json`
3. Create database via migrations

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Alternatively, you can use the provided `database.sql` script to create the database.

4. Run

```bash
dotnet run
```

## SQL script

`database.sql` is provided as an export-style script of the expected schema + seed.

## Required queries

The app prints:
- cars bought by customer id 1
- sales in a period
- sales count per manager

## Notes

Seed data is configured via `HasData()` in `DealershipContext`.
