using UnityEngine;
using System.Collections;


public class ItemWeaponMechanical : ItemWeapon {
	public override Item getItem () {
		return getWeapon();
	}

	public override Weapon getWeapon() {
		return new WeaponMechanical(itemName, itemType, gold, silver, copper, isKeyItem, inventoryTexture, spritePrefab, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape);
	}
}