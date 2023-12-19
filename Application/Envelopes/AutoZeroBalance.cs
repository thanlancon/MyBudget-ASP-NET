using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Envelopes
{
    public class AutoZeroBalance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid envelopeID;
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var envelope = await _context.Envelopes.FindAsync(request.envelopeID);
                if (envelope != null)
                {
                    var envelopeFunding = await _context.Envelopes.FindAsync(envelope.EnvelopeId_Funding);
                    decimal balance = envelope.TotalBalance;
                    envelope.TotalBalance = 0;
                    envelopeFunding.TotalBalance += balance;
                    await _context.SaveChangesAsync();
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    return Result<Unit>.Failure(ResponseConstants.Envelope.NotFound);
                }
            }
        }
    }
}
