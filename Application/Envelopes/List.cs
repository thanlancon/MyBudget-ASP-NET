using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;

namespace Application.Envelopes
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<Envelope>>>
        {
            public EnvelopeParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<Envelope>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<Envelope>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Envelopes
                                    .OrderBy(x => x.Name)
                                    .AsQueryable();
                return Result<PagedList<Envelope>>
                       .Success(await PagedList<Envelope>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
