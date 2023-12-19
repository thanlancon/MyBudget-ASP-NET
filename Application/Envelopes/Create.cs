using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Envelopes
{
    public class Create
    {
        public class Command : IRequest<Result<string>>
        {
            public Envelope Envelope { get; set; }
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
                string resultCode = await Core.IsCreateAble(_context, request.Envelope);
                if (resultCode == ResponseConstants.IsUpdateAble)
                {
                    _context.Envelopes.Add(request.Envelope);
                    await _context.SaveChangesAsync();
                    return Result<string>.Success(ResponseConstants.UpdateSuccess);
                }
                return Result<string>.Failure(ResponseConstants.Envelope.NameIsExist);
            }

        }
    }
}
