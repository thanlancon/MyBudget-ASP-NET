using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Envelopes
{
    public class RecalculateTotalBalance
    {
        public class Query : IRequest { }

        public class Handler : IRequestHandler<Query>
        {
            private DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task Handle(Query request, CancellationToken cancellationToken)
            {
                var envelopes = await _context.Envelopes.ToListAsync();
                var transactions = await _context.Transactions.ToListAsync();

                int[] envelopeIndex = Enumerable.Repeat(0, envelopes.Count).ToArray();
                for (int i = 0; i < envelopes.Count; i++)
                {
                    envelopes[i].TotalBalance = 0;
                }
                for (int i = 0; i < transactions.Count; i++)
                {
                    var envelope = envelopes.Find(e => e.Id == transactions[i].EnvelopeId);

                    if (envelope != null)
                    {
                        envelope.TotalBalance += transactions[i].Inflow - transactions[i].Outflow;
                    }

                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
