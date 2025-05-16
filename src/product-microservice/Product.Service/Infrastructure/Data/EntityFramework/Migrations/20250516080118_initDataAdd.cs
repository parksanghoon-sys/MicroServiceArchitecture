using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Product.Service.Infrastructure.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class initDataAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "Clothing");

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 3, "Electronics" },
                    { 4, "Home & Kitchen" },
                    { 5, "Books" },
                    { 6, "Sports & Outdoors" },
                    { 7, "Beauty & Personal Care" },
                    { 8, "Toys & Games" },
                    { 9, "Food & Beverage" },
                    { 10, "Office Products" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price", "ProductTypeId" },
                values: new object[,]
                {
                    { 1, "Lightweight running shoes with cushioned sole", "Running Shoes", 89.99m, 1 },
                    { 2, "Waterproof hiking boots for rough terrain", "Hiking Boots", 129.99m, 1 },
                    { 3, "Comfortable everyday sneakers", "Casual Sneakers", 69.99m, 1 },
                    { 4, "High-top basketball shoes with ankle support", "Basketball Shoes", 119.99m, 1 },
                    { 5, "Elegant leather dress shoes for formal occasions", "Formal Dress Shoes", 149.99m, 1 },
                    { 6, "Comfortable summer sandals", "Sandals", 39.99m, 1 },
                    { 7, "Professional soccer cleats for optimal performance", "Soccer Cleats", 109.99m, 1 },
                    { 8, "Insulated winter boots for cold weather", "Winter Boots", 89.99m, 1 },
                    { 9, "Easy slip-on loafers for casual wear", "Slip-on Loafers", 59.99m, 1 },
                    { 10, "Specialized shoes for tennis courts", "Tennis Shoes", 74.99m, 1 },
                    { 11, "Cotton crew neck t-shirt", "T-Shirt", 19.99m, 2 },
                    { 12, "Classic denim jeans with straight fit", "Jeans", 49.99m, 2 },
                    { 13, "Warm hoodie with front pocket", "Hoodie", 39.99m, 2 },
                    { 14, "Insulated winter jacket for extreme cold", "Winter Jacket", 129.99m, 2 },
                    { 15, "Two-piece formal suit for business occasions", "Formal Suit", 199.99m, 2 },
                    { 16, "Button-up dress shirt for formal wear", "Dress Shirt", 45.99m, 2 },
                    { 17, "Casual summer shorts", "Shorts", 29.99m, 2 },
                    { 18, "Warm knit sweater for winter", "Sweater", 54.99m, 2 },
                    { 19, "Quick-dry material swimwear", "Swimsuit", 34.99m, 2 },
                    { 20, "Comfortable sleepwear set", "Pajamas", 24.99m, 2 },
                    { 21, "Latest model smartphone with high-resolution camera", "Smartphone", 899.99m, 3 },
                    { 22, "Lightweight laptop with high-performance processor", "Laptop", 1299.99m, 3 },
                    { 23, "10-inch tablet with retina display", "Tablet", 499.99m, 3 },
                    { 24, "Noise-cancelling wireless earbuds", "Wireless Earbuds", 149.99m, 3 },
                    { 25, "Fitness tracking smartwatch with heart rate monitor", "Smart Watch", 249.99m, 3 },
                    { 26, "Next-gen gaming console with 1TB storage", "Gaming Console", 499.99m, 3 },
                    { 27, "Professional DSLR camera with 24MP sensor", "Digital Camera", 799.99m, 3 },
                    { 28, "Portable waterproof bluetooth speaker", "Bluetooth Speaker", 79.99m, 3 },
                    { 29, "E-ink display e-reader with backlight", "E-reader", 129.99m, 3 },
                    { 30, "2TB portable external hard drive", "External Hard Drive", 89.99m, 3 },
                    { 31, "Programmable coffee maker with thermal carafe", "Coffee Maker", 79.99m, 4 },
                    { 32, "High-speed blender for smoothies and soups", "Blender", 69.99m, 4 },
                    { 33, "4-slice toaster with multiple settings", "Toaster", 49.99m, 4 },
                    { 34, "10-piece non-stick cookware set", "Cookware Set", 149.99m, 4 },
                    { 35, "Professional 8-piece knife set with block", "Knife Set", 99.99m, 4 },
                    { 36, "Cordless stick vacuum with HEPA filter", "Vacuum Cleaner", 199.99m, 4 },
                    { 37, "6-person wooden dining table", "Dining Table", 299.99m, 4 },
                    { 38, "3-seater fabric sofa with chaise", "Sofa", 599.99m, 4 },
                    { 39, "Queen size wooden bed frame", "Bed Frame", 249.99m, 4 },
                    { 40, "Adjustable LED desk lamp with USB port", "Desk Lamp", 39.99m, 4 },
                    { 41, "Latest fiction bestseller novel", "Fiction Bestseller", 24.99m, 5 },
                    { 42, "International cuisine cookbook with 500 recipes", "Cookbook", 29.99m, 5 },
                    { 43, "Popular self-improvement book", "Self-Help Book", 19.99m, 5 },
                    { 44, "Comprehensive world history book", "History Book", 34.99m, 5 },
                    { 45, "Award-winning sci-fi novel", "Science Fiction Novel", 22.99m, 5 },
                    { 46, "Biography of influential historical figure", "Biography", 27.99m, 5 },
                    { 47, "Illustrated children's story book", "Children's Book", 14.99m, 5 },
                    { 48, "Comprehensive travel guide with maps", "Travel Guide", 21.99m, 5 },
                    { 49, "Beginner's guide to programming", "Programming Book", 39.99m, 5 },
                    { 50, "Coffee table art book with high-quality prints", "Art Book", 49.99m, 5 },
                    { 51, "Non-slip exercise yoga mat", "Yoga Mat", 29.99m, 6 },
                    { 52, "Professional tennis racket with case", "Tennis Racket", 89.99m, 6 },
                    { 53, "Official size indoor/outdoor basketball", "Basketball", 24.99m, 6 },
                    { 54, "4-person waterproof camping tent", "Camping Tent", 129.99m, 6 },
                    { 55, "50L hiking backpack with hydration system", "Hiking Backpack", 79.99m, 6 },
                    { 56, "Mountain bike with 21 speeds", "Bicycle", 349.99m, 6 },
                    { 57, "Complete set of golf clubs for beginners", "Golf Clubs Set", 299.99m, 6 },
                    { 58, "Telescopic fishing rod with reel", "Fishing Rod", 59.99m, 6 },
                    { 59, "Pair of 5kg adjustable dumbbells", "Dumbbells", 49.99m, 6 },
                    { 60, "Anti-fog ski goggles with UV protection", "Ski Goggles", 39.99m, 6 },
                    { 61, "Gentle facial cleanser for all skin types", "Facial Cleanser", 14.99m, 7 },
                    { 62, "Moisturizing shampoo for dry hair", "Shampoo", 12.99m, 7 },
                    { 63, "Luxury perfume with floral notes", "Perfume", 79.99m, 7 },
                    { 64, "Rechargeable electric razor for men", "Electric Razor", 89.99m, 7 },
                    { 65, "Professional hair dryer with diffuser", "Hair Dryer", 59.99m, 7 },
                    { 66, "Complete makeup kit with 30 colors", "Makeup Kit", 49.99m, 7 },
                    { 67, "Daily face moisturizer with SPF 30", "Moisturizer", 19.99m, 7 },
                    { 68, "10-piece nail polish set with various colors", "Nail Polish Set", 29.99m, 7 },
                    { 69, "Sonic electric toothbrush with timer", "Electric Toothbrush", 69.99m, 7 },
                    { 70, "Hydrating sheet face masks, pack of 10", "Face Mask", 24.99m, 7 },
                    { 71, "Creative building blocks set, 500 pieces", "Building Blocks", 39.99m, 8 },
                    { 72, "Family strategy board game", "Board Game", 29.99m, 8 },
                    { 73, "High-speed remote control race car", "Remote Control Car", 49.99m, 8 },
                    { 74, "Soft plush teddy bear, 18 inches", "Stuffed Animal", 19.99m, 8 },
                    { 75, "Collectible superhero action figure", "Action Figure", 24.99m, 8 },
                    { 76, "1000-piece landscape jigsaw puzzle", "Puzzle", 17.99m, 8 },
                    { 77, "Family card game for 2-6 players", "Card Game", 12.99m, 8 },
                    { 78, "STEM learning toy for ages 8-12", "Educational Toy", 34.99m, 8 },
                    { 79, "Beginner drone with HD camera", "Drone", 89.99m, 8 },
                    { 80, "Wooden doll house with furniture", "Doll House", 69.99m, 8 },
                    { 81, "Premium single-origin coffee beans, 1lb", "Gourmet Coffee", 18.99m, 9 },
                    { 82, "Assorted gourmet chocolates, 24 pieces", "Chocolate Box", 29.99m, 9 },
                    { 83, "Organic herbal tea assortment, 40 bags", "Organic Tea", 14.99m, 9 },
                    { 84, "Artisanal Italian pasta varieties, 5 pack", "Pasta Set", 22.99m, 9 },
                    { 85, "Extra virgin olive oil, 500ml", "Olive Oil", 19.99m, 9 },
                    { 86, "Gourmet spice collection, 12 jars", "Spice Collection", 39.99m, 9 },
                    { 87, "Premium red wine, vintage 2018", "Wine Bottle", 49.99m, 9 },
                    { 88, "Assorted fruit jam set, 4 jars", "Jam Set", 24.99m, 9 },
                    { 89, "Gourmet cheese assortment, 5 types", "Cheese Selection", 34.99m, 9 },
                    { 90, "High-protein snack bars, box of 12", "Protein Bars", 19.99m, 9 },
                    { 91, "Premium ruled notebook, 200 pages", "Notebook", 12.99m, 10 },
                    { 92, "Luxury ballpoint pen set, 5 pens", "Pen Set", 29.99m, 10 },
                    { 93, "Ergonomic office chair with lumbar support", "Office Chair", 199.99m, 10 },
                    { 94, "Multi-compartment desk organizer", "Desk Organizer", 24.99m, 10 },
                    { 95, "Wireless all-in-one color printer", "Printer", 159.99m, 10 },
                    { 96, "Cross-cut paper shredder for home office", "Paper Shredder", 49.99m, 10 },
                    { 97, "2-drawer metal filing cabinet", "Filing Cabinet", 79.99m, 10 },
                    { 98, "Wall-mounted magnetic whiteboard, 36x24\"", "Whiteboard", 39.99m, 10 },
                    { 99, "Scientific calculator with 417 functions", "Calculator", 19.99m, 10 },
                    { 100, "Heavy-duty stapler with 5000 staples", "Stapler", 14.99m, 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "Shorts");
        }
    }
}
