using System.ComponentModel.DataAnnotations.Schema;

namespace Financial_Tracker.Models
{
    [Table("expenses")]
    public class Expense
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Category { get; set; } // This could be an enum for better type safety

        public Account Account { get; set; }
    }
}
