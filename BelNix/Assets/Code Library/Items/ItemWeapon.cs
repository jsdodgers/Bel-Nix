using UnityEngine;
using System.Collections;

public enum DamageType {Crushing, Piercing, Slashing}

[RequireComponent (typeof (Character))]
public class ItemWeapon : EditorItem {

	public int hit;
	public int range;
	public int numberOfDamageDice;
	public int diceType;
	public int damageBonus;
	public DamageType damageType;
	public int criticalChance;
	public int durabilityChance;
	public bool isRanged = false;
	public Vector2[] shape;
//	Weapon weapon;
	public Weapon getWeapon() {
//		if (weapon == null)
			return new Weapon(itemName, itemType, gold, silver, copper, isKeyItem, inventoryTexture, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape);
//		return weapon;
	}
/*
	public void Update() {
		Debug.LogWarning("Weapon Update");
	}*/

}
