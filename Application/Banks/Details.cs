using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Banks
{
    public class Details
    {
        public class Query : IRequest<Result<Bank>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<Bank>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Bank>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.Banks.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<Bank>.Failure(ResponseConstants.Bank.NotFound);
                }
                return Result<Bank>.Success(envelope);
            }
        }

    }
}
