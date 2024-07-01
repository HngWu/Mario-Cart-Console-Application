using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MushroomPocket.Model
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int EXP { get; set; }
        public int Level { get; set; }
        public string Skill { get; set; }

        public Character()
        {
        }

        public Character(string name, int hp, int exp, int level, string skill)
        {

            Name = name;
            HP = hp;
            EXP = exp;
            Level = level;
            Skill = skill;
        }
    }
}
