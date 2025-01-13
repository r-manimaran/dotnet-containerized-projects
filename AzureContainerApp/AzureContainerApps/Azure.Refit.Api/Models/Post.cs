namespace Azure.Refit.Api.Models
{
    public class Post
    {
        public  int Id  { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;   
        
    }
}
