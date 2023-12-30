using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Transactions
{
    public class MonthlyByEnvelope
    {
        public class Query : PagingParams, IRequest<Result<PagedList<Transaction>>>
        {
            public MonthlyEnvelopeParams Params { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<PagedList<Transaction>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<Transaction>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = from t in _context.Transactions
                            where t.EnvelopeId == request.Params.envelopeID
                                && t.TransactionDate.Month == request.Params.month
                                && t.TransactionDate.Year == request.Params.year
                            select t;
                return Result<PagedList<Transaction>>.Success(await PagedList<Transaction>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
