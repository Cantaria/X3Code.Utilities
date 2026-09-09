# X3Code.Repository

> ⚠️ **Note**: This library is currently under development. Use with care and own risk!

## Overview
X3Code.Repository is a lightweight library that provides a flexible and easy-to-implement repository pattern for .NET applications. It's designed to work seamlessly with Entity Framework while remaining database provider-independent.

## Features
- 🎯 Generic repository pattern implementation
- 🔌 Database provider agnostic
- 🤝 Entity Framework integration
- 🧱 Easy-to-use base repository classes
- 📦 CRUD operations out of the box
- 🔒 Unit of work: several repositories in one database transaction

## Installation
```shell
dotnet add package X3Code.Repository
```

## Quick Start
1. Define your entity by implementing `Entity<T>`. This class inherits from `IEntity<T>` and will implement your primary key with the type defined.
```csharp
public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
}
```
The type defined in here will be used for the EntityId property. So, if you choose `Entity<Guid>` you will get
```csharp
public Guid EntityId { get; set; }
```
if you choose `Entity<int>` you will get
```csharp
public int EntityId { get; set; }
```
etc.

2. Create an empty interface for your repository and inherit from `IBaseRepository<T>`.
> This will make it much easier to handle the repositories and add them to the DI container.
```csharp
public interface ICustomRepository : IBaseRepository<Custom>
{
}
```

3. Then create the corresponding repository. You need to inherit `BaseRepository<TEntity, TContext>` and your own `ICustomRepository`. With this trick your repository class stays clean and easy. You can add functionality if needed.
```csharp
public class CustomRepository : BaseRepository<Custom, AppContext>, ICustomRepository
{
    public CustomRepository(AppContext context) : base(context)
    {
    }
}
```
> Name your context in the type parameter. There is a one-parameter `BaseRepository<TEntity>` that
> asks for the base `DbContext`, but a DI container can only satisfy that if you register
> `DbContext` yourself — and as soon as you have a second context, that registration silently
> decides which one every repository gets.
> If you need access to the DbContext or the entity, you can use the inherited properties
> ```csharp
> protected DbContext DataBase { get; }
> protected DbSet<TEntity> Entities { get; }
> ```

## Prerequisites
- .NET 10.0 or later
- Entity Framework Core

## Registration
```csharp
services.AddDbContext<AppContext>(options => options.UseSqlServer(connectionString));

// Registers IUnitOfWork<AppContext> and, unless you say otherwise, IUnitOfWork
services.AddX3CodeRepository<AppContext>();

services.AddScoped<ICustomRepository, CustomRepository>();
```

With more than one context, register each and resolve the typed interface:

```csharp
services.AddX3CodeRepository<AppContext>(registerUntyped: false);
services.AddX3CodeRepository<ReportingContext>(registerUntyped: false);
```

```csharp
public ReportService(IUnitOfWork<ReportingContext> unitOfWork) { ... }
```

If you have exactly one context and existing repositories built on the one-parameter
`BaseRepository<TEntity>`, `services.AddDbContextAlias<AppContext>()` registers `DbContext` as an
alias so they keep resolving.

## Usage Examples
```csharp
// Inject and use in your service
public class ProductService
{
    private readonly ICustomRepository _repository;

    public ProductService(ICustomRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product?> GetProductAsync(Guid id)
    {
        return await _repository.GetAsync(x => x.EntityId == id);
    }
}
```

## Unit of work
`IUnitOfWork` groups the writes of several repositories into a single database transaction.
Everything inside the scope is committed together, or not at all.

```csharp
await using var scope = await unitOfWork.BeginAsync();

await personRepository.AddAsync(person);
await orderRepository.AddAsync(order);

await scope.CompleteAsync();
```

Scopes may be nested: only the outermost scope commits. If any scope is left without
`CompleteAsync`, the whole transaction is rolled back — from then on `IsAbandoned` is `true`
on every scope of that unit of work, and `CompleteAsync` throws `UnitOfWorkAbandonedException`.

> The provider has to support transactions. `BeginAsync` refuses the EF Core in-memory
> provider instead of handing out a scope that guarantees nothing.

If the context is configured with `EnableRetryOnFailure`, use `ExecuteAsync` instead. A
retrying execution strategy refuses a transaction the caller opened itself, so the whole
block has to be handed to the strategy:

```csharp
await unitOfWork.ExecuteAsync(async token =>
{
    await personRepository.AddAsync(person, cancellationToken: token);
    await orderRepository.AddAsync(order, cancellationToken: token);
});
```

It commits when the block returns and rolls back when it throws. The block may run more than
once if the strategy retries, so keep side effects outside the database out of it.

## Contributing
Contributions are welcome! As this project is in an experimental state, please feel free to:
- Report bugs
- Suggest features
- Submit pull requests

## License
MIT

## Disclaimer
This library is currently in an experimental state. While it's functional, the API might change in future releases.
