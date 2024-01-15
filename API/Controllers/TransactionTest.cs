using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Transactions;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TransactionTest : BaseApiController
    {
        [HttpGet]
        public async Task<List<Transaction>> GetByBankID([FromBody] Transaction transaction)
        {
            return await Mediator.Send(new Test.Query{ transaction = transaction });
        }
    }
}
