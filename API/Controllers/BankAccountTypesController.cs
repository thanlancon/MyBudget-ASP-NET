using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.BankAccountTypes;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BankAccountTypesController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult> List([FromQuery] BankAccountTypeParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Details(Guid id)
        {
            return HandleResult( await Mediator.Send(new Details.Query { Id = id }));
        }
        //[Authorize(Policy = "DefaultPolicy")]
        [HttpPost]
        public async Task<ActionResult> Create(BankAccountType bankAccountType)
        {
            return HandleResult(await Mediator.Send(new Create.Command { BankAccountType = bankAccountType }));
        }
        //[Authorize(Policy = "DefaultPolicy")]
        [HttpPut]
        public async Task<ActionResult> Edit(BankAccountType bankAccountType)
        {
            return HandleResult(await Mediator.Send(new Edit.Command { BankAccountType = bankAccountType }));
        }
        //[Authorize(Policy = "DefaultPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
