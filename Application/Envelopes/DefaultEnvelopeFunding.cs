using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Envelopes
{
    public class DefaultEvelopeFunding
    {
        public class Query : IRequest<Result<string>>
        {
            public string EnvelopeName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<string>>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var envelopes = await _context.Envelopes.ToListAsync();
                var envelopeID = _context.Envelopes.FirstOrDefault(x => x.Name == request.EnvelopeName).Id;
                if (envelopeID != Guid.Empty)
                {
                    for (int i = 0; i < envelopes.Count; i++)
                    {
                        envelopes[i].EnvelopeId_Funding = envelopeID;
                    }

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
