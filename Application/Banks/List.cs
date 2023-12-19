using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Banks
{
   public class List
    {
        public class Query : IRequest<Result<PagedList<Bank>>>
        {
            public BankParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<Bank>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<Bank>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Banks
                                    .OrderBy(x => x.Code)
                                    .AsQueryable();
                return Result<PagedList<Bank>>
                       .Success(await PagedList<Bank>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
