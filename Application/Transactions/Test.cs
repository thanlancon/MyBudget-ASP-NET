using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Transactions
{
    public class Test
    {
        public class Query : IRequest<List<Transaction>>
        {
            public Transaction transaction { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<Transaction>>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<List<Transaction>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Transactions
                .Where(t => t.BankId == request.transaction.BankId)
                .Where(t => t.TransactionDate >= request.transaction.TransactionDate)
                .Where(t => t.PostDate >= request.transaction.PostDate)
                .Where(t => t.Id != request.transaction.Id)
                .OrderBy(t => t.TransactionDate)
                .OrderBy(t => t.PostDate)
                .ThenBy(t => t.SequenceNumber);
                return await query.ToListAsync();
            }
        }
    }
}
