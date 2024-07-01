using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Luigi : Character
    {
        public Luigi()
        {
        }
        public Luigi(string name, int hp, int exp, int level, string skill) : base("Luigi", 100, 0, 1, "Precision and Accuracy")
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
