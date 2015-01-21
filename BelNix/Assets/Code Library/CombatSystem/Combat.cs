using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Combat {
	
	public static Hit rollHit(Unit attacker) {
		int dieRoll = rollD20();
		int criticalHitChance = attacker.getCritChance();//attacker.characterSheet.characterSheet.characterLoadout.rightHand.criticalChance;
		return new Hit(attacker.rollForSkill(Skill.Melee, attacker.attackEnemyIsFavoredRace(), 20, dieRoll) + (flanking(attacker) ? 2 : 0) + (attacker.characterSheet.characterSheet.characterProgress.hasFeature(ClassFeature.Uncanny_Knowledge) ? 1 : 0) + (attacker.hasWeaponFocus() ? 2 : 0) - attacker.temperedHandsMod, (dieRoll * 5) > (100 - criticalHitChance));
	}
	
	
	public static int rollDamage(Unit attacker) { return rollDamage(attacker, false); }
	public static int rollDamage(Unit attacker, bool critical)  {
		return 0;   //return attacker.characterSheet.characterSheet.characterLoadout.rightHand.rollDamage(critical) + (critical ? attacker.characterSheet.characterSheet.combatScores.getCritical()
	}
	
	// Returns true if the attacker and a teammate are flanking the defender (they are on opposite sides of the defender)
	public static bool flanking(Unit attacker) {
		return flanking(attacker, attacker.attackEnemy);
	}
	
	public static bool flanking(Unit attacker, Unit enemy) {
		// Get the positions on the grid for the attacker and defender
		Vector3 attackerPosition = attacker.position;
		Vector3 defenderPosition = enemy.position;
		
		// Store the XY coordinates only, flip the Y coordinate
		Vector2 processedAttackerPosition = new Vector2(attackerPosition.x,-attackerPosition.y);
		Vector2 processedDefenderPosition = new Vector2(defenderPosition.x, -defenderPosition.y);
		
		// Use the processed coordinates to obtain the coordinate that a teammate of the attacker 
		//      would need to occupy to flank the defender
		Vector2 teammateCoords = new Vector2((processedAttackerPosition.x == processedDefenderPosition.x) ? processedAttackerPosition.x : processedDefenderPosition.x - (processedAttackerPosition.x - processedDefenderPosition.x),
		                                     (processedAttackerPosition.y == processedDefenderPosition.y) ? processedAttackerPosition.y : processedDefenderPosition.y - (processedAttackerPosition.y - processedDefenderPosition.y));
		
		// return whether or not a teammate is there, which determines whether the defender is flanked or not
		Tile t = attacker.mapGenerator.tiles[(int)teammateCoords.x, (int)teammateCoords.y];
		return t.hasAliveAlly(attacker);
	}
	
	// Simulate a single 20-sided die being rolled
	public static int rollD20() {
		return UnityEngine.Random.Range(1, 21);
	}
}