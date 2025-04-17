using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product.Service.Models;

internal class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public required decimal Price { get; set; }
    public required int ProductTypeId { get; set; }
    [ForeignKey("ProductTypeId")]
    public ProductType? ProductType { get; set; }
}
