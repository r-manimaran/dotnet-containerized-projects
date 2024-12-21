using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Models;

public class Order
{
    public Order()
    {
        Products  = new List<OrderProduct>();
        OrderedDate = DateTime.UtcNow;
    }
    [Key]
    public int Id { get; set; }
    [Required]
    public virtual ICollection<OrderProduct> Products{ get;set; } 
    [Required]
    public DateTime OrderedDate { get; set; }
    [Required]
    [Column(TypeName ="decimal(18,2)")]
    public decimal TotalPrice { get; set; }
}

public class OrderProduct
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName ="decimal(18,2)")]
    public decimal Price { get; set; }

    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}
