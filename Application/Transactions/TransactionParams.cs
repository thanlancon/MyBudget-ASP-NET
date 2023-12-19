using Application.Core;

namespace Application.Transactions
{
    public class TransactionParams : PagingParams
    {
        public Guid BankID { get; set; }
    }
}
