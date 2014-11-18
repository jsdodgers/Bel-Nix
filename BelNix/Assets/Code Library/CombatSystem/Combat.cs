using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CharacterInfo;

namespace CombatSystem
{
    class Combat
    {

        public static void attack(ref CharacterSheet attacker, ref CharacterSheet defender)
        {
            // Calculate hit/miss, if miss we get to skip the following logic.


            // use the attacker's offensive stats, compare them to the defender's AC.
            // Deal damage appropriately.
        }

        public static void attack(Unit attacker, Unit defender)
        {
            //attack(attacker.ch
            //int attackPower = attacker.GetComponent<CharacterLoadOut>().rightHand.rollDamage();
        }
    }
}
