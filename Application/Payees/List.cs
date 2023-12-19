using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Payees
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<Payee>>>
        {
            public PayeeParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<Payee>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<Payee>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Payees
                                    .OrderBy(x => x.Name)
                                    .AsQueryable();
                return Result<PagedList<Payee>>
                       .Success(await PagedList<Payee>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
