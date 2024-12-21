using MassTransit.Saga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.DTOs;

public record ProductList(int ProductId, string ProductName, decimal Price, int Quantity);

