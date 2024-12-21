using Shared.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.DTOs;

public record OrderSummary(int Id, List<ProductList> Products, decimal TotalAmount, DateTime OrderedDate);
