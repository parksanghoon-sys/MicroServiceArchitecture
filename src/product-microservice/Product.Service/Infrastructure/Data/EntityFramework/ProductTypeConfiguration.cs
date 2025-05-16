using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Service.Models;

namespace Product.Service.Infrastructure.Data.EntityFramework;

internal class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasMaxLength(100);

        // 10개의 ProductType 시드 데이터
        builder.HasData(
            new ProductType { Id = 1, Type = "Shoes" },
            new ProductType { Id = 2, Type = "Clothing" },
            new ProductType { Id = 3, Type = "Electronics" },
            new ProductType { Id = 4, Type = "Home & Kitchen" },
            new ProductType { Id = 5, Type = "Books" },
            new ProductType { Id = 6, Type = "Sports & Outdoors" },
            new ProductType { Id = 7, Type = "Beauty & Personal Care" },
            new ProductType { Id = 8, Type = "Toys & Games" },
            new ProductType { Id = 9, Type = "Food & Beverage" },
            new ProductType { Id = 10, Type = "Office Products" }
        );
    }
}
