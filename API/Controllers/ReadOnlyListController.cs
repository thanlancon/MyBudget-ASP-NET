using Application.ReadOnlyList;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ReadOnlyListController : BaseApiController
    {
        [HttpGet("listbanks")]
        public async Task<List<ValueAndText>> ListBanks()
        {
            return await Mediator.Send(new ListBanks.Query());
        }
        [HttpGet("listbankaccounts")]
        public async Task<List<ValueAndText>> ListBankAccounts()
        {
            return await Mediator.Send(new ListBankAccounts.Query());
        }
        [HttpGet("listbankaccounttypes")]
        public async Task<List<ValueAndText>> ListBankAccountTypes()
        {
            return await Mediator.Send(new ListBankAccountTypes.Query());
        }
        [HttpGet("listcategories")]
        public async Task<List<ValueAndText>> ListCategories()
        {
            return await Mediator.Send(new ListCategories.Query());
        }
        [HttpGet("listenvelopes")]
        public async Task<List<ValueAndText>> ListEnvelopes()
        {
            return await Mediator.Send(new ListEnvelopes.Query());
        }
        [HttpGet("listpayees")]
        public async Task<List<ValueAndText>> ListPayees()
        {
            return await Mediator.Send(new ListPayees.Query());
        }
    }
}
