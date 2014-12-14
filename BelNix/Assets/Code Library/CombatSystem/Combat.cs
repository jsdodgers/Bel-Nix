using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CharacterInfo;
using UnityEngine;

namespace CombatSystem
{
    class Combat
    {
        /*
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
        */

        public static Hit rollHit(Unit attacker)
        {
            int diceRoll = rollD20();
            int criticalHitChance = attacker.characterSheet.characterSheet.characterLoadout.rightHand.criticalChance;
            return new Hit(attacker.characterSheet.characterSheet.skillScores.getScore(Skill.Melee) + diceRoll + ((flanking(attacker)) ? 2 : 0), (diceRoll * 5) > (100 - criticalHitChance));
        }


        public static int rollDamage(Unit attacker) { return rollDamage(attacker, false); }
        public static int rollDamage(Unit attacker, bool critical)
        {
            return 0;   //return attacker.characterSheet.characterSheet.characterLoadout.rightHand.rollDamage(critical) + (critical ? attacker.characterSheet.characterSheet.combatScores.getCritical()
        }

        // Returns true if the attacker and a teammate are flanking the defender (they are on opposite sides of the defender)
        public static bool flanking(Unit attacker)
        {
            // Get the positions on the grid for the attacker and defender
            Vector3 attackerPosition = attacker.position;
            Vector3 defenderPosition = attacker.attackEnemy.position;

            // Store the XY coordinates only, flip the Y coordinate
            Vector2 processedAttackerPosition = new Vector2(attackerPosition.x,-attackerPosition.y);
            Vector2 processedDefenderPosition = new Vector2(defenderPosition.x, -defenderPosition.y);

            // Use the processed coordinates to obtain the coordinate that a teammate of the attacker 
            //      would need to occupy to flank the defender
            Vector2 teammateCoords = new Vector2((processedAttackerPosition.x == processedDefenderPosition.x) ? processedAttackerPosition.x : processedDefenderPosition.x - (processedAttackerPosition.x - processedDefenderPosition.x),
                                                 (processedAttackerPosition.y == processedDefenderPosition.y) ? processedAttackerPosition.y : processedDefenderPosition.y - (processedAttackerPosition.y - processedDefenderPosition.y));

            // return whether or not a teammate is there, which determines whether the defender is flanked or not
            return attacker.mapGenerator.tiles[(int)teammateCoords.x, (int)teammateCoords.y].hasAlly(attacker);
        }

        // Simulate a single 20-sided die being rolled
        private static int rollD20()
        {
            return UnityEngine.Random.Range(1, 21);
        }
    }
}