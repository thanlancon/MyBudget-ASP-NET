using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Banks
{
    public static class Core
    {

        public static async Task<string> IsCreateAble(DataContext context, Bank item)
        {
            var count = await context.Banks.CountAsync(e => e.Code == item.Code);
            if (count > 0)
            {
                return ResponseConstants.Bank.CodeIsExist;
            }
            count = await context.Banks.CountAsync(e => e.Name == item.Name);
            if (count > 0)
            {
                return ResponseConstants.Bank.NameIsExist;
            }

            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context, Guid Id)
        {
            var count = await context.BankAccounts.CountAsync(e => e.BankId == Id);
            if (count > 0)
            {
                return ResponseConstants.Bank.BankAccountIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, Bank item)
        {
            var count = await context.Banks.CountAsync(
                e => e.Code == item.Code && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.Bank.CodeIsExist;
            }
            count = await context.Banks.CountAsync(
                e => e.Name == item.Name && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.Bank.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
