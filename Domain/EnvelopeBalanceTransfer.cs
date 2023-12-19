using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [NotMapped]
    public class EnvelopeBalanceTransfer
    {
        public Guid envelopeID_From { get; set; }
        public Guid envelopeID_To { get; set; }
        public decimal balanceTransfer { get; set; }
    }
}
