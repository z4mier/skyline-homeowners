using System;
using System.ComponentModel.DataAnnotations;

namespace SkylineHOA.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000)]
        public string Message { get; set; }

        [Required(ErrorMessage = "Target audience is required.")]
        public string Target { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
    }
}
