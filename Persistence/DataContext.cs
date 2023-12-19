
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bank> Banks {get;set;}
        public DbSet<BankAccount> BankAccounts {get;set;}
        public DbSet<BankAccountType> BankAccountTypes {get;set;}
        public DbSet<Category> Categories {get;set;}
        public DbSet<Envelope> Envelopes {get;set;}
        public DbSet<Payee> Payees {get;set;}
        public DbSet<Transaction> Transactions {get;set;}
    }
}
