using HomeLoanPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeLoanPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<LoanDocument> LoanDocuments { get; set; }
        public DbSet<LoanStatusHistory> LoanStatusHistories { get; set; }
        public DbSet<LoanAccount> LoanAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Example: one-to-many between LoanApplication and Documents
            builder.Entity<LoanApplication>()
                   .HasMany(l => l.Documents)
                   .WithOne(d => d.LoanApplication)
                   .HasForeignKey(d => d.LoanApplicationId);

            builder.Entity<LoanApplication>()
                   .HasMany(l => l.StatusHistory)
                   .WithOne(h => h.LoanApplication)
                   .HasForeignKey(h => h.LoanApplicationId);

            builder.Entity<LoanApplication>()
                   .HasOne(l => l.LoanAccount)
                   .WithOne(a => a.LoanApplication)
                   .HasForeignKey<LoanAccount>(a => a.LoanApplicationId);
        }
    }
}
