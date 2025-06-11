using Microsoft.EntityFrameworkCore;
using OrderService.Api.Model;

namespace OrderService.Api.Data;

public class OrderServiceContext: DbContext
{
    public OrderServiceContext(DbContextOptions<OrderServiceContext> options) : base(options)
    {
        
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderHasProduct> OrderHasProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<OrderHasProduct>()
            .HasOne(osh => osh.Order)
            .WithMany()
            .HasForeignKey(osh => osh.OrderId);
    }
}