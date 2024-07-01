using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Waluigi : Character
    {

        public Waluigi(string name, int hp, int exp, int level, string skill) : base("Waluigi", 100, 0, 1, "Agility")
        {

            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }

}
