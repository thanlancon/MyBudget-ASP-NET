using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [NotMapped]
    public class EnvelopeMonthlyBalance
    {
        /// <summary>
        /// envelope ID
        /// </summary>
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// balance of a month of the year
        /// </summary>
        public decimal Balance { get; set; }

        
    }
}
