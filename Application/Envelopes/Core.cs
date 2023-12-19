using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Envelopes
{
    public static class Core
    {
        
        public static async Task<string> IsCreateAble(DataContext context, Envelope item)
        {
            var count = await context.Envelopes.CountAsync(e => e.Name == item.Name);
            if (count > 0)
            {
                return ResponseConstants.Envelope.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context, Envelope item)
        {
            var count = await context.Transactions.CountAsync(e => e.EnvelopeId == item.Id);
            if (count > 0)
            {
                return ResponseConstants.Envelope.TransactionExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, Envelope item)
        {
            var count = await context.Envelopes.CountAsync(
                e => e.Name == item.Name && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.Envelope.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
