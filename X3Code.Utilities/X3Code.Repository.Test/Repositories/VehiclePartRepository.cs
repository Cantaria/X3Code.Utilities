using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Entities;

namespace X3Code.Repository.Test.Repositories;

public class VehiclePartRepository : BaseRepository<VehiclePart>, IVehiclePartRepository
{
    public VehiclePartRepository(DbContext context) : base(context)
    {
    }
}