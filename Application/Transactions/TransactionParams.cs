using Application.Core;

namespace Application.Transactions
{
    public class TransactionParams : PagingParams
    {
        public Guid BankID { get; set; }
    }
    public class MonthlyEnvelopeParams : PagingParams
    {
        public Guid envelopeID { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }
}
