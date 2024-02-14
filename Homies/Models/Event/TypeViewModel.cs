using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants;

namespace Homies.Models.Event
{
    public class TypeViewModel
    {
        [Required(ErrorMessage = RequiredField)]
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredField)]
        [StringLength(TypeNameMaxLength,
            MinimumLength = TypeNameMinLength,
            ErrorMessage = RequiredLength)]
        public string Name { get; set; } = string.Empty;

    }
}