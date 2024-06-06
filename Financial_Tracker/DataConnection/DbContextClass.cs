using Financial_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using static Financial_Tracker.Models.User;

namespace Financial_Tracker.DataConnection
{
    public class DbContextClass : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbContextClass(DbContextOptions<DbContextClass> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Login>().HasNoKey(); // Configure Login as a keyless entity
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Expense> Expense { get; set; }



    }
}