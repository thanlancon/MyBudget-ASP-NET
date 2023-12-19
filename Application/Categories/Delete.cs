using Application.Core;
using MediatR;
using Persistence;

namespace Application.Categories
{
    public class Delete
    {
        public class Command : IRequest<Result<string>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var item = await _context.Categories.FindAsync(request.Id);
                if (item != null)
                {
                    string resultCode = await Core.IsDeleteAble(_context, item);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        _context.Categories.Remove(item);
                        await _context.SaveChangesAsync();
                        return Result<string>.Success(ResponseConstants.UpdateSuccess);
                    }
                    return Result<string>.Failure(resultCode);
                }
                return Result<string>.Failure(ResponseConstants.Category.NotFound);
            }
        }
    }
}
