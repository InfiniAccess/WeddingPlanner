using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Display(Name = "Wedder One: ")]
        [Required(ErrorMessage = "Must have a wedder.")]
        public string WedderOne { get; set; }
        [Required(ErrorMessage = "Must have a wedder.")]
        [Display(Name = "Wedder Two: ")]
        public string WedderTwo { get; set; }
        [Required(ErrorMessage = "Must have a date.")]
        [Display(Name = "Date: ")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Address: ")]
        [Required(ErrorMessage = "Must have an address.")]
        public string Address { get; set; }

        public int UserId { get; set; }

        public User Creator { get; set; }

        public List<Guest> Attendees { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}