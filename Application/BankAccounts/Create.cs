using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.BankAccounts
{
    public class Create
    {
        public class Command : IRequest<Result<string>>
        {
            public BankAccount BankAccount { get; set; }
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
                string resultCode = await Core.IsCreateAble(_context, request.BankAccount);
                if (resultCode == ResponseConstants.IsUpdateAble)
                {
                    _context.BankAccounts.Add(request.BankAccount);
                    await _context.SaveChangesAsync();
                    return Result<string>.Success(ResponseConstants.UpdateSuccess);
                }
                return Result<string>.Failure(resultCode);
            }

        }
    }
}
