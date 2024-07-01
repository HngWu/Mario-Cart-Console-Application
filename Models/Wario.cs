using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Wario : Character
    {
        public Wario()
        {
        }
        public Wario(string name, int hp, int exp, int level, string skill) : base("Wario", 100, 0, 1, "Strength")
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
