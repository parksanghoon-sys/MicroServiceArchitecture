using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework;

internal class ProductContext : DbContext, IProductStore
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
    {
        
    }
    public DbSet<Models.Product> Products { get; set; }
    public DbSet<Models.ProductType> ProductTypes { get; set; }
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

        if(existingProduct is not null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;

            await SaveChangesAsync();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductTypeConfiguration());
    }
}
