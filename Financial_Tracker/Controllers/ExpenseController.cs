using Financial_Tracker.DataConnection;
using Financial_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Financial_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly DbContextClass _context;

        public ExpenseController(DbContextClass context)
        {
            _context = context;
        }

        [HttpGet("user-expenses")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetUserExpenses()
        {
            // Get the username from the JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in JWT token.");
            }

            // Find the user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find all accounts associated with the user
            var accounts = await _context.Account
                .Where(a => a.userId == user.Id)
                .ToListAsync();

            if (accounts == null || accounts.Count == 0)
            {
                return NotFound("User does not have any accounts.");
            }

            // Get all expenses associated with the user's accounts
            var accountIds = accounts.Select(a => a.Id).ToList();
            var expenses = await _context.Expense
                .Where(e => accountIds.Contains(e.AccountId))
                .ToListAsync();

            if (expenses == null || expenses.Count == 0)
            {
                return NotFound("User does not have any expenses.");
            }

            return Ok(expenses);
        }
        [HttpPost("create-expense")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto createExpenseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the username from the JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Username not found in JWT token.");
            }

            // Find the user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find the account associated with the expense
            var account = await _context.Account
                .SingleOrDefaultAsync(a => a.Id == createExpenseDto.AccountId && a.userId == user.Id);

            if (account == null)
            {
                return NotFound("Account not found or does not belong to the user.");
            }

            // Check if the account has sufficient balance
            if (account.Balance < createExpenseDto.Amount)
            {
                return BadRequest("Insufficient balance in the account.");
            }

            // Deduct the expense amount from the account balance
            account.Balance -= createExpenseDto.Amount;

            // Create the expense object
            var expense = new Expense
            {
                AccountId = createExpenseDto.AccountId,
                Description = createExpenseDto.Description,
                Amount = createExpenseDto.Amount,
                Category = createExpenseDto.Category
            };

            // Add the expense to the database
            _context.Expense.Add(expense);

            // Save changes to the database within a transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "An error occurred while processing the request.");
                }
            }

            return Ok(expense);
        }

    }
}
