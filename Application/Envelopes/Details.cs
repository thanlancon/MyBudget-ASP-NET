using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Envelopes
{
    public class Details
    {
        public class Query : IRequest<Result<Envelope>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<Envelope>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Envelope>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.Envelopes.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<Envelope>.Failure(ResponseConstants.Envelope.NotFound);
                }
                return Result<Envelope>.Success(envelope);
            }
        }

    }
}
