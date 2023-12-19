using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Categories
{
    public class Details
    {
        public class Query : IRequest<Result<Category>>
        {
            public Guid Id;
        }

        public class Handler : IRequestHandler<Query, Result<Category>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Category>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelope= await _context.Categories.FindAsync(request.Id);
                if(envelope == null)
                {
                    return Result<Category>.Failure(ResponseConstants.Category.NotFound);
                }
                return Result<Category>.Success(envelope);
            }
        }

    }
}
