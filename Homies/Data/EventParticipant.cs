using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homies.Data
{
    public class EventParticipant
    {
        [Required]
        [ForeignKey(nameof(HelperId))]
        public string HelperId { get; set; } = string.Empty;

        [Required]
        public IdentityUser Helper { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(EventId))]
        public int EventId { get; set; }

        public Event Event { get; set; } = null!;

    }
}

//•	HelperId – a  string, Primary Key, foreign key (required)
//•	Helper – IdentityUser
//•	EventId – an integer, Primary Key, foreign key (required)
//•	Event – Event
