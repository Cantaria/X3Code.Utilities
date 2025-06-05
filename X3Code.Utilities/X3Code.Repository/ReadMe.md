# X3Code.Repository

> ⚠️ **Note**: This library is currently in an experimental state.

## Overview
X3Code.Repository is a lightweight library that provides a flexible and easy-to-implement repository pattern for .NET applications. It's designed to work seamlessly with Entity Framework while remaining database provider-independent.

## Features
- 🎯 Generic repository pattern implementation
- 🔌 Database provider agnostic
- 🤝 Entity Framework integration
- 🧱 Easy-to-use base repository classes
- 📦 CRUD operations out of the box
- 🎨 Flexible and extensible design

## Installation
```shell
dotnet add package X3Code.Repository
```

## Quick Start
1. Define your entity by implementing `Entity<T>`. This class inherits from `IEntity<T>` and will implement you primary key with the type defined.
```csharp
csharp public class Product : Entity<T> 
{
    public string Name { get; set; } // Other properties... 
}
```
The type defined in here will be used for the EntityId Property. So, if you choose `Entity<Guid>` you will get
```csharp
public Guid EntityId { get; set; }
```
if you choose `Entity<int>` you will get
```csharp
public int EntityId { get; set; }
```
etc

2. Create an empty interface for your repository and inherit from `IBaseRepository<T>`
> This will make it much easier, to handle the repositories and add them to the DI-container
```csharp
public interface ICustomRepository : IBaseRepository<Custom>
{
    
}
```

3. Then create the corresponding repository. You need to inherit the `BaseRepository<T>` and your self created `ICustomRepository`. With this trick, you repository class stays clean and easy. You can add functionality if needed.
```csharp
public class CustomRepository : BaseRepository<Custom>, ICustomRepository
{
    public VehiclePartRepository(DbContext context) : base(context)
    {
    }
}
```
> If you need access to the DbContext or the Entity, you can use the inherited properties
> ```csharp
> protected DbContext DataBase { get; }    
> protected DbSet<TEntity> Entities { get; }
> ```

4. Add your Repositories to the DI-Container
5. Register your Entities to the Db-Context

## Prerequisites
- .NET 9.0 or later
- Entity Framework Core

## Usage Examples
```csharp
// Register in your DI container
services.AddScoped<IRepository<Product>, ProductRepository>();

// Inject and use in your service
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
```
## Features in Detail
- Generic CRUD operations
- Async support
- Specification pattern support
- Query customization
- Transaction management
- Unit of work pattern integration

## Contributing
Contributions are welcome! As this project is in an experimental state, please feel free to:
- Report bugs
- Suggest features
- Submit pull requests

## License
MIT
## Disclaimer
This library is currently in an experimental state. While it's functional, the API might change in future releases.
This README provides a comprehensive overview of the project while keeping it clean and professional. It includes:
- Clear project description
- Feature highlights
- Installation instructions
- Quick start guide
- Usage examples
- Contributing guidelines
- Important notices about the experimental status

You may want to customize the following sections:
1. Add specific version numbers or compatibility information
2. Include more detailed usage examples
3. Add license information
4. Expand the contributing guidelines
5. Add any specific configuration requirements
6. Include troubleshooting section if needed

Would you like me to modify any section or add more specific information?