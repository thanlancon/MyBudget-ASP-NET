using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.BankAccounts
{
    public class Details
    {
        public class Query : IRequest<Result<BankAccount>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<BankAccount>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<BankAccount>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.BankAccounts.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<BankAccount>.Failure(ResponseConstants.BankAccounts.NotFound);
                }
                return Result<BankAccount>.Success(envelope);
            }
        }

    }
}
