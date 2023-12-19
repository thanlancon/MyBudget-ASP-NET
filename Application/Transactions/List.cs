using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Transactions
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<Transaction>>>
        {
            public TransactionParams Params { get; set; }
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
                return await LoadList(request.Params.BankID,request.Params.PageNumber,request.Params.PageSize);
            }
            public async Task<Result<PagedList<Transaction>>> LoadList(Guid bankID, int page = 0, int pagesize = 0)
            {

                var query = _context.Transactions
                                    .Where(x => x.BankId == bankID)
                                    .OrderByDescending(d => d.TransactionDate)
                                    .ThenByDescending(d => d.PostDate)
                                    .ThenByDescending(d => d.SequenceNumber)
                                    .AsQueryable();
                return Result<PagedList<Transaction>>
                       .Success(await PagedList<Transaction>.CreateAsync(query,
                           page, pagesize));

            }
        }
    }
}
