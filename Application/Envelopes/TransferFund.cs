using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Persistence;

namespace Application.Envelopes
{
    public class TransferFund
    {
        public class Command : IRequest<Result<string>>
        {
            public EnvelopeBalanceTransfer envelopeBalanceTransfer { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var envelopeFrom = await _context.Envelopes.FindAsync(request.envelopeBalanceTransfer.envelopeID_From);
                var envelopeTo = await _context.Envelopes.FindAsync(request.envelopeBalanceTransfer.envelopeID_To);
                if (envelopeFrom != null && envelopeTo != null)
                {
                    envelopeFrom.TotalBalance -= request.envelopeBalanceTransfer.balanceTransfer;
                    envelopeTo.TotalBalance += request.envelopeBalanceTransfer.balanceTransfer;
                    await _context.SaveChangesAsync();
                    return Result<string>.Success(ResponseConstants.UpdateSuccess);
                }
                else
                {
                    return Result<string>.Failure(ResponseConstants.Envelope.NotFound);
                }
            }
        }
    }
}
