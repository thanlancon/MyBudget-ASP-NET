using System.Linq;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Envelopes
{
    /// <summary>
    /// Used to get envelopes's balance of all activities in a monthly
    /// </summary>
    public class MonthlyActivitiesBalance
    {
        public class Query : IRequest<Result<List<EnvelopeMonthlyBalance>>>
        {
            public int month { get; set; }
            public int year { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<EnvelopeMonthlyBalance>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<EnvelopeMonthlyBalance>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = from t in _context.Transactions
                            where t.TransactionDate.Month == request.month && t.TransactionDate.Year == request.year
                            group t by new { t.EnvelopeId, t.TransactionDate.Month, t.TransactionDate.Year } into newT
                            select new EnvelopeMonthlyBalance
                            {
                                Month = request.month,
                                Year = request.year,
                                Id = newT.Key.EnvelopeId,
                                Balance = newT.Sum<Transaction>(x => x.Inflow - x.Outflow)
                            };

                var items = await query.ToListAsync();
                if (items == null)
                {
                    return Result<List<EnvelopeMonthlyBalance>>.Failure("Cannot load envelope montly balance");
                }
                return Result<List<EnvelopeMonthlyBalance>>.Success(items);
            }
        }
    }
}
