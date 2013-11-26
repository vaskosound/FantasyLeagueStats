using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FantasyStatsApp.Models
{
    public class SubmitTeamPriceModel
    {
        [Required]
        [Range(90, 120, ErrorMessage="The Price must be between {1} and {2}")]
        [DisplayName("Your Team Price")]
        public decimal TeamPrice { get; set; }
    }
}