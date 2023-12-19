using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BankAccounts
{
    public static class Core
    {

        public static async Task<string> IsCreateAble(DataContext context, BankAccount item)
        {
            var count = await context.BankAccounts.CountAsync(e => e.Code == item.Code);
            if (count > 0)
            {
                return ResponseConstants.BankAccounts.CodeIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context, BankAccount item)
        {
            var count = await context.Transactions.CountAsync(e => e.BankId == item.Id);
            if (count > 0)
            {
                return ResponseConstants.BankAccounts.TransactionExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, BankAccount item)
        {
            var count = await context.BankAccounts.CountAsync(
                e => e.Code == item.Code && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.BankAccounts.CodeIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
