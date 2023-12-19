using Application.Envelopes;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EnvelopesController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult> List([FromQuery] EnvelopeParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }
        [HttpGet]
        [Route("monthlybalances")]
        public async Task<ActionResult> MonthlyBalance([FromQuery] MonthlyActivitiesBalance.Query request)
        {
            return HandleResult(await Mediator.Send(new MonthlyActivitiesBalance.Query { month = request.month, year = request.year }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetEnvelope(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }
        [HttpPost]
        public async Task<IActionResult> CreateEnvelope(Envelope envelope)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Envelope = envelope }));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEnvelope(Guid id, Envelope envelope)
        {
            envelope.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Envelope = envelope }));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvelope(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
        [HttpPut("recalculateTotalBalance")]
        public async Task<IActionResult> RecalculateTotalBalance()
        {
            await Mediator.Send(new RecalculateTotalBalance.Query());
            return Ok();
        }
        [HttpPut("transferfund")]
        public async Task<IActionResult> TransferFund(EnvelopeBalanceTransfer envelopeBalanceTransfer)
        {
            return HandleResult(await Mediator.Send(new TransferFund.Command
            {
                envelopeBalanceTransfer = envelopeBalanceTransfer
            }));
        }
        [HttpPut("defaultenvelopefunding")]
        public async Task<IActionResult> SetDefaultEnvelopeFunding(string envelopeName = "Budget")
        {
            return HandleResult(await Mediator.Send(new DefaultEvelopeFunding.Query { EnvelopeName = envelopeName }));
        }
        [HttpPut("autozerobalance/{id}")]
        public async Task<IActionResult> AutoZeroBalance(Guid id)
        {
            return HandleResult(await Mediator.Send(new AutoZeroBalance.Command { envelopeID = id }));
        }
    }
}
