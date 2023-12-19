using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BankAccountType
    {
        public Guid Id { get; set; }

        [Required,StringLength(20,MinimumLength =3)]
        public string Code { get; set; }
        [Required,StringLength(20,MinimumLength =3)]
        public string Name { get; set; }
    }
}
