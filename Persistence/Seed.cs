using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tom",
                        UserName = "tom",
                        Email = "tom@test.com"
                    },
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }

                if (!context.Banks.Any())
                {
                    await context.AddRangeAsync(SeedBanks());
                }
                if (!context.BankAccountTypes.Any())
                {
                    await context.AddRangeAsync(SeedBankAccountType());
                }
                await context.SaveChangesAsync();
            }
        }
        public static List<Bank> SeedBanks()
        {
            return new List<Bank>
            {
                new Bank
                {
                    Code ="US Bank",
                    Name = "US Bank",
                },
                new Bank
                {
                    Code = "Cap One",
                    Name = "Captital One Bank"
                },
                new Bank
                {
                    Code = "Discover",
                    Name = "Discover Bank"
                }
            };
        }
        public static List<BankAccountType> SeedBankAccountType()
        {
            return new List<BankAccountType>
            {
                new BankAccountType
                {
                    Code = "Credit",
                    Name = "Credit card account"
                },
                new BankAccountType
                {
                    Code = "Checking",
                    Name = "Checking account"
                },
                new BankAccountType
                {
                    Code = "Saving",
                    Name = "Saving account"
                }
            };
        }
    }
}
