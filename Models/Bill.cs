using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SkylineHOA.Models
{
    public class Bill
    {
        public int BillId { get; set; }

        [BindNever] // prevent model binding for UserId; it will be set manually in the controller
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Amenity name is required.")]
        public string AmenityName { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Reservation fee is required.")]
        public decimal ReservationFee { get; set; }

        [Required(ErrorMessage = "Total fee is required.")]
        public decimal TotalFee { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }

        public string? Remarks { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
