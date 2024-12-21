using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.DTOs;

public class OrderRequest
{
    public List<ProductDto> Products { get; set; }
}
public class ProductDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 0;
    public decimal Price { get; set; }

}
