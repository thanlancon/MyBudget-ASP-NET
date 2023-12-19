using Application.BankAccounts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BankAccountsController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult> List([FromQuery] BankAccountParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Details(Guid Id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = Id }));
        }
        [HttpPost]
        public async Task<IActionResult> Create(BankAccount bankAccount)
        {
            return HandleResult(await Mediator.Send(new Create.Command { BankAccount = bankAccount }));
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult> Edit(Guid Id, BankAccount bankAccount)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { BankAccount = bankAccount }));
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(Guid Id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = Id }));
        }
    }
}
