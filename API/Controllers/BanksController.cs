using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Banks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class BanksController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult> List([FromQuery] BankParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBank(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }
        [HttpPost]
        public async Task<ActionResult> CreateBank(Bank bank)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Bank = bank }));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditBank(Guid id, Bank bank)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { Bank = bank }));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBank(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
