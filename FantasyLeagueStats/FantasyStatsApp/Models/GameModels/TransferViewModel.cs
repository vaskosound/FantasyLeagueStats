using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models.GameModels
{
    public class TransferViewModel
    {
        public PlayerGameViewModel[] Team { get; set; }

        public IEnumerable<PlayerBasicModel> Players { get; set; }

    }
}