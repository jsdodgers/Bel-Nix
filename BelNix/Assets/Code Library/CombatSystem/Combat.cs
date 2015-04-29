using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Combat  {
	
	public static Hit rollHit(Unit attacker)  {
		int dieRoll = rollD20();
		int criticalHitChance = attacker.getCritChance();//attacker.characterSheet.characterSheet.characterLoadout.rightHand.criticalChance;
		return new Hit(attacker.rollForSkill((attacker.getWeapon().isRanged ? Skill.Ranged : Skill.Melee), attacker.attackEnemyIsFavoredRace(), 20, dieRoll) + (flanking(attacker) ? 2 : 0) + (attacker.hasUncannyKnowledge() ? 1 : 0) + (attacker.hasWeaponFocus() ? 2 : 0) + attacker.getOneOfManyBonus(OneOfManyMode.Hit) - attacker.temperedHandsMod, (dieRoll * 5) > (100 - criticalHitChance));
	}
	
	
	public static int rollDamage(Unit attacker)  { return rollDamage(attacker, false); }
	public static int rollDamage(Unit attacker, bool critical)   {
		return 0;   //return attacker.characterSheet.characterSheet.characterLoadout.rightHand.rollDamage(critical) + (critical ? attacker.characterSheet.characterSheet.combatScores.getCritical()
	}
    public static int rollDamage(Unit attacker, Unit attackedEnemy, bool crit) {
        return attacker.rollDamage(crit);
    }


    public static AttackHandler getAttackHandler() {
        try {
            AttackHandler attackHandler = GameObject.Find("Event Handler").GetComponent<AttackHandler>();
            return attackHandler;
        }
        catch (Exception e) {
            GameObject eventHandler = new GameObject("Event Handler");
            eventHandler.transform.SetParent(GameObject.Find("MapGenerator").transform);
            return eventHandler.AddComponent<AttackHandler>();
        }
    }

    public static ComposureDamageHandler getComposureDamageHandler()
    {
        try
        {
            ComposureDamageHandler composureDamageHandler = GameObject.Find("Event Handler").GetComponent<ComposureDamageHandler>();
            return composureDamageHandler;
        }
        catch (Exception e)
        {
            // I'll use getAttackHandler as a shortcut, letting it retrieve or set up the Event Handler for me.
            GameObject eventHandler = getAttackHandler().gameObject;
            return eventHandler.AddComponent<ComposureDamageHandler>();
        }
    }

    private static void OnAttackHit(Unit attacker, Unit attackedEnemy, int damage, bool ranged = false, bool crit = false, bool overClockedAttack = false) {
        AttackHandler attackHandler = getAttackHandler();
        attackHandler.OnAttackHit(attacker, attackedEnemy, damage, ranged, crit, overClockedAttack);
    }
    private static void OnAttackMissed(Unit attacker, Unit attackedEnemy, bool ranged = false, bool overClockedAttack = false) {
        AttackHandler attackHandler = getAttackHandler();
        attackHandler.OnAttackMissed(attacker, attackedEnemy, ranged, overClockedAttack);
    }

    private static void OnComposureDamageHit(Unit attacker, Unit attackedEnemy, int damage)
    {
        ComposureDamageHandler composureDamageHandler = getComposureDamageHandler();
        composureDamageHandler.OnComposureDamageHit(attacker, attackedEnemy, damage);
    }
    private static void OnComposureDamageMissed(Unit attacker, Unit attackedEnemy)
    {
        ComposureDamageHandler composureDamageHandler = getComposureDamageHandler();
        composureDamageHandler.OnComposureDamageMissed(attacker, attackedEnemy);
    }


    public static void dealDamage(Unit attacker, Unit attackedEnemy, bool overClockedAttack = false) {

        bool animate = false;
        if (!attacker.damageCalculated) {
            attacker.calculateDamage();
            animate = true;
        }
        attacker.damageCalculated = false;

        Hit hit     = Combat.rollHit(attacker);
        Hit critHit = Combat.rollHit(attacker);
        int enemyAC = attackedEnemy.getAC();
        bool crit = hit.crit && critHit.hit >= enemyAC;
        int weaponDamage = (overClockedAttack ? overClockDamage(attacker) : rollDamage(attacker, attackedEnemy, crit));//.characterLoadout.rightHand.rollDamage();
        bool didHit = hit.hit >= enemyAC || hit.crit;
        attackedEnemy.showDamage(weaponDamage, didHit, crit);
        
        BattleGUI.writeToConsole(attacker.getName() + 
            (didHit ? (overClockedAttack ? " over clocked " : (crit ? " critted " : " hit ")) : " missed ") + 
            attackedEnemy.getName() + (didHit ? " with " + 
            (attacker.getWeapon() == null ? attacker.getGenderString() + " fist " : attacker.getWeapon().itemName + " ") + 
            "for " + weaponDamage + " damage!" : "!"), (attacker.team == 0 ? Log.greenColor : Color.red));

        if (didHit)  {
            attackedEnemy.damage(weaponDamage, attacker, animate);
            BloodScript.spillBlood(attacker, attackedEnemy, weaponDamage);
            OnAttackHit(attacker, attackedEnemy, weaponDamage, false, crit, overClockedAttack);
        }
        else
            OnAttackMissed(attacker, attackedEnemy, false, overClockedAttack);
        //    attackedEnemy.damage(0, attacker);

        if (overClockedAttack) {
            Debug.Log("Over Clocked Attack!!!");
            Weapon weapon = attacker.characterSheet.characterSheet.characterLoadout.rightHand;
            if (weapon is ItemMechanical) {
                ((WeaponMechanical)weapon).overClocked = true;
            }
        }
        if (!attackedEnemy.moving) {
            attackedEnemy.attackedByCharacter = attacker;
            attackedEnemy.setRotationToAttackedByCharacter();
            attackedEnemy.beingAttacked = true;
        }
        else {
            attackedEnemy.shouldMove--;
            if (attackedEnemy.shouldMove < 0) attackedEnemy.shouldMove = 0;
        }
        //Debug.Log((hit.hit > 4 ? "wapoon: " + weaponDamage : "miss!") + " hit: " + hit.hit + "  " + hit.crit + "  critHit: " + critHit.hit + "   enemyAC: " + enemyAC);
    }

    public static int overClockDamage(Unit attacker) {
        return attacker.characterSheet.overloadDamage();
    }
	
    public static bool dealComposureDamage(Unit attacker, Unit attackedEnemy, int damage)
    {
        if ((damage > 0) && (!attackedEnemy.characterSheet.characterSheet.combatScores.isInPrimalState()))
        {
            attackedEnemy.crushingHitSFX();
            attackedEnemy.loseComposure(damage);
            
            if (attackedEnemy.characterSheet.characterSheet.combatScores.isInPrimalState())
            {
                attackedEnemy.inPrimal = true;
                attackedEnemy.primalControl = 0;
                attackedEnemy.primalInstigator = attacker;
                attackedEnemy.primalTurnsLeft = attacker.characterSheet.characterSheet.combatScores.getDominion() + 1;
                AudioManager.getAudioManager().playAudioClip(SFXClip.ComposureBreak, 0.4f);
                return true;
            }
            AudioManager.getAudioManager().playAudioClip(SFXClip.ComposureDamage, 0.4f);
        }
        return false;
    }

	// Returns true if the attacker and a teammate are flanking the defender (they are on opposite sides of the defender)
	public static bool flanking(Unit attacker)  {
		return flanking(attacker, attacker.attackEnemy);
	}
	
	public static bool flanking(Unit attacker, Unit enemy)  {
		if (attacker == null || enemy == null) return false;
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
		if (teammateCoords.x < 0 || teammateCoords.y < 0 || teammateCoords.x >= attacker.mapGenerator.actualWidth || teammateCoords.y >= attacker.mapGenerator.actualHeight) return false;
		// return whether or not a teammate is there, which determines whether the defender is flanked or not
		Tile t = attacker.mapGenerator.tiles[(int)teammateCoords.x, (int)teammateCoords.y];
		return t.hasAliveAlly(attacker);
	}
	
	// Simulate a single 20-sided die being rolled
	public static int rollD20()  {
		return UnityEngine.Random.Range(1, 21);
	}
}

public class AttackHandler : MonoBehaviour {
    public delegate void attackEventHandler(AttackEventArgs args);
    public event attackEventHandler attackHit;
    public event attackEventHandler attackMissed;

     public void OnAttackHit(Unit attacker, Unit attackedEnemy, int damage, bool ranged = false, bool crit = false, bool overClockedAttack = false) {
        if (attackHit != null) {
            attackHit(new AttackEventArgs() {
                attackingUnit = attacker.gameObject,
                attackedUnit = attackedEnemy.gameObject,
                missed = false,
                damageDealt = damage,
                rangedAttack = ranged,
                criticalHit = crit,
                overClockedAttack = overClockedAttack
            });
        }
    }
    public void OnAttackMissed(Unit attacker, Unit attackedEnemy, bool ranged = false, bool overClockedAttack = false) {
        if (attackMissed != null) {
            attackMissed(new AttackEventArgs() {
                attackingUnit = attacker.gameObject,
                attackedUnit = attackedEnemy.gameObject,
                missed = true,
                damageDealt = 0,
                rangedAttack = ranged,
                criticalHit = false,
                overClockedAttack = overClockedAttack
            });
        }
    }
}

public class AttackEventArgs : EventArgs {
    public GameObject attackingUnit;
    public GameObject attackedUnit;
    public int damageDealt;
    public bool missed;
    public bool rangedAttack;
    public bool criticalHit;
    public bool overClockedAttack;
}

public class ComposureDamageHandler : MonoBehaviour
{
    public delegate void composureDamageHandler(ComposureDamageEventArgs args);
    public event composureDamageHandler composureDamageHit;
    public event composureDamageHandler composureDamageMissed;

    public void OnComposureDamageHit(Unit attacker, Unit attackedEnemy, int damage)
    {
        if (composureDamageHit != null)
        {
            composureDamageHit(new ComposureDamageEventArgs()
            {
                attackingUnit = attacker.gameObject,
                attackedUnit = attackedEnemy.gameObject,
                missed = false,
                damageDealt = damage
            });
        }
    }
    public void OnComposureDamageMissed(Unit attacker, Unit attackedEnemy)
    {
        if (composureDamageMissed != null)
        {
            composureDamageMissed(new ComposureDamageEventArgs()
            {
                attackingUnit = attacker.gameObject,
                attackedUnit = attackedEnemy.gameObject,
                missed = true,
                damageDealt = 0,
            });
        }
    }
}

public class ComposureDamageEventArgs : EventArgs
{
    public GameObject attackingUnit;
    public GameObject attackedUnit;
    public int damageDealt;
    public bool missed;
}