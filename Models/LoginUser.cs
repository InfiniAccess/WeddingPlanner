using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required to login.")]
        [EmailAddress]
        public string LoginEmail { get; set; }
        [Required(ErrorMessage = "Password is required to login.")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

    }
}