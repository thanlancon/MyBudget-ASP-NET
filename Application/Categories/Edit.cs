using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Categories
{
    public class Edit
    {
        public class Command : IRequest<Result<string>>
        {
            public Category Category { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var item = await _context.Categories.FindAsync(request.Category.Id);
                if (item != null)
                {
                    string resultCode = await Core.IsEditAble(_context, item);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        _mapper.Map(request.Category, item);
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
