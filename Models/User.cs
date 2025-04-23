using System;
using System.ComponentModel.DataAnnotations;

namespace SkylineHOA.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? ContactNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Role { get; set; }
    }
}
