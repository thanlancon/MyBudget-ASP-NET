using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Transactions
{
    public class Create
    {
        public class Command : IRequest<Result<string>>
        {
            public Transaction Transaction { get; set; }
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
                if (request.Transaction != null)
                {
                    string resultCode = Core.IsDeleteAble(_context, request.Transaction);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        TransactionActions transactionActions = new TransactionActions(request.Transaction, _context);
                        return await transactionActions.UpdateTransaction();
                    }
                    return Result<string>.Failure(resultCode);
                }
                return Result<string>.Failure(ResponseConstants.Transaction.NotFound);
            }
        }
    }
}
