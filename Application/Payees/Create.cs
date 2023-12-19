using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Payees
{
    public class Create
    {
        public class Command : IRequest<Result<string>>
        {
            public Payee Payee { get; set; }
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
                string resultCode = await Core.IsCreateAble(_context, request.Payee);
                if (resultCode == ResponseConstants.IsUpdateAble)
                {
                    _context.Payees.Add(request.Payee);
                    await _context.SaveChangesAsync();
                    return Result<string>.Success(ResponseConstants.UpdateSuccess);
                }
                return Result<string>.Failure(ResponseConstants.Payee.NameIsExist);
            }

        }
    }
}
