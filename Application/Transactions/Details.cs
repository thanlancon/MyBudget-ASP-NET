using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Transactions
{
    public class Details
    {
        public class Query : IRequest<Result<Transaction>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<Transaction>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Transaction>> Handle(Query request, CancellationToken cancellationToken)
            {
                var transaction= await _context.Transactions.FindAsync(request.Id);
                if (transaction != null) 
                {
                    return Result<Transaction>.Success(transaction);
                }
                return Result<Transaction>.Failure(ResponseConstants.Transaction.NotFound);
            }
        }

    }
}
