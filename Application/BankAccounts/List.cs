using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BankAccounts
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<BankAccount>>>
        {
            public BankAccountParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<BankAccount>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<PagedList<BankAccount>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.BankAccounts
                                    .OrderBy(x => x.Code)
                                    .AsQueryable();
                return Result<PagedList<BankAccount>>
                       .Success(await PagedList<BankAccount>.CreateAsync(query,
                           request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
