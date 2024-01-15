using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BankAccountTypes
{
    public static class Core
    {

        public static async Task<string> IsCreateAble(DataContext context, BankAccountType item)
        {
            var count = await context.BankAccountTypes.CountAsync(e => e.Code == item.Code);
            if (count > 0)
            {
                return ResponseConstants.BankAccountType.CodeIsExist;
            }

            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context,  Guid Id)
        {
            var count = await context.BankAccounts.CountAsync(e => e.BankAccountTypeId == Id);
            if (count > 0)
            {
                return ResponseConstants.BankAccountType.BankAccountIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, BankAccountType item)
        {
            var count = await context.BankAccountTypes.CountAsync(
                e => e.Code == item.Code && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.BankAccountType.CodeIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
