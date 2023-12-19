using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Envelope
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TotalBalance { get; set; } = 0;

        public Guid EnvelopeId_Funding { get; set; } = Guid.Empty;

    }
}
