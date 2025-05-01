namespace SkylineHOA.Models
{
    public class RequestWithUser
    {
        public int RequestId { get; set; }
        public string? RequestType { get; set; }
        public string? Status { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
