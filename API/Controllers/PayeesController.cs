using Application.Payees;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PayeesController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult> List([FromQuery] PayeeParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPayee(Guid id)
        {
            return HandleResult( await Mediator.Send(new Details.Query { Id = id }));
        }
        [HttpPost]
        public async Task<ActionResult> CreatePayee(Payee payee)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Payee = payee }));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditPayee(Guid id, Payee payee)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { Payee = payee }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayee(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
