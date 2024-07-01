using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Models
{
    public class Leaderboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public int Score { get; set; }

        public Leaderboard()
        {
        }

        public Leaderboard(string name, string character, int score)
        {
            Name = name;
            Character = character;
            Score = score;
        }
    }

}
