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

	public int rollDamage()
	{
		int damageDealt = 0;
		for(int i = 0; i < numberOfDamageDice; i++)
		{
			damageDealt += Random.Range(1, diceType);
		}

		return damageDealt;
	}
}
