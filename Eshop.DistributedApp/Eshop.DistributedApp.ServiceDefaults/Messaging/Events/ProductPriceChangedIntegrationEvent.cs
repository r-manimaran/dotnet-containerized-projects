using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.DistributedApp.ServiceDefaults.Messaging.Events;

public record ProductPriceChangedIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; }= default!;
    public decimal Price { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
}
