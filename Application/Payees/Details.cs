using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Payees
{
    public class Details
    {
        public class Query : IRequest<Result<Payee>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<Payee>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Payee>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.Payees.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<Payee>.Failure(ResponseConstants.Payee.NotFound);
                }
                return Result<Payee>.Success(envelope);
            }
        }

    }
}
