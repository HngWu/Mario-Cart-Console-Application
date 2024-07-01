using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Daisy : Character
    {
        public Daisy()
        {
        }

        public Daisy(string name, int hp, int exp, int level, string skill) : base("Daisy", 100, 0, 1, "Leadership")
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
