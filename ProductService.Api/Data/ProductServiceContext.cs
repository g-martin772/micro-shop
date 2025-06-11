using Microsoft.EntityFrameworkCore;
using ProductService.Api.Models;

namespace ProductService.Api.Data;

public class ProductServiceContext: DbContext
{
    public ProductServiceContext(DbContextOptions<ProductServiceContext> options) : base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}