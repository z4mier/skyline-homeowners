using System;
using System.ComponentModel.DataAnnotations;

namespace SkylineHOA.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string RequestType { get; set; }

        public string Urgency { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
        public string? ApprovedBy { get; set; }

    }
}
