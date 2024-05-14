using Microsoft.EntityFrameworkCore;
using CarDealerAPI.Models;
namespace CarDealerAPI.Data;
public class CarDealerContext : DbContext
{
    public CarDealerContext(DbContextOptions<CarDealerContext> options) : base(options)
    {
    }

    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Model> Models { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>()
            .HasMany(m => m.Models)
            .WithOne(model => model.Manufacturer)
            .HasForeignKey(model => model.ManufacturerId);

        // Дополнительные настройки миграции и связей могут быть определены здесь.
    }
}
