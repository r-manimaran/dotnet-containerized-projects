using AppreciateAppApi.Models;

namespace AppreciateAppApi.DTO.Appreciation;

// Recogonize Appreciate and Value Everyone (RAVE)
public class Appreciation
{
    public int CategoryId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Sender Sender { get; set; }
    public List<Receiver> Receivers { get; set; } = new List<Receiver>();
    public AppreciationType AppreciationType { get; set; }
}
