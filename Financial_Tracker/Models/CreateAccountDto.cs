using System.ComponentModel.DataAnnotations;

namespace Financial_Tracker.Models
{
    public class CreateAccountDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}
