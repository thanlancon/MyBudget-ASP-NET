using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.ReadOnlyList
{
    public class ReadOnlyListBase
    {
/*
        public class Query : IRequest<List<ValueAndText>> { };
        public class Handler : IRequestHandler<Query, List<ValueAndText>>
        {
            private DataContext _context;
            protected List<T> _datasource;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<List<ValueAndText>> Handle(Query request, CancellationToken cancellationToken)
            {
                var returnList = new List<ValueAndText>();
                var listItems =  _datasource;
                listItems.ForEach(li =>
                {
                    returnList.Add(new ValueAndText
                    {
                        Value = {...li, ["Id"]},
                        //Text = li.Code
                    });
                });
                return returnList;
            }
        }
        */
    }
}
