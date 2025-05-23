using Microsoft.Extensions.VectorData;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogApi.Models;

public class ProductVector
{
    [VectorStoreKey]
    public int Id { get; set; }
    [VectorStoreData]
    public string Name { get; set; } = default!;
    [VectorStoreData]
    public string Description { get; set; } = default!;
    [VectorStoreData]
    public string ImageUrl { get; set; } = default!;
    [VectorStoreData]
    public decimal Price { get; set; }
    [NotMapped]
    [VectorStoreVector(384,DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; } = Array.Empty<float>();
}
