namespace AppreciateAppApi.DTO.Appreciation
{
    public class CreateAppreciationRequest
    {
        public List<Receiver> Receivers { get; set; } = new List<Receiver>();
        public int CategoryId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int SenderId { get; set; }        
    }
}
