using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class SubmitTeamPrice
    {
        [Required]
        [DisplayName("Team Price")]
        public decimal TeamPrice { get; set; }
    }
}