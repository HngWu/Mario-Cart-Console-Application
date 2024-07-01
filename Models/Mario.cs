using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Mario : Character
    {
        public Mario()
        {
        }
        public Mario(string name, int hp, int exp, int level, string skill) : base("Mario", 100, 0, 1, "Magic Abilities")
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
