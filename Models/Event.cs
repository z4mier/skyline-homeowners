using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkylineHOA.Models
{
    [Table("Events")] 
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [MaxLength(10)]
        public string StartTime { get; set; } 

        [Required(ErrorMessage = "End Time is required.")]
        [MaxLength(10)]
        public string EndTime { get; set; } 

        [Required(ErrorMessage = "Venue is required.")]
        [MaxLength(100)]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(300)]
        public string Description { get; set; }
    }
}
