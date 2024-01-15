using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Payees
{
    public static class Core
    {

        public static async Task<string> IsCreateAble(DataContext context, Payee item)
        {
            var count = await context.Payees.CountAsync(e => e.Name == item.Name);
            if (count > 0)
            {
                return ResponseConstants.Payee.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context, Guid Id)
        {
            var count = await context.Transactions.CountAsync(e => e.PayeeId == Id);
            if (count > 0)
            {
                return ResponseConstants.Payee.TransactionExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, Payee item)
        {
            var count = await context.Payees.CountAsync(
                e => e.Name == item.Name && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.Payee.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
