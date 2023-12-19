using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Bank
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name ="Bank Code")]
        [StringLength(20,MinimumLength =3)]
        public string Code { get; set; }

        [Required]
        [Display(Name ="Bank Name")]
        [StringLength(50,MinimumLength =3)]
        public string Name { get; set; }


    }
}
