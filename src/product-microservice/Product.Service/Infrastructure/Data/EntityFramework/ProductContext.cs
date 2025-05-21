﻿using Microsoft.EntityFrameworkCore;
using Product.Service.Models;

namespace Product.Service.Infrastructure.Data.EntityFramework;

internal class ProductContext : DbContext, IProductStore
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
    {
    }
    public DbSet<Models.Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured == false)
        {
            // optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=Product;Port=5432;User Id=postgres;Password=gcstest1#;");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductTypeConfiguration());
    }

    public async Task<Models.Product?> GetById(int id)
    {
        return await Products
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateProduct(Models.Product product)
    {
        Products.Add(product);

        await SaveChangesAsync();
    }

    public async Task UpdateProduct(Models.Product product)
    {
        var existingProduct = await FindAsync<Models.Product>(product.Id);

        if (existingProduct is not null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;            

            await SaveChangesAsync(acceptAllChangesOnSuccess: false);
        }
    }

    public async Task<List<ProductType>> GatProducTypeAll()
    {
        return await ProductTypes.ToListAsync();
    }
}
