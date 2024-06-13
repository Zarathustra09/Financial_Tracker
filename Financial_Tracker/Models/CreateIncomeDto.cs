using System.ComponentModel.DataAnnotations;

namespace Financial_Tracker.Models
{
    public class CreateIncomeDto
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
