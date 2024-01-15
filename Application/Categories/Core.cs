using Application.Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Categories
{
    public static class Core
    {

        public static async Task<string> IsCreateAble(DataContext context, Category item)
        {
            var count = await context.Categories.CountAsync(e => e.Name == item.Name);
            if (count > 0)
            {
                return ResponseConstants.Category.NameIsExist;
            }


            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsDeleteAble(DataContext context, Guid Id)
        {
            var count = await context.Envelopes.CountAsync(e => e.CategoryId == Id);
            if (count > 0)
            {
                return ResponseConstants.Category.EnvelopeExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
        public static async Task<string> IsEditAble(DataContext context, Category item)
        {
            var count = await context.Categories.CountAsync(
                e => e.Name == item.Name && e.Id != item.Id);
            if (count > 0)
            {
                return ResponseConstants.Category.NameIsExist;
            }
            return ResponseConstants.IsUpdateAble;
        }
    }
}
