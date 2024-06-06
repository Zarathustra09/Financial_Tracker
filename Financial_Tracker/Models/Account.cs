using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial_Tracker.Models
{
    [Table("accounts")]
    public class Account
    {
      
        public int Id { get; set; }
        public int userId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public User User { get; set; }
        public ICollection<Expense> Expense { get; set; }
    }
}
