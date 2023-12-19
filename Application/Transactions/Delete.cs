using Application.Core;
using MediatR;
using Persistence;

namespace Application.Transactions
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
                var item = await _context.Transactions.FindAsync(request.Id);
                if (item != null)
                {
                    string resultCode = Core.IsDeleteAble(_context, item);
                    if (resultCode == ResponseConstants.IsUpdateAble)
                    {
                        TransactionActions transactionActions = new TransactionActions(item, _context);
                        return await transactionActions.UpdateTransaction(true);
                    }
                    return Result<string>.Failure(resultCode);
                }
                return Result<string>.Failure(ResponseConstants.Transaction.NotFound);
            }
        }
    }
}
