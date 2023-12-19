using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BankAccountTypes
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<BankAccountType>>>
        {
            public BankAccountTypeParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<BankAccountType>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<BankAccountType>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.BankAccountTypes
                                    .OrderBy(x => x.Code)
                                    .AsQueryable();
                return Result<PagedList<BankAccountType>>
                       .Success(await PagedList<BankAccountType>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
