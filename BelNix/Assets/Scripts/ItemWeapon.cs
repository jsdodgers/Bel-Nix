using UnityEngine;
using System.Collections;

public enum DamageType {CRUSHING, PIERCING, SLASHING}

[RequireComponent (typeof (Character))]
public class ItemWeapon : Item1 {
	public int hit;
	public int range;
	public int numberOfDamageDice;
	public int diceType;
	public int damageBonus;
	public DamageType damageType;
	public int criticalChance;
	public int durabilityChance;
	public bool isRanged = false;

	public int rollDamage() {
		return rollDamage(false);
	}

	public int rollDamage(bool critical)
	{
		int damageDealt = 0;
		for(int i = 0; i < numberOfDamageDice; i++)
		{
			damageDealt += (critical ? diceType : Random.Range(1, diceType+1));
		}

		return damageDealt;
	}
}
