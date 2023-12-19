using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Transactions
{
    public class Edit
    {
        public class Command : IRequest<Result<string>>
        {
            public Transaction Transaction { get; set; }
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
                var item = await _context.Transactions.FindAsync(request.Transaction.Id);
                _mapper.Map(request.Transaction, item);
                TransactionActions transactionActions = new TransactionActions(item, _context);
                return await transactionActions.UpdateTransaction();
            }
        }
    }
}
