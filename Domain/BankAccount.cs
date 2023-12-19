using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BankAccount
    {

        public Guid Id { get; set; }

        [Required,StringLength(20,MinimumLength =3)]
        public string Code { get; set; }

        public Guid BankId { get; set; }

        public Guid BankAccountTypeId { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Display(Name = "Cleared Balance")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ClearedBalance { get; set; } = 0;

        [Display(Name = "Uncleared Balance")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnClearedBalance { get; set; } = 0;

        [Display(Name = "Total Balance")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBalance
        {
            get { return ClearedBalance + UnClearedBalance; }
        }
    }
}
