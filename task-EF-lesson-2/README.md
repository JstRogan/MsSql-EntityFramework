# task-EF lesson 2

## Run

1. Open folder `AutoSalonEfLesson2`
2. Update connection string in `appsettings.json`
3. Create migrations and update database

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Run

```bash
dotnet run
```

## What is implemented

- Entities: Car, Dealer, Customer, Order, CarOrder
- Data annotations validation: Required, StringLength, Range
- Unique index: Make+Model via [Index]
- One-to-many: Dealer -> Cars
- Many-to-many: Customer <-> Car through CarOrder
- Eager loading: Include dealers + cars
- Explicit loading: Entry().Reference().Load()
- Lazy loading: UseLazyLoadingProxies + virtual navigation properties
- FromSqlRaw query
- CRUD for Car
- Soft delete via IsDeleted and filtering in queries
- Transaction example with BeginTransaction
- SQL logging via LogTo

## Migration add/rollback (Color)

1. Add property `public string? Color { get; set; }` to `Car`
2. Create migration

```bash
dotnet ef migrations add AddCarColor
dotnet ef database update
```

3. Rollback

```bash
dotnet ef database update InitialCreate
dotnet ef migrations remove
```
