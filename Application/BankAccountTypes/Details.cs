using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.BankAccountTypes
{
    public class Details
    {
        public class Query : IRequest<Result<BankAccountType>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<BankAccountType>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<BankAccountType>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.BankAccountTypes.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<BankAccountType>.Failure(ResponseConstants.BankAccountType.NotFound);
                }
                return Result<BankAccountType>.Success(envelope);
            }
        }

    }
}
