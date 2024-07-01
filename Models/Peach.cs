using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Peach : Character
    {
        public Peach()
        {
        }
        public Peach(string name, int hp, int exp, int level, string skill) : base("Peach", 100, 0, 1, "Combat Skills")
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
