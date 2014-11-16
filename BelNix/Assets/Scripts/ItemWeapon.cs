using UnityEngine;
using System.Collections;

public enum DamageType {CRUSHING, PIERCING, SLASHING}

[RequireComponent (typeof (Character))]
public class ItemWeapon : MonoBehaviour {
	public int hit;
	public int range;
	public int numberOfDamageDice;
	public int diceType;
	public int damageBonus;
	public DamageType damageType;
	public int criticalChance;
	public int durabilityChance;
}
