using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.ReadOnlyList
{
    public class ListBankAccountTypes
    {

        public class Query : IRequest<List<ValueAndText>> { };
        public class Handler : IRequestHandler<Query, List<ValueAndText>>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<List<ValueAndText>> Handle(Query request, CancellationToken cancellationToken)
            {
                var returnList = new List<ValueAndText>();
                var listItems = await _context.BankAccountTypes.ToListAsync();
                listItems.ForEach(li =>
                {
                    returnList.Add(new ValueAndText
                    {
                        Value = li.Id.ToString(),
                        Text = li.Code
                    });
                });
                return returnList;
            }
        }
    }
}
