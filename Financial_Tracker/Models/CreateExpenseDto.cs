namespace Financial_Tracker.Models
{
    public class CreateExpenseDto
    {
        public int AccountId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Category { get; set; }
    }
}
