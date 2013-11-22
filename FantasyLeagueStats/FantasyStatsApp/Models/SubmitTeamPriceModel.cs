using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FantasyStatsApp.Models
{
    public class SubmitTeamPriceModel
    {
        [Required]
        [DisplayName("Your Team Price")]
        public decimal TeamPrice { get; set; }
    }
}