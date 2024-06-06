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
    public class DashboardController : ControllerBase
    {
        private readonly DbContextClass _context;

        public DashboardController(DbContextClass context)
        {
            _context = context;
        }

        [HttpGet("user-account")]
        public async Task<ActionResult<Account>> GetUserAccount()
        {
            // Get the username from the JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username not found in JWT token.");
            }

            // Find the user by username
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find the account associated with the user
            var account = await _context.Account
                .FirstOrDefaultAsync(a => a.userId == user.Id);

            if (account == null)
            {
                return NotFound("User does not have an account.");
            }

            return Ok(account);
        }

        


    }
}
