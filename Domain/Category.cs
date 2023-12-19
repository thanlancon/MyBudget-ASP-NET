using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required, StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
