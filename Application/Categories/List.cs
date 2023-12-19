using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Categories
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<Category>>>
        {
            public CategoryParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<Category>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<Category>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Categories
                                    .OrderBy(x => x.Name)
                                    .AsQueryable();
                return Result<PagedList<Category>>
                       .Success(await PagedList<Category>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
