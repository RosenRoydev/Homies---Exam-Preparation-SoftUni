using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants;
namespace Homies.Models.Event

{
    public class EventAddViewModel
    {
        [Required(ErrorMessage = RequiredField)]
        [StringLength(EventNameMaxLength,
            MinimumLength = EventNameMinLength,
            ErrorMessage = RequiredLength)]
        
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [StringLength(EventDescriptionMaxLength,
            MinimumLength = EventDescriptionMinLength,
            ErrorMessage = RequiredLength)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        public string Start { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        public string End { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        public int TypeId { get; set; }
        public IEnumerable <TypeViewModel> Types { get; set; } = new List<TypeViewModel>();
    }
}
