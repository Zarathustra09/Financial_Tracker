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
    public class AccountController : ControllerBase
    {
        private readonly DbContextClass _context;

        public AccountController(DbContextClass context)
        {
            _context = context;
        }

        [HttpGet("user-accounts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Account>>> GetUserAccounts()
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

            return Ok(accounts);
        }

        [HttpPost("create-account")]
        [Authorize]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
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

            // Create a new account
            var account = new Account
            {
                Name = createAccountDto.Name,
                Balance = createAccountDto.Balance,
                userId = user.Id
            };

            // Add the new account to the database
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPost("add-income")]
        [Authorize]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeDto createIncomeDto)
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

            // Find the account associated with the income
            var account = await _context.Account
                .SingleOrDefaultAsync(a => a.Id == createIncomeDto.AccountId && a.userId == user.Id);

            if (account == null)
            {
                return NotFound("Account not found or does not belong to the user.");
            }

            // Add the income amount to the account balance
            account.Balance += createIncomeDto.Amount;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(account);
        }

    }
}
