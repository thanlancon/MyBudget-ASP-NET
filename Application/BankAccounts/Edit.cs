using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.BankAccounts
{
    public class Edit
    {
        public class Command : IRequest<Result<string>>
        {
            public BankAccount BankAccount { get; set; }
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
                var item = await _context.BankAccounts.FindAsync(request.BankAccount.Id);
                if (item != null)
                {
                    string resultCode = await Core.IsEditAble(_context, item);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        _mapper.Map(request.BankAccount, item);
                        await _context.SaveChangesAsync();
                        return Result<string>.Success(ResponseConstants.UpdateSuccess);
                    }
                    return Result<string>.Failure(resultCode);
                }
                return Result<string>.Failure(ResponseConstants.BankAccounts.NotFound);
            }
        }
    }
}
