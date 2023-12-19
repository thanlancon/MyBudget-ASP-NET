using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{

    [NotMapped]
    public class ValueAndText
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
