using Application.Transactions;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public partial class TransactionsController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult> GetTransactions([FromQuery] TransactionParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }
        [HttpPost]
        public async Task<ActionResult> CreateTransaction(Transaction transaction)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Transaction = transaction }));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }

}
