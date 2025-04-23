using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkylineHOA.Models
{
    [Table("requests")]
    public class Request
    {
        [Key]
        [Column("request_id")]
        public string RequestId { get; set; } = Guid.NewGuid().ToString(); // because it's varchar, not int

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("request_type")]
        public string? RequestType { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("urgency")]
        public string? Urgency { get; set; }

        [Column("date_submitted")]
        public DateTime DateSubmitted { get; set; }

        [Column("date_updated")]
        public DateTime? DateUpdated { get; set; }

        [Column("attachment_path")]
        public string? AttachmentPath { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }
    }
}
