using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.Service.Infrastructure.Data.EntityFramework;

internal class ProductConfiguration : IEntityTypeConfiguration<Models.Product>
{
    public void Configure(EntityTypeBuilder<Models.Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.HasOne(p => p.ProductType)
            .WithMany()
            .HasForeignKey(p => p.ProductTypeId);

        // 100개 제품 시드 데이터 (각 타입별 10개씩)
        builder.HasData(
            // 타입 1: Shoes (Id 1-10)
            new Models.Product { Id = 1, Name = "Running Shoes", Description = "Lightweight running shoes with cushioned sole", Price = 89.99m, ProductTypeId = 1 },
            new Models.Product { Id = 2, Name = "Hiking Boots", Description = "Waterproof hiking boots for rough terrain", Price = 129.99m, ProductTypeId = 1 },
            new Models.Product { Id = 3, Name = "Casual Sneakers", Description = "Comfortable everyday sneakers", Price = 69.99m, ProductTypeId = 1 },
            new Models.Product { Id = 4, Name = "Basketball Shoes", Description = "High-top basketball shoes with ankle support", Price = 119.99m, ProductTypeId = 1 },
            new Models.Product { Id = 5, Name = "Formal Dress Shoes", Description = "Elegant leather dress shoes for formal occasions", Price = 149.99m, ProductTypeId = 1 },
            new Models.Product { Id = 6, Name = "Sandals", Description = "Comfortable summer sandals", Price = 39.99m, ProductTypeId = 1 },
            new Models.Product { Id = 7, Name = "Soccer Cleats", Description = "Professional soccer cleats for optimal performance", Price = 109.99m, ProductTypeId = 1 },
            new Models.Product { Id = 8, Name = "Winter Boots", Description = "Insulated winter boots for cold weather", Price = 89.99m, ProductTypeId = 1 },
            new Models.Product { Id = 9, Name = "Slip-on Loafers", Description = "Easy slip-on loafers for casual wear", Price = 59.99m, ProductTypeId = 1 },
            new Models.Product { Id = 10, Name = "Tennis Shoes", Description = "Specialized shoes for tennis courts", Price = 74.99m, ProductTypeId = 1 },

            // 타입 2: Clothing (Id 11-20)
            new Models.Product { Id = 11, Name = "T-Shirt", Description = "Cotton crew neck t-shirt", Price = 19.99m, ProductTypeId = 2 },
            new Models.Product { Id = 12, Name = "Jeans", Description = "Classic denim jeans with straight fit", Price = 49.99m, ProductTypeId = 2 },
            new Models.Product { Id = 13, Name = "Hoodie", Description = "Warm hoodie with front pocket", Price = 39.99m, ProductTypeId = 2 },
            new Models.Product { Id = 14, Name = "Winter Jacket", Description = "Insulated winter jacket for extreme cold", Price = 129.99m, ProductTypeId = 2 },
            new Models.Product { Id = 15, Name = "Formal Suit", Description = "Two-piece formal suit for business occasions", Price = 199.99m, ProductTypeId = 2 },
            new Models.Product { Id = 16, Name = "Dress Shirt", Description = "Button-up dress shirt for formal wear", Price = 45.99m, ProductTypeId = 2 },
            new Models.Product { Id = 17, Name = "Shorts", Description = "Casual summer shorts", Price = 29.99m, ProductTypeId = 2 },
            new Models.Product { Id = 18, Name = "Sweater", Description = "Warm knit sweater for winter", Price = 54.99m, ProductTypeId = 2 },
            new Models.Product { Id = 19, Name = "Swimsuit", Description = "Quick-dry material swimwear", Price = 34.99m, ProductTypeId = 2 },
            new Models.Product { Id = 20, Name = "Pajamas", Description = "Comfortable sleepwear set", Price = 24.99m, ProductTypeId = 2 },

            // 타입 3: Electronics (Id 21-30)
            new Models.Product { Id = 21, Name = "Smartphone", Description = "Latest model smartphone with high-resolution camera", Price = 899.99m, ProductTypeId = 3 },
            new Models.Product { Id = 22, Name = "Laptop", Description = "Lightweight laptop with high-performance processor", Price = 1299.99m, ProductTypeId = 3 },
            new Models.Product { Id = 23, Name = "Tablet", Description = "10-inch tablet with retina display", Price = 499.99m, ProductTypeId = 3 },
            new Models.Product { Id = 24, Name = "Wireless Earbuds", Description = "Noise-cancelling wireless earbuds", Price = 149.99m, ProductTypeId = 3 },
            new Models.Product { Id = 25, Name = "Smart Watch", Description = "Fitness tracking smartwatch with heart rate monitor", Price = 249.99m, ProductTypeId = 3 },
            new Models.Product { Id = 26, Name = "Gaming Console", Description = "Next-gen gaming console with 1TB storage", Price = 499.99m, ProductTypeId = 3 },
            new Models.Product { Id = 27, Name = "Digital Camera", Description = "Professional DSLR camera with 24MP sensor", Price = 799.99m, ProductTypeId = 3 },
            new Models.Product { Id = 28, Name = "Bluetooth Speaker", Description = "Portable waterproof bluetooth speaker", Price = 79.99m, ProductTypeId = 3 },
            new Models.Product { Id = 29, Name = "E-reader", Description = "E-ink display e-reader with backlight", Price = 129.99m, ProductTypeId = 3 },
            new Models.Product { Id = 30, Name = "External Hard Drive", Description = "2TB portable external hard drive", Price = 89.99m, ProductTypeId = 3 },

            // 타입 4: Home & Kitchen (Id 31-40)
            new Models.Product { Id = 31, Name = "Coffee Maker", Description = "Programmable coffee maker with thermal carafe", Price = 79.99m, ProductTypeId = 4 },
            new Models.Product { Id = 32, Name = "Blender", Description = "High-speed blender for smoothies and soups", Price = 69.99m, ProductTypeId = 4 },
            new Models.Product { Id = 33, Name = "Toaster", Description = "4-slice toaster with multiple settings", Price = 49.99m, ProductTypeId = 4 },
            new Models.Product { Id = 34, Name = "Cookware Set", Description = "10-piece non-stick cookware set", Price = 149.99m, ProductTypeId = 4 },
            new Models.Product { Id = 35, Name = "Knife Set", Description = "Professional 8-piece knife set with block", Price = 99.99m, ProductTypeId = 4 },
            new Models.Product { Id = 36, Name = "Vacuum Cleaner", Description = "Cordless stick vacuum with HEPA filter", Price = 199.99m, ProductTypeId = 4 },
            new Models.Product { Id = 37, Name = "Dining Table", Description = "6-person wooden dining table", Price = 299.99m, ProductTypeId = 4 },
            new Models.Product { Id = 38, Name = "Sofa", Description = "3-seater fabric sofa with chaise", Price = 599.99m, ProductTypeId = 4 },
            new Models.Product { Id = 39, Name = "Bed Frame", Description = "Queen size wooden bed frame", Price = 249.99m, ProductTypeId = 4 },
            new Models.Product { Id = 40, Name = "Desk Lamp", Description = "Adjustable LED desk lamp with USB port", Price = 39.99m, ProductTypeId = 4 },

            // 타입 5: Books (Id 41-50)
            new Models.Product { Id = 41, Name = "Fiction Bestseller", Description = "Latest fiction bestseller novel", Price = 24.99m, ProductTypeId = 5 },
            new Models.Product { Id = 42, Name = "Cookbook", Description = "International cuisine cookbook with 500 recipes", Price = 29.99m, ProductTypeId = 5 },
            new Models.Product { Id = 43, Name = "Self-Help Book", Description = "Popular self-improvement book", Price = 19.99m, ProductTypeId = 5 },
            new Models.Product { Id = 44, Name = "History Book", Description = "Comprehensive world history book", Price = 34.99m, ProductTypeId = 5 },
            new Models.Product { Id = 45, Name = "Science Fiction Novel", Description = "Award-winning sci-fi novel", Price = 22.99m, ProductTypeId = 5 },
            new Models.Product { Id = 46, Name = "Biography", Description = "Biography of influential historical figure", Price = 27.99m, ProductTypeId = 5 },
            new Models.Product { Id = 47, Name = "Children's Book", Description = "Illustrated children's story book", Price = 14.99m, ProductTypeId = 5 },
            new Models.Product { Id = 48, Name = "Travel Guide", Description = "Comprehensive travel guide with maps", Price = 21.99m, ProductTypeId = 5 },
            new Models.Product { Id = 49, Name = "Programming Book", Description = "Beginner's guide to programming", Price = 39.99m, ProductTypeId = 5 },
            new Models.Product { Id = 50, Name = "Art Book", Description = "Coffee table art book with high-quality prints", Price = 49.99m, ProductTypeId = 5 },

            // 타입 6: Sports & Outdoors (Id 51-60)
            new Models.Product { Id = 51, Name = "Yoga Mat", Description = "Non-slip exercise yoga mat", Price = 29.99m, ProductTypeId = 6 },
            new Models.Product { Id = 52, Name = "Tennis Racket", Description = "Professional tennis racket with case", Price = 89.99m, ProductTypeId = 6 },
            new Models.Product { Id = 53, Name = "Basketball", Description = "Official size indoor/outdoor basketball", Price = 24.99m, ProductTypeId = 6 },
            new Models.Product { Id = 54, Name = "Camping Tent", Description = "4-person waterproof camping tent", Price = 129.99m, ProductTypeId = 6 },
            new Models.Product { Id = 55, Name = "Hiking Backpack", Description = "50L hiking backpack with hydration system", Price = 79.99m, ProductTypeId = 6 },
            new Models.Product { Id = 56, Name = "Bicycle", Description = "Mountain bike with 21 speeds", Price = 349.99m, ProductTypeId = 6 },
            new Models.Product { Id = 57, Name = "Golf Clubs Set", Description = "Complete set of golf clubs for beginners", Price = 299.99m, ProductTypeId = 6 },
            new Models.Product { Id = 58, Name = "Fishing Rod", Description = "Telescopic fishing rod with reel", Price = 59.99m, ProductTypeId = 6 },
            new Models.Product { Id = 59, Name = "Dumbbells", Description = "Pair of 5kg adjustable dumbbells", Price = 49.99m, ProductTypeId = 6 },
            new Models.Product { Id = 60, Name = "Ski Goggles", Description = "Anti-fog ski goggles with UV protection", Price = 39.99m, ProductTypeId = 6 },

            // 타입 7: Beauty & Personal Care (Id 61-70)
            new Models.Product { Id = 61, Name = "Facial Cleanser", Description = "Gentle facial cleanser for all skin types", Price = 14.99m, ProductTypeId = 7 },
            new Models.Product { Id = 62, Name = "Shampoo", Description = "Moisturizing shampoo for dry hair", Price = 12.99m, ProductTypeId = 7 },
            new Models.Product { Id = 63, Name = "Perfume", Description = "Luxury perfume with floral notes", Price = 79.99m, ProductTypeId = 7 },
            new Models.Product { Id = 64, Name = "Electric Razor", Description = "Rechargeable electric razor for men", Price = 89.99m, ProductTypeId = 7 },
            new Models.Product { Id = 65, Name = "Hair Dryer", Description = "Professional hair dryer with diffuser", Price = 59.99m, ProductTypeId = 7 },
            new Models.Product { Id = 66, Name = "Makeup Kit", Description = "Complete makeup kit with 30 colors", Price = 49.99m, ProductTypeId = 7 },
            new Models.Product { Id = 67, Name = "Moisturizer", Description = "Daily face moisturizer with SPF 30", Price = 19.99m, ProductTypeId = 7 },
            new Models.Product { Id = 68, Name = "Nail Polish Set", Description = "10-piece nail polish set with various colors", Price = 29.99m, ProductTypeId = 7 },
            new Models.Product { Id = 69, Name = "Electric Toothbrush", Description = "Sonic electric toothbrush with timer", Price = 69.99m, ProductTypeId = 7 },
            new Models.Product { Id = 70, Name = "Face Mask", Description = "Hydrating sheet face masks, pack of 10", Price = 24.99m, ProductTypeId = 7 },

            // 타입 8: Toys & Games (Id 71-80)
            new Models.Product { Id = 71, Name = "Building Blocks", Description = "Creative building blocks set, 500 pieces", Price = 39.99m, ProductTypeId = 8 },
            new Models.Product { Id = 72, Name = "Board Game", Description = "Family strategy board game", Price = 29.99m, ProductTypeId = 8 },
            new Models.Product { Id = 73, Name = "Remote Control Car", Description = "High-speed remote control race car", Price = 49.99m, ProductTypeId = 8 },
            new Models.Product { Id = 74, Name = "Stuffed Animal", Description = "Soft plush teddy bear, 18 inches", Price = 19.99m, ProductTypeId = 8 },
            new Models.Product { Id = 75, Name = "Action Figure", Description = "Collectible superhero action figure", Price = 24.99m, ProductTypeId = 8 },
            new Models.Product { Id = 76, Name = "Puzzle", Description = "1000-piece landscape jigsaw puzzle", Price = 17.99m, ProductTypeId = 8 },
            new Models.Product { Id = 77, Name = "Card Game", Description = "Family card game for 2-6 players", Price = 12.99m, ProductTypeId = 8 },
            new Models.Product { Id = 78, Name = "Educational Toy", Description = "STEM learning toy for ages 8-12", Price = 34.99m, ProductTypeId = 8 },
            new Models.Product { Id = 79, Name = "Drone", Description = "Beginner drone with HD camera", Price = 89.99m, ProductTypeId = 8 },
            new Models.Product { Id = 80, Name = "Doll House", Description = "Wooden doll house with furniture", Price = 69.99m, ProductTypeId = 8 },

            // 타입 9: Food & Beverage (Id 81-90)
            new Models.Product { Id = 81, Name = "Gourmet Coffee", Description = "Premium single-origin coffee beans, 1lb", Price = 18.99m, ProductTypeId = 9 },
            new Models.Product { Id = 82, Name = "Chocolate Box", Description = "Assorted gourmet chocolates, 24 pieces", Price = 29.99m, ProductTypeId = 9 },
            new Models.Product { Id = 83, Name = "Organic Tea", Description = "Organic herbal tea assortment, 40 bags", Price = 14.99m, ProductTypeId = 9 },
            new Models.Product { Id = 84, Name = "Pasta Set", Description = "Artisanal Italian pasta varieties, 5 pack", Price = 22.99m, ProductTypeId = 9 },
            new Models.Product { Id = 85, Name = "Olive Oil", Description = "Extra virgin olive oil, 500ml", Price = 19.99m, ProductTypeId = 9 },
            new Models.Product { Id = 86, Name = "Spice Collection", Description = "Gourmet spice collection, 12 jars", Price = 39.99m, ProductTypeId = 9 },
            new Models.Product { Id = 87, Name = "Wine Bottle", Description = "Premium red wine, vintage 2018", Price = 49.99m, ProductTypeId = 9 },
            new Models.Product { Id = 88, Name = "Jam Set", Description = "Assorted fruit jam set, 4 jars", Price = 24.99m, ProductTypeId = 9 },
            new Models.Product { Id = 89, Name = "Cheese Selection", Description = "Gourmet cheese assortment, 5 types", Price = 34.99m, ProductTypeId = 9 },
            new Models.Product { Id = 90, Name = "Protein Bars", Description = "High-protein snack bars, box of 12", Price = 19.99m, ProductTypeId = 9 },

            // 타입 10: Office Products (Id 91-100)
            new Models.Product { Id = 91, Name = "Notebook", Description = "Premium ruled notebook, 200 pages", Price = 12.99m, ProductTypeId = 10 },
            new Models.Product { Id = 92, Name = "Pen Set", Description = "Luxury ballpoint pen set, 5 pens", Price = 29.99m, ProductTypeId = 10 },
            new Models.Product { Id = 93, Name = "Office Chair", Description = "Ergonomic office chair with lumbar support", Price = 199.99m, ProductTypeId = 10 },
            new Models.Product { Id = 94, Name = "Desk Organizer", Description = "Multi-compartment desk organizer", Price = 24.99m, ProductTypeId = 10 },
            new Models.Product { Id = 95, Name = "Printer", Description = "Wireless all-in-one color printer", Price = 159.99m, ProductTypeId = 10 },
            new Models.Product { Id = 96, Name = "Paper Shredder", Description = "Cross-cut paper shredder for home office", Price = 49.99m, ProductTypeId = 10 },
            new Models.Product { Id = 97, Name = "Filing Cabinet", Description = "2-drawer metal filing cabinet", Price = 79.99m, ProductTypeId = 10 },
            new Models.Product { Id = 98, Name = "Whiteboard", Description = "Wall-mounted magnetic whiteboard, 36x24\"", Price = 39.99m, ProductTypeId = 10 },
            new Models.Product { Id = 99, Name = "Calculator", Description = "Scientific calculator with 417 functions", Price = 19.99m, ProductTypeId = 10 },
            new Models.Product { Id = 100, Name = "Stapler", Description = "Heavy-duty stapler with 5000 staples", Price = 14.99m, ProductTypeId = 10 }
        );
    }
}
