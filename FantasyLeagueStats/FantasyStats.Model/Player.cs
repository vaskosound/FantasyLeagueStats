using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FantasyStats.Model
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int TeamId { get; set; }

        public virtual Team Team { get; set; }

        [Required]
        public Position Position { get; set; }

        public double Selected { get; set; }

        public int Points { get; set; }

        public decimal Price { get; set; }

        [Required]
        public bool IsInjured { get; set; }

        public int MinutesPlayed { get; set; }

        public decimal PointsPerGame { get; set; }

        public decimal PlayerForm { get; set; }

        public int RoundScore { get; set; }
    }
}

