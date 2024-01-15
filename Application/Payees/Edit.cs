using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Payees
{
    public class Edit
    {
        public class Command : IRequest<Result<string>>
        {
            public Payee Payee { get; set; }
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
                var item = await _context.Payees.FindAsync(request.Payee.Id);
                if (item != null)
                {
                    string resultCode = await Core.IsEditAble(_context, request.Payee);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        _mapper.Map(request.Payee, item);
                        await _context.SaveChangesAsync();
                        return Result<string>.Success(ResponseConstants.UpdateSuccess);
                    }
                    return Result<string>.Failure(resultCode);
                }
                return Result<string>.Failure(ResponseConstants.Payee.NotFound);
            }
        }
    }
}
