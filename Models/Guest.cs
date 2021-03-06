using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        public int UserId { get; set; }

        public int WeddingId { get; set; }

        public User AGuest { get; set; }

        public Wedding AWedding { get; set; }
    }
}