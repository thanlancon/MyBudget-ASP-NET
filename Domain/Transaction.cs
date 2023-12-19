using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public partial class Transaction
    {
        public Transaction()
        {

        }
        public Guid Id { get; set; }
        /// <summary>
        /// Transaction ID of a transaction where the fund came from
        /// 0 : fund was not transfer from any other transaction
        /// </summary>
        public Guid TransactionTransferID { get; set; } = Guid.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public Guid BankId { get; set; } = Guid.Empty;
        public Guid BankId_Transfer { get; set; } = Guid.Empty;
        public Guid PayeeId { get; set; } = Guid.Empty;
        public Guid EnvelopeId { get; set; } = Guid.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Inflow { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Outflow { get; set; } = 0;
        public bool IsCleared { get; set; }=false;
        [Column(TypeName ="decimal(18,2)")]
        public decimal TotalBalance { get; set; }
        [StringLength(200)]
        public string Note { get; set; }
        /// <summary>
        /// sequence that a transaction is entered.
        /// Used for sorting
        /// </summary>
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SequenceNumber { get; set; }
        public void UnlinkTransferTransaction()
        {
            TransactionTransferID = Guid.Empty;
        }
    }
}
