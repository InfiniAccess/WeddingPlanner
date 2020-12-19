using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        [MinLength(2, ErrorMessage = "Requires atleast two characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name:")]
        [MinLength(2, ErrorMessage = "Requires atleast two characters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address:")]
        public string Email { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Password must be atleast 2 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password must match.")]
        [NotMapped]
        [Display(Name = "Confirm Password:")]
        public string Confirm { get; set; }

        // Nav Prop - One to Many - A user can have many weddings.
        List<Wedding> MyWeddings { get; set; }

        List<Guest> AttendingGuests { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}